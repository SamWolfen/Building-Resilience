using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System;

public class PlayerAnimator : NetworkBehaviour {

	public Animator anim;
	public float speed = 2.0f;
	public float rotationSpeed = 75.0f;

	int timer = 0;

	// Use this for initialization
	void Start () {
		anim = this.GetComponent<Animator> ();
	}
	
	// Update is called once per frame
	void Update () {
		float translation = Input.GetAxis ("Vertical") * speed;
		float rotation = Input.GetAxis ("Horizontal") * rotationSpeed;
		translation *= Time.deltaTime;
		rotation *= Time.deltaTime;

		transform.Translate (0, 0, translation);
		transform.Rotate (0, rotation, 0);

		if (translation != 0) {
			anim.SetBool ("isWalking", true);
		} else {
			anim.SetBool ("isWalking", false);
		}

		if (Input.GetKey (KeyCode.G) || Input.GetKey (KeyCode.E)) {
			anim.SetBool ("isGathering", true);
			timer = 30;
		} else {
			timer -= 1;

			if (timer < 0) {
				anim.SetBool ("isGathering", false);
			}
		}

	}
}
