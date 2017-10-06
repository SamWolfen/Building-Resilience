using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// A temporary class intended to randomly scatter resource producers over the map. Eventually
/// resource producers will be integrated into the level structure.
public class ResourceProducerScatterer : NetworkBehaviour {
    
    public GameObject resourceProducer;
    
    // Use this for initialization
    public override void OnStartServer () {
        Debug.Log ("Starting resource producers");
        /*for (int i = 0; i < 100; i++)
        {
            Vector3 pos = new Vector3 ((int) Random.Range (0f, 100.0f), WorldManager.minLevelHeight + 2.0f, (int) Random.Range (0f, 100.0f));
            GameObject rpInstance = UnityEngine.Object.Instantiate (resourceProducer, pos, Quaternion.identity);
            rpInstance.GetComponent<ResourceProducer>().resourceType = (int) Random.Range (0f, 4f);
            NetworkServer.Spawn (rpInstance);
        }*/
    }
 }
