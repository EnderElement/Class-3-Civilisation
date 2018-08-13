using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaSize : MonoBehaviour {
    
    public static float radius;
    //public static List<Transform> nodeInRadius = new List<Transform>();

	void Update () {
        radius = Mathf.Lerp(radius,GameController.reach,0.2f);
        transform.localScale = new Vector3(radius + 0.5f, radius + 0.5f, radius + 0.5f);
        foreach (Transform node in NodeGeneration.nodeList) {
            //if (GameController.distList.Keys[radius] <= radius) nodeInRadius.Add(node);
        }
	}
}
