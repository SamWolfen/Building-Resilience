using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestUI : MonoBehaviour {

    public static QuestUI uiManager;

    //bool
    public bool availableQuest = false;
    public bool ongoingQuest = false;
    public bool activePanel = false;
    public bool activeQuestLog = false;

	public bool displayLog = true;

    public GameObject questPanel;
    public GameObject questLog;


    private QuestObject currentQuestObj;

    public List<Quest> questAvailables = new List<Quest>();
    public List<Quest> questActives = new List<Quest>();

    public GameObject questBtn;
    public GameObject questLogBtn;
    private List<GameObject> btns = new List<GameObject>();

	public GameObject acceptBtn;
	public GameObject giveupBtn;
    public GameObject completeBtn;

    public Transform qbtnHolder1;
    public Transform qbtnHolder2;
    public Transform qLogbtnHolder;

    //quest info
    public Text qTitle;
    public Text qDesc;
	public Text qTask;

    //quest log
    public Text qlTitle;
    public Text qlDesc;
	public Text qlTask;

	public QBtn acceptScript;
	public QBtn declineScript;
	public QBtn completeScript;


	void Start()
	{
		acceptBtn = GameObject.Find ("QuestCanvas").transform.Find ("QuestPanel").transform.Find ("Description").transform.Find ("GameObject").transform.Find ("Accept").gameObject;
		acceptScript = acceptBtn.GetComponent<QBtn> ();

		giveupBtn = GameObject.Find ("QuestCanvas").transform.Find ("QuestPanel").transform.Find ("Description").transform.Find ("GameObject").transform.Find ("Decline").gameObject;
		declineScript = giveupBtn.GetComponent<QBtn> ();

		completeBtn = GameObject.Find ("QuestCanvas").transform.Find ("QuestPanel").transform.Find ("Description").transform.Find ("GameObject").transform.Find ("Complete").gameObject;
		completeScript = completeBtn.GetComponent<QBtn> ();


		acceptBtn.SetActive (false);
		giveupBtn.SetActive (false);
		completeBtn.SetActive (false);


	}


    void Awake()
    {
        if(uiManager == null)
        {
            uiManager = this;
        }
        else if(uiManager != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);

		panelHide();
    } 

	
	// Update is called once per frame
	void Update ()
    {
		if(Input.GetKeyDown(KeyCode.Q)&&activePanel==false)
        {
			
				activeQuestLog = !activeQuestLog;
				DisplayQLogPanel ();
				displayLog = !displayLog;

        }
			


	}

	//function for checking quests in quest panel
    public void checkQuest(QuestObject questObject)
    {
        currentQuestObj = questObject;
        QuestManager.qManager.RequestQ(questObject);

        if((ongoingQuest || availableQuest) && !activePanel)
        {
            //display questPanel
            DisplayPanel();
        }
        else
        {
            Debug.Log("No available Quest");
        }
    }

	//Display quest log panel description and tasks
	public void DisplayQLog(Quest OngoingQuest)
	{
		qlTitle.text = OngoingQuest.Title;

		if (OngoingQuest.progress == Quest.QProgress.ONGOING) {
			qlDesc.text = OngoingQuest.desc;
			qlTask.text = OngoingQuest.task + " : " + OngoingQuest.taskCount + " / " + OngoingQuest.taskReq;

		} else if (OngoingQuest.progress == Quest.QProgress.COMPLETE) {
			qlDesc.text = OngoingQuest.desc;
			qlTask.text = OngoingQuest.task + " : " + OngoingQuest.taskCount + " / " + OngoingQuest.taskReq;

		}


	}



	//display Quest panel
    public void DisplayPanel()
    {
        activePanel = true;
        questPanel.SetActive(activePanel);

		createQuestBtns();
    }

	//function on instantiating the quest button and its information
	public void DisplayQLogPanel()
	{
		questLog.SetActive (activeQuestLog);
		if (activeQuestLog && !activePanel) {
			foreach (Quest currentQuest in QuestManager.qManager.currentQlist) {
				GameObject questbutton = Instantiate (questLogBtn);
				QLogBtn qbutton = questbutton.GetComponent<QLogBtn> ();

				qbutton.questID = currentQuest.Qid;
				qbutton.questTitle.text = currentQuest.Title;

				questbutton.transform.SetParent (qLogbtnHolder, false);
				btns.Add (questbutton);
			}
		} else if (!activeQuestLog && !activePanel) {
			hideQuestLogPanel ();
		}
	}



	//Hide Panel
	public void panelHide()
	{
		activePanel = false;
		availableQuest = false;
		ongoingQuest = false;

		qTitle.text = "";
		qDesc.text = "";
		qTask.text = "";

		questAvailables.Clear();
		questActives.Clear(); 

		for (int i = 0; i < btns.Count; i++) 
		{
			Destroy(btns[i]);	
		}
		btns.Clear();

		questPanel.SetActive(activePanel);
	}

	//hide quest log panel
	public void hideQuestLogPanel()
	{
		activeQuestLog = false;

		qlTitle.text = "";
		qlDesc.text = "";
		qlTask.text = "";

		for (int i = 0; i < btns.Count; i++) {
			Destroy (btns [i]);	
		}
		btns.Clear ();
		questLog.SetActive (activeQuestLog);
	}


    //fill buttons for panels
    void createQuestBtns()
    {
        foreach(Quest questAvailable in questAvailables)
        {
			GameObject questButton = Instantiate(questBtn);
			QBtn qBScript = questButton.GetComponent<QBtn>();

			qBScript.questID = questAvailable.Qid;
			qBScript.questTitle.text = questAvailable.Title;

			questButton.transform.SetParent (qbtnHolder1, false);
			btns.Add (questButton);

        }

		foreach(Quest questOngoing in questActives)
		{
			GameObject questButton = Instantiate(questBtn);
			QBtn qBScript = questButton.GetComponent<QBtn>();

			qBScript.questID = questOngoing.Qid;
			qBScript.questTitle.text = questOngoing.Title;

			questButton.transform.SetParent (qbtnHolder2, false);
			btns.Add (questButton);

		}
    }



	//display quest title, description and tasks
	public void displayQuestInfo(int questID)
	{

		for (int i = 0; i < questAvailables.Count; i++) 
		{
			if (questAvailables[i].Qid == questID) 
			{
				qTitle.text = questAvailables[i].Title;

				if (questAvailables[i].progress == Quest.QProgress.AVAILABLE) 
				{
					qDesc.text = questAvailables[i].desc;
					qTask.text = questAvailables[i].task + " : " + questAvailables [i].taskCount + " / " + questAvailables [i].taskReq;

				}					
			}
		}

		for(int i = 0; i < questActives.Count;i++)
		{
			if (questActives[i].Qid == questID) 
			{
				qTitle.text = questActives[i].Title;

				if (questActives [i].progress == Quest.QProgress.ONGOING) {
					qDesc.text = questActives [i].desc;
					qTask.text = questActives [i].task + " : " + questActives [i].taskCount + " / " + questActives [i].taskReq;

				} 
				else if (questActives [i].progress == Quest.QProgress.COMPLETE) 
				{
					qDesc.text = questActives [i].desc;
					qTask.text = questActives [i].task + " : " + questActives [i].taskCount + " / " + questActives [i].taskReq;
				}
			}
		}
	}











}

