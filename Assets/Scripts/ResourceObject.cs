using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ResourceObject : MonoBehaviour {

	bool inTrigger;
	Collider collision;

	// Use this for initialization
	void Start () {
		inTrigger = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (inTrigger == true && Input.GetKeyDown (KeyCode.G))
		{
			Debug.LogError ("Pressing G!!!!!!!!!!!!!!!!!!!!!!");
			var hit = collision.transform.parent.gameObject;
			var playerstate = hit.GetComponent<PlayerState>();
			//playerstate.takeResource ();
		}
	}

	void OnTriggerEnter(Collider c)
	{
		if(c.tag == "Player")
		{
			inTrigger = true;
			collision = c;
		}
	}

	void OnTriggerExit(Collider c)
	{
		if (c.tag == "Player")
		{
			inTrigger = false;
		}
	}
}
