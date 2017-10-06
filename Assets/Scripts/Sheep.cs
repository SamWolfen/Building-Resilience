using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Sheep : MonoBehaviour {


	Color c;

	void Start()
	{
		c = gameObject.GetComponent<MeshRenderer>().material.color;
		gameObject.GetComponent<MeshRenderer>().material.color = c;
	}


	void Update() 
	{
		
	}
}
