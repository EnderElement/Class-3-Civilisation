using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemController : MonoBehaviour {

    public Transform home;
    private float radius;

	private void Start()
	{
        //DontDestroyOnLoad(gameObject);
	}

	void Update () {
        float dist = Vector3.Distance(home.position, transform.position);
        //radius = Mathf.Lerp(radius, Mathf.Max(1f,((dist/6f)/Mathf.Max(1f, (GameController.reach / 30f)))), 0.2f);
        radius = Mathf.Lerp(radius, Mathf.Max(1f,((dist/6f)/Mathf.Max(1f, (GameController.reach / 30f))))+dist/10f, 0.2f);
        //if (GameController.claimedSystems.ContainsValue(transform)) transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.white;
        transform.GetChild(0).localScale = new Vector3(radius, radius, radius);
	}
}
