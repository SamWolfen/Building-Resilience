using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ValidateLogin : NetworkBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	public void OnClick ()
	{
	  Debug.Log ("Clicked");
          Debug.Log ("on: " + Network.isServer + " - " + Network.isClient);

          NetworkManager.singleton.ServerChangeScene ("PlayArea");
// 	  Application.LoadLevel ("PlayArea");
// 	  Vector3 spawnPosition = new Vector3 (0, 0, 0);
//           Quaternion spawnRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
//         
// 	  GameObject worldManager = (GameObject) Instantiate (worldManagerPrefab, spawnPosition, spawnRotation);
// 	  NetworkServer.Spawn (worldManager);
        }
}
