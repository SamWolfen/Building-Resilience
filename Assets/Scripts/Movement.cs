using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Movement : MonoBehaviour {
	public GameObject Sheep;
	Vector3 pos;




	float pauto = 0f;

	float sinPauto;
	float cosPauto;



	// Use this for initialization
	void Start () {
		pos = Sheep.transform.position;

		
	}
	
	// Update is called once per frame
	void Update () {

		//DontDestroyOnLoad (this.gameObject);


		pauto = pauto + 0.01f;
		sinPauto = Mathf.Sin (pauto)/50;
		cosPauto = Mathf.Cos (pauto)/50;


		pos = new Vector3 (pos.x + sinPauto, pos.y, pos.z + cosPauto);
		Sheep.transform.position = pos;
		Sheep.transform.rotation = Quaternion.LookRotation (new Vector3 (-sinPauto, 0.0f, -cosPauto));


	}
}
