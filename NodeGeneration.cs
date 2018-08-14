using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeGeneration : MonoBehaviour {

    public Object Node;
    public int separation = 1;
	public int maxIter = 100;
    public static List<Transform> nodeList = new List<Transform>();
	//private int maxIter;

	//private void Awake()
	//{
	//maxIter = GameController.maxIter;

	//}

	private void Start()
	{
        //DontDestroyOnLoad(gameObject);
	}

	void Awake () {
        for (int i = 1; i < maxIter; i++) {
            Instantiate(Node, Random.insideUnitCircle * i * separation, Quaternion.Euler(0f, 0f, 0f),gameObject.transform);
            nodeList.Add(transform.GetChild(i));
        }
	}
}