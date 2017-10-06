using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestObject : MonoBehaviour {

    private bool inTrigger = false;

	public bool displayq = false;


    public List<int> availableIDs = new List<int>();
    public List<int> receivableIDs = new List<int>();


    public GameObject qMarker;
    public Image qImage;

    public Sprite qAvailableSprite;
    public Sprite qReceivableSprite;

	// Use this for initialization
	void Start () {
        setQMarker();
	}
	
	//set marker on top of npc
	public void setQMarker()
    {
        if(QuestManager.qManager.CheckCompleteQ(this))
        {
            qMarker.SetActive(true);
            qImage.sprite = qReceivableSprite;
            qImage.color = Color.yellow;
        }
        else if(QuestManager.qManager.CheckAvailableQ(this))
        {
            qMarker.SetActive(true);
            qImage.sprite = qAvailableSprite;
            qImage.color = Color.yellow;
        }
        else if (QuestManager.qManager.CheckAcceptedQ(this))
        {
            qMarker.SetActive(true);
            qImage.sprite = qReceivableSprite;
            qImage.color = Color.gray;
        }
        else
        {
            qMarker.SetActive(false);
        }
    }


	// Update is called once per frame
	void Update () {
		if(inTrigger == true && Input.GetKeyDown(KeyCode.R)&& QuestUI.uiManager.activeQuestLog==false)
        {
			if (!QuestUI.uiManager.activePanel) {


				//open UI
				QuestUI.uiManager.checkQuest (this);


				QuestManager.qManager.AddQItem("Talk to the other guy", 1);
				QuestManager.qManager.AddQItem("Return", 1);

			} else {
				
				QuestUI.uiManager.panelHide ();
				QuestUI.uiManager.acceptBtn.SetActive(false);
				QuestUI.uiManager.giveupBtn.SetActive(false);
				QuestUI.uiManager.completeBtn.SetActive(false);
			}
          

        }
	}

    //NPC have rigidbody and 2 colliders. 1 collider(non trigger) and the other collider(trigger)
    //Player must have rigidbody, collider(non trigger) and a tag called "Player"
    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            inTrigger = true;
            
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            inTrigger = false;
            
        }
    }

}
