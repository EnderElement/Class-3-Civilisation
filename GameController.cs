using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Analytics;

public class GameController : MonoBehaviour {

    public static float reach;
    public float R = 1;
    public static int maxIter;
    public int maxNodes = 100;
    public Transform home;
    public GameObject endScreen;

    public Text distText;
    public Text scoreText;

    private Transform currentSystem;
    public static SortedList<int,Transform> claimedSystems = new SortedList<int,Transform>();
    public static List<Transform> nodeInRadius = new List<Transform>();
    public static SortedDictionary<float,Transform> distList = new SortedDictionary<float,Transform>();
    private SortedList<float,float> distListKeys = new SortedList<float,float>();
    private LineRenderer line;
    private bool traverse; 
    private int faketime;
    private int index;
    private int i;
    private float prevR;
    private int civSize;
    public static float totalLength;
    private AnalyticsEventTracker eventTracker;
    public static bool endGame;

	private void Awake()
	{
        //DontDestroyOnLoad(home.gameObject);
        //DontDestroyOnLoad(distText.gameObject);
        //DontDestroyOnLoad(endScreen);
        //foreach (Transform node in NodeGeneration.nodeList)
        //{
        //    DontDestroyOnLoad(node.gameObject);
        //    float dist = Vector3.Distance(home.position, node.position);
        //    distList.Add(dist, node);
        //    distListKeys.Add(dist, dist);
        //}
	}

	void Start () {
        currentSystem = home;
        civSize = 0;
        claimedSystems.Add(civSize,home);
        index = 1;
        faketime = 0;
        i = 1;
        traverse = false;
        endGame = false;
        foreach (Transform node in NodeGeneration.nodeList)
            {
                float dist = Vector3.Distance(home.position, node.position);
                distList.Add(dist, node);
                distListKeys.Add(dist, dist);
            }
        line = GetComponent<LineRenderer>();
        eventTracker = GetComponent<AnalyticsEventTracker>();
        eventTracker.name = "TotalDistance";
        distText.GetComponent<Text>();
        distText.text = "Network Distance: 0ly";
	}

	void Update () {
        R = Mathf.Clamp(R, 1f, R);
        reach = R;
        foreach (Transform node in NodeGeneration.nodeList)
        {
            if (Vector3.Distance(home.position, node.position) < R / 2f + 0.5f && !nodeInRadius.Contains(node))
            {
                nodeInRadius.Add(node);
                node.GetChild(0).GetComponent<SpriteRenderer>().color = Color.white;
                node.GetChild(0).GetComponent<Klak.Motion.ConstantMotion>().rotationSpeed = 20f;
            }
        }
        //prevR = R;
        //AreaExpansion();
        ObjectClick();
        GameArea();
        if (claimedSystems.Count >= NodeGeneration.nodeList.Count) {
            if (!endGame)
            {
                ReportFinalDist(totalLength);
            }
            //eventTracker.TriggerEvent();
            distText.text = "";
            scoreText.text = "Final Network Distance: " + Convert.ToString(Mathf.Round(totalLength))+"ly";
            endScreen.SetActive(true);
            endGame = true;
            //Debug.Log("End game");
        }
        //Debug.Log(Time.fixedTime);
        //Debug.Log(nodeInRadius.Count);
        //Debug.Log(claimedSystems.Count);
        //Debug.Log(NodeGeneration.nodeList.Count);
	}

    void ObjectClick() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            bool inRange = false;
            if (Vector3.Distance(home.position, hit.transform.position) <= R / 2f + 0.5f || nodeInRadius.Count <= claimedSystems.Count)
            {
                inRange = true;
            }
            if (hit.transform.tag == "Node" && !claimedSystems.ContainsValue(hit.transform) && inRange)
            {
                civSize += 1;
                claimedSystems.Add(civSize,hit.transform);
                //hit.transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.white;
                totalLength += Vector3.Distance(claimedSystems[civSize - 1].position, claimedSystems[civSize].position) + Vector3.Distance(home.position,claimedSystems[civSize].position);
                traverse = true;
                distText.text = "Network Distance: "+Convert.ToString(Mathf.Round(totalLength))+"ly";
            }
        }
    }

    void GameArea() {
        float dist = Vector3.Distance(home.position, claimedSystems[civSize].position);
        line.positionCount = claimedSystems.Count+1;
        line.SetPosition(0, home.position);
        if (traverse)
        {
            R = Mathf.Max(dist * 2f + 0.5f,R);
            line.SetPosition(civSize, claimedSystems[civSize].position);
            traverse = false;
        }
    }

    void AreaExpansion() {
        index = Mathf.Clamp(index, 1, NodeGeneration.nodeList.Count);
        if (faketime % 100f == 0) traverse = true;
        faketime++;
        maxIter = maxNodes;
        if (traverse)
        {
            float lowDist = distListKeys.Keys[index];
            Transform prevSystem = currentSystem;
            currentSystem = distList[lowDist];
            R = lowDist*2f;
            Debug.DrawRay(prevSystem.position, currentSystem.position);
            //claimedSystems.Add(currentSystem);
            prevSystem = currentSystem;
            distList.Clear();
            distListKeys.Clear();
            index++;
            traverse = false;
        }
    }

    public void QuitGame() {
        Application.Quit();
    }

    public void RestartGame() {
        SceneManager.LoadScene("Universe");
    }

    public void ReportFinalDist (float finaldist) {
        AnalyticsEvent.Custom("FinalDist", new Dictionary<string, object>
        {
            { "totalLength", finaldist },
            { "time_elapsed", Time.timeSinceLevelLoad }
        });
        Debug.Log("exported score");
    }
}
