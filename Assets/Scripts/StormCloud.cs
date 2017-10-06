using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
public class StormCloud : NetworkBehaviour
{
   // [SyncVar]
    public float CloudX = 1;
    public float CloudY = 1;
    public float CloudZ = 1;
    private float CloudDuration = 7.0f;
    public Vector3 Direcion = new Vector3(1,1,1);
    public float Speed = 1;
    public int CloudLevel;
    GameObject CloudObject;
    ParticleSystem Rainparticle;
    
   

	// Use this for initialization
	void Start ()
    {
        InvokeRepeating("DestroyStuff", 0.1f, 2f);
        Direcion = new Vector3(Random.Range(-5.0f, 5.0f), 0, Random.Range(-5.0f, 5.0f));
    }

    void Awake()
    {
       // Rainparticle = gameObject.particleSystem;
        if (CloudDuration > 0)
        {
            Destroy(this.gameObject, CloudDuration);
        }
        Rainparticle = GetComponent<ParticleSystem>();
        var shapetemp = Rainparticle.shape;

        //shapetemp.scale = new Vector3(1,1,1);
        //shapetemp.scale.Set(1,1,1);
        transform.localScale = new Vector3(Random.Range(10.0f, 20.0f), Random.Range(3.0f, 5.0f), Random.Range(10.0f, 20.0f));
    
 
        InvokeRepeating("DestroyStuff", 0.0f, 1f);
    }

   public void DestroyStuff()
    {

        if (!isServer)
        {
            return;
        }

        // Instantiate(, CloudPosition, CloudRotation);

        //Get the area of the cloud
        //print(this.transform.position);
        Vector3 LightningOrigin = this.transform.position;

        //Generate the direction for the raycast and make sure its inbounds with the cloud object
        float Xspawnpos = Random.Range(LightningOrigin.x - (CloudX / 2), LightningOrigin.x + (CloudX / 2));
        float Yspawnpos = LightningOrigin.y - 1;
        float Zspawnpos = Random.Range(LightningOrigin.z - (CloudX / 2), LightningOrigin.z + (CloudX / 2));

        LightningOrigin = new Vector3(Xspawnpos,Yspawnpos,Zspawnpos);

        RaycastHit ObjectHit;

        if (Physics.Raycast(LightningOrigin, -Vector3.up, out ObjectHit, 100))
        {
            //print("Found an object - distance: " + ObjectHit.distance);
          //  print("Found an object - Name: " + ObjectHit.collider.name);
         //   print("Found an object - Tag: " + ObjectHit.collider.tag);
            if(ObjectHit.collider.tag ==  "Block" || ObjectHit.collider.tag == "Resource")
            {
                // Destroy(ObjectHit.collider.gameObject.gameObject);
                
                 //ObjectHit.collider.gameObject.
                 //Destroy(ObjectHit.collider.gameObject);

                ResourceTakeMessage m = new ResourceTakeMessage();
                m.position = ObjectHit.collider.transform.position;
                m.amount = -2;
                NetworkManager.singleton.client.Send(LevelMsgType.ResourceUpdate, m);

                print("Object: Destoryed");
            }
        }
        

    }



	// Update is called once per frame
	void Update () {

        //transform.localScale = new Vector3(CloudX,CloudY,CloudZ);
        //DestroyStuff();
      transform.Translate(Direcion * Speed * Time.deltaTime);
       
    }
}
