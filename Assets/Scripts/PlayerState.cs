using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class PlayerState : NetworkBehaviour {
    
    private const float barSize = 0.2f;

//     [SyncVar(hook = "OnChangeResourceLevels")]
	public List<float> resourceLevels;
    [SyncVar(hook = "OnChangeResources")]
    private bool resourceChanged;
    
    /// The prefab for components used to display resource levels.
    public GameObject resourceDisplayElement;
    
    /// The actual objects used as part of the bar displaying level of resources.
    private GameObject [] resourceDisplayObjects = null;
    
	private bool inTrigger;

	private string ResourceName;

	private Vector3 ResourcePosition;

	PlayerMove pmove;

	//GameObject playerObject;

    // Use this for initialization
    void Start () {
        resourceLevels.Add (0.1f);
        resourceLevels.Add (0.1f);
        resourceLevels.Add (0.1f);
        
        OnChangeResourceLevels (resourceLevels);

		inTrigger = false;
		ResourceName = "";
		ResourcePosition = new Vector3 ();

		pmove = gameObject.GetComponent<PlayerMove>();
    }

	void Update()
	{
		// Delete eventually as this is now done in PlayerMove
		//if (inTrigger == true && Input.GetKeyDown (KeyCode.G))
		//{
			//takeResource ();
		//}
	}

	// Update is called once per frame
	/*void Update () {
		if (inTrigger == true && Input.GetKeyDown (KeyCode.G))
		{
			//Debug.LogError ("In resource trigger range!!!!!!!!!!!!!!!!!!!!!!");
			if (ResourceName == "WoodResourceBrick(Clone)")
			{
				changeResource (0, 0.05f);
				ResourceTakeMessage m = new ResourceTakeMessage ();
				m.position = ResourcePosition;
				NetworkManager.singleton.client.Send (LevelMsgType.ResourceUpdate, m);  
			}
		}
	}*/
    
	public void changeResource (int resourceType, float deltaResource)
    {
        //if (!isServer)
        //{
           // return;
       // }

		resourceLevels [resourceType] += deltaResource;
		pmove.resourceLevelsPMove [resourceType] += deltaResource;
        
        if (resourceLevels[resourceType] > 1.0f)
        {
            resourceLevels[resourceType] = 1.0f;
			pmove.resourceLevelsPMove [resourceType] = 1.0f;
        }
        
        resourceChanged = !resourceChanged;
        Debug.Log ("Changed " + this);
    }

    void OnChangeResources (bool changed)
    {
                Debug.Log ("Refreshfix");
        OnChangeResourceLevels (resourceLevels);
    }

//       void OnGUI() {
//           Debug.Log ("Updating resource");
//           GUI.DrawTexture(new Rect(10, 10, 60, 60), aTexture, ScaleMode.ScaleToFit, true, 10.0F);
//       }
	void OnChangeResourceLevels (List<float> resourceLevels )
    {
                Debug.Log ("Refresh");
        //if (!isLocalPlayer)
        //{
          //  return;
        //}
                
        // Initialize objects for the resource bar.
       /* if (resourceDisplayObjects == null)
        {
            /// The object under which resource display elements are shown.
            GameObject resourceAreaDisplay = GameObject.Find("ResourceAreaDisplay");
            GameObject gameGlobals = GameObject.Find("GameGlobals");

            resourceDisplayObjects = new GameObject [GloopResources.NumberOfResources];
            for (int i = 0; i < GloopResources.NumberOfResources; i++)
            {
              GameObject go = UnityEngine.Object.Instantiate (resourceDisplayElement, new Vector3 (0, 0, 0), Quaternion.identity);
              resourceDisplayObjects[i] = go;
              resourceDisplayObjects[i].transform.SetParent (resourceAreaDisplay.transform, false);
              resourceDisplayObjects[i].GetComponent<MeshRenderer>().material = gameGlobals.GetComponent<GloopResources>().resourceMaterials[i];
            }
        }
        
        // Translate resource levels into size and position of the resource bar.
        float position = -0.75f;
        for (int i = 0; i < GloopResources.NumberOfResources; i++)
        {
            resourceDisplayObjects[i].transform.localPosition = new Vector3 (position, 0.75f, 0.0f);
            float amt = barSize * resourceLevels[i];
            resourceDisplayObjects[i].transform.localScale = new Vector3 (amt, barSize, barSize);
            
            position += barSize;
        }*/
    }

	public void takeResource()
	{
		if (inTrigger == true && ResourceName == "WoodResourceBrick(Clone)")
		{
            QuestManager.qManager.AddQItem("Harvest a block", 1);
            //Debug.LogError ("In resource trigger range!!!!!!!!!!!!!!!!!!!!!!");
            changeResource (0, 0.05f);
			OnChangeResources (resourceChanged);
			//pmove = gameObject.GetComponent<PlayerMove> ();
			inTrigger = false;
			ResourceTakeMessage m = new ResourceTakeMessage ();
			m.position = ResourcePosition;
			m.amount = -1;
			NetworkManager.singleton.client.Send (LevelMsgType.ResourceUpdate, m);  
		}
		if (inTrigger == true && ResourceName == "DirtResourceBrick(Clone)")
		{
            QuestManager.qManager.AddQItem("Harvest a block", 1);
            //Debug.LogError ("In resource trigger range!!!!!!!!!!!!!!!!!!!!!!");
            changeResource (1, 0.05f);
			OnChangeResources (resourceChanged);
			//pmove = gameObject.GetComponent<PlayerMove> ();
			inTrigger = false;
			ResourceTakeMessage m = new ResourceTakeMessage ();
			m.position = ResourcePosition;
			m.amount = -1;
			NetworkManager.singleton.client.Send (LevelMsgType.ResourceUpdate, m);  
		}
		if (inTrigger == true && ResourceName == "CrystalResourceBrick(Clone)")
		{
            QuestManager.qManager.AddQItem("Harvest a block", 1);
            //Debug.LogError ("In resource trigger range!!!!!!!!!!!!!!!!!!!!!!");
            changeResource (2, 0.05f);
			OnChangeResources (resourceChanged);
			//pmove = gameObject.GetComponent<PlayerMove> ();
			inTrigger = false;
			ResourceTakeMessage m = new ResourceTakeMessage ();
			m.position = ResourcePosition;
			m.amount = -1;
			NetworkManager.singleton.client.Send (LevelMsgType.ResourceUpdate, m);  
		}
	}

	public void expendResorce(int type, float amount)
	{
		if (type == 2) {
			changeResource (0, amount);
			OnChangeResources (resourceChanged);
		}
		if (type == 3) {
			changeResource (1, amount);
			OnChangeResources (resourceChanged);
		}
		if (type == 4) {
			changeResource (2, amount);
			OnChangeResources (resourceChanged);
		}
	}

	public float getResource(int type)
	{
		if (type == 2) {
			return resourceLevels [0];
		}
		if (type == 3) {
			return resourceLevels [1];
		}
		if (type == 4) {
			return resourceLevels [2];
		}
		return 0.0f;
	}

	void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.tag == "Resource")
		{
			inTrigger = true;
			ResourceName = other.name;
			//ResourcePosition = other.gameObject.transform.position;
			ResourcePosition = other.transform.position;
			//playerObject = other.transform.parent.gameObject;
		}
	}

	void OnTriggerStay(Collider other)
	{
		if(other.gameObject.tag == "Resource")
		{
			inTrigger = true;
			ResourceName = other.name;
			//ResourcePosition = other.gameObject.transform.position;
			ResourcePosition = other.transform.position;
			//playerObject = other.transform.parent.gameObject;
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (other.tag == "Resource")
		{
			inTrigger = false;
			ResourceName = "";
			ResourcePosition = new Vector3();
		}
	}
}
