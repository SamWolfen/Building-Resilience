// Not currently used.


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BlobNetworkManager : NetworkManager {
    
    public GameObject worldManagerPrefab;
    public GameObject stormCloud;
    public GameObject localLevelPrefab;
    
    public override void OnStartServer ()
    {
        base.OnStartServer ();
        
        Debug.Log ("Blob server ready");
        
        Vector3 spawnPosition = new Vector3 (10, 0, 0);
        Quaternion spawnRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        
        GameObject worldManager = (GameObject) Instantiate (worldManagerPrefab, spawnPosition, spawnRotation);
        
        DontDestroyOnLoad (worldManager);
        worldManager.GetComponent<WorldManager>().OnStartServer ();
    }
    
    public override void OnStartClient (NetworkClient client)
    {
        base.OnStartClient (client);
        ClientScene.RegisterPrefab(stormCloud);
        Debug.Log ("Blob client ready");
    }
    
    
    public override void OnClientSceneChanged (NetworkConnection conn)
    {
        base.OnClientSceneChanged (conn);
        
        Debug.Log ("Blob client scene change to: " + networkSceneName);
        
        if (networkSceneName  == "PlayArea")
        {
            Vector3 spawnPosition = new Vector3 (0, 0, 0);
            Quaternion spawnRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
            
            var localLevel = (GameObject) Instantiate (localLevelPrefab, spawnPosition, spawnRotation);
            localLevel.GetComponent<LocalWorld>().OnStartClient ();
            
            ClientScene.AddPlayer (conn, 0);
        }
    }
    
    //     public override void OnServerConnect(NetworkConnection conn)
    //     {
    //         base.OnServerConnect (conn);
    //         
    //         Debug.Log ("Client connected");
    //     }
}
