using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class FogScript : NetworkBehaviour {
  public  int Timerforfog;
     bool Disabled;
    public ParticleSystem FogParticle;
	// Use this for initialization
	void Start () {

        print("Fog started");
        DontDestroyOnLoad(this.gameObject);
              FogParticle = GetComponent<ParticleSystem>();
                InvokeRepeating("AddOne", 1, 1);
    }
	
	// Update is called once per frame
	void Update ()
    {
        //DontDestroyOnLoad (this.gameObject);

        if (Timerforfog > 10)
        {
            EnableSystem();
        }
	}

 
    public void DisableSystem()
    {
       
        var Emittemp = FogParticle.emission;
        Emittemp.enabled = false;
        

    }
    public void EnableSystem()
    {
        //FogParticle.Play();
       
        var Emittemp = FogParticle.emission;
        Emittemp.enabled = true;
    }
    public void EnableinTime()
    {
        //FogParticle.Play();


        Invoke("EnableSystem",5);
    }
    public void AddOne()
    {
        Timerforfog++;
    }
}
