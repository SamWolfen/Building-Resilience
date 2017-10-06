using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class StormGenerator : NetworkBehaviour
{

    public GameObject stormCloud;

    Vector3 CloudPosition;//= new Vector3(18, 5, 7);
    Quaternion CloudRotation= Quaternion.identity;
    void GenerateCloud(Vector3 Position, Vector3 Direction, int CloudLevel)
    {
        Instantiate(stormCloud);
        //StormCloud.AddComponent<StormCloud>();
    }
    public void GenerateCloud()
    {

        //Instantiate(StormCloud, CloudPosition, CloudRotation);
        SpawnObject();
    }

    public override void OnStartClient()
    {
        ClientScene.RegisterPrefab(stormCloud);
    }


    // Use this for initialization
    void Start() {
       InvokeRepeating("SpawnObjectRand",1,10);
    }

    // Update is called once per frame
    void Update()
    {

        
        if (Input.GetButtonDown("Cloud"))
        {

            SpawnObjectRand();

        }
    }

    [Server]
    public void SpawnObject()
    {
        CloudPosition = new Vector3(18, 5, 7);
        GameObject obj = (GameObject)Instantiate(stormCloud, CloudPosition, CloudRotation);
        NetworkServer.Spawn(obj);
    }

    public void SpawnObjectRand()
    {
        Vector3 Cposition = new Vector3(Random.Range(-10.0f, 100.0f),40, Random.Range(-10.0f, 100.0f));
        GameObject obj = (GameObject)Instantiate(stormCloud, Cposition, CloudRotation);
        NetworkServer.Spawn(obj);
    }
}