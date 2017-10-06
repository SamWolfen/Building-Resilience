using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// A temporary class intended to randomly scatter resource producers over the map. Eventually
/// resource producers will be integrated into the level structure.
public class Sheepmaker : NetworkBehaviour {

	public GameObject Sheep;

	// Use this for initialization
	public override void OnStartServer () {



		Vector3[] posSheep = new[] {
			new Vector3 (16.44f, 2.54f, 9.01f), 
			new Vector3 (29.8f, 10.58f, 36.57f), 
			new Vector3 (26.91f, 5.49f, 66.88f),
			new Vector3 (48.9f, 3.5f, 68.08f),
			new Vector3 (60.0f, 5.49f, 89.22f)
		};



		//pos [3] = 
		//pos [4] = new Vector3 (2, 3, 8);  
		//pos [5] = new Vector3 (2, 3, 8);




		Debug.Log ("Starting resource producers");
		for (int i = 0; i <= 5; i++)
		{

			GameObject rpInstance = UnityEngine.Object.Instantiate (Sheep, posSheep[i], Quaternion.identity);
			rpInstance.GetComponent<ResourceProducer>().resourceType = (int) Random.Range (0f, 4f);
			NetworkServer.Spawn (rpInstance);


		}

	}
}

