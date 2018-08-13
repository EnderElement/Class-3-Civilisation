using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
    private float totalLength;

	void Start () {
        currentSystem = home;
        civSize = 0;
        claimedSystems.Add(civSize,home);
        index = 1;
        faketime = 0;
        i = 1;
        traverse = false;
        line = GetComponent<LineRenderer>();
        distText.GetComponent<Text>();
        foreach (Transform node in NodeGeneration.nodeList)
        {
            float dist = Vector3.Distance(home.position, node.position);
            distList.Add(dist,node);
            distListKeys.Add(dist, dist);
        }
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
            }
        }
        //prevR = R;
        //AreaExpansion();
        ObjectClick();
        GameArea();
        if (claimedSystems.Count >= NodeGeneration.nodeList.Count+1) {
            distText.text = "";
            scoreText.text = "Final Network Distance: " + Convert.ToString(Mathf.Round(totalLength))+"ly";
            endScreen.SetActive(true);
            Debug.Log("End game");
        }
        //Debug.Log(Time.fixedTime);
        //Debug.Log(nodeInRadius.Count);
        Debug.Log(claimedSystems.Count);
        Debug.Log(NodeGeneration.nodeList.Count+1);
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
        Debug.Log("Quit");
        Application.Quit();
    }

    public void RestartGame() {
        Debug.Log("Restart");
        SceneManager.LoadScene("Universe");
    }
}
