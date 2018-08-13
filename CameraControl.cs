using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour {

    private Camera camera;

	void Start () {
        camera = transform.GetComponent<Camera>();
	}
	
	void Update () {
        camera.orthographicSize = Mathf.Lerp(camera.orthographicSize,Mathf.Max(camera.orthographicSize,Mathf.Clamp(AreaSize.radius,5f,(AreaSize.radius / 2))+AreaSize.radius/20f),0.2f);
        //camera.orthographicSize = (AreaSize.radius / 2) + 1;
	}
}
