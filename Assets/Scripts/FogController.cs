using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class FogController : MonoBehaviour {

    GameObject[] Players;
   public GameObject[] FogBoxes;
    public GameObject FogBox;
    Quaternion FogBoxRotation = Quaternion.identity;
    // Use this for initialization
    void Start ()
    {
        //if (isServer)
        {
          CmdCreateFogGrid(); 
        }

        print("Got fogboxes pre ");

        FogBoxes = GameObject.FindGameObjectsWithTag("Fog");
        
        print("Got fogboxes " + FogBoxes.Length );

    }

    // Update is called once per frame
    void Update ()
    {
       

        Players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in Players)
        {
            foreach (GameObject fogb in FogBoxes)
            {
                float distance = Vector3.Distance(player.transform.position, fogb.transform.position);
              //  print("the distance is ::: "+ distance);
                if (distance < 15)
                {
                    fogb.GetComponent<FogScript>().DisableSystem();
                    // fogb.GetComponent<FogScript>().EnableinTime();
                    fogb.GetComponent<FogScript>().Timerforfog = 0;

                }

            }
        }
	}

    //[Command]
    void CmdCreateFogGrid()
    {
        for(int x = 0; x < 100; x = x + 20)
        {
            for (int y = 0; y  < 100; y = y + 20)
            {
                SpawnFog(x,y);
                //GameObject obj = (GameObject)Instantiate(FogBox, new Vector3(x, -5, y), FogBoxRotation);
                // NetworkServer.Spawn(obj);
            }
        }
    }
    

    
    public void SpawnFog(int Fogx, int Fogy)
    {
        print("Fog spawned");
        GameObject obj = (GameObject)Instantiate(FogBox, new Vector3(Fogx,5,Fogy), FogBoxRotation);
        NetworkServer.Spawn(obj);
    }

}
