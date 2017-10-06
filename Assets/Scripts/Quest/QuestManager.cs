using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour {

    public static QuestManager qManager;

    public List<Quest> qList = new List<Quest>(); //Master quest list
    public List<Quest> currentQlist = new List<Quest>(); //Current questlist

    void Awake()
    {
        if(qManager == null)
        {
            qManager = this;
        }
        else if (qManager != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

	//request quest function
    public void RequestQ(QuestObject NPC)
    {
        //Check Available Quest
        if(NPC.availableIDs.Count > 0)
        {
            for(int i = 0; i < qList.Count; i++)
            {
                for(int j = 0; j < NPC.availableIDs.Count; j++)
                {
                    if(qList[i].Qid == NPC.availableIDs[j] && qList[i].progress == Quest.QProgress.AVAILABLE)
                    {
                        Debug.Log("Quest ID: " + NPC.availableIDs[j] + " " + qList[i].progress);

                        //test
                        //AcceptQ(NPC.availableIDs[j]);

                        //quest UI
                        QuestUI.uiManager.availableQuest = true;
                        QuestUI.uiManager.questAvailables.Add(qList[i]);
                    }
                }
            }
        }

        //Check Active Quest
        for (int i = 0; i < currentQlist.Count; i++)
        {
            for (int j = 0; j < NPC.receivableIDs.Count; j++)
            {
				if (currentQlist[i].Qid == NPC.receivableIDs[j] && (currentQlist[i].progress == Quest.QProgress.ONGOING || currentQlist[i].progress == Quest.QProgress.COMPLETE))
                {
                    Debug.Log("Quest ID: " + NPC.receivableIDs[j] + " is " + currentQlist[i].progress);

                    //test
                    //CompleteQ(NPC.receivableIDs[j]);

                    //quest UI
                    QuestUI.uiManager.ongoingQuest = true;
                    QuestUI.uiManager.questActives.Add(currentQlist[i]);
                }
            }
        }


    }


    //Accept Quest function
    public void AcceptQ(int questID)
    {
        for (int i = 0; i < qList.Count; i++)
        {
            if (qList[i].Qid == questID && qList[i].progress == Quest.QProgress.AVAILABLE)
            {
                currentQlist.Add(qList[i]);
                qList[i].progress = Quest.QProgress.ONGOING;
            }
        }
    }


    //Quest Decline/ Give up function
    public void DeclineQ(int questID)
    {
        for (int i = 0; i < currentQlist.Count; i++)
        {
            if (currentQlist[i].Qid == questID && currentQlist[i].progress == Quest.QProgress.ONGOING)
            {                
                currentQlist[i].progress = Quest.QProgress.AVAILABLE;
                currentQlist[i].taskCount = 0;
                currentQlist.Remove(currentQlist[i]);
            }
        }
    }

    //Quest Complete Function
    public void CompleteQ(int questID)
    {
        for (int i = 0; i < currentQlist.Count; i++)
        {
            if (currentQlist[i].Qid == questID && currentQlist[i].progress == Quest.QProgress.COMPLETE)
            {
                currentQlist[i].progress = Quest.QProgress.DONE;              
                currentQlist.Remove(currentQlist[i]);

                //reward
            }
        }
        CheckChainQ(questID);
    }

    //Check chain quest
    void CheckChainQ(int questID)
    {
        int temp = 0;
        for(int i = 0; i < qList.Count; i++)
        {
            if(qList[i].Qid == questID && qList[i].nxtQuest > 0)
            {
                temp = qList[i].nxtQuest;
            }
        }

        if(temp > 0)
        {
            for(int i = 0; i < qList.Count; i++)
            {
                if(qList[i].Qid == temp && qList[i].progress == Quest.QProgress.UNAVAILABLE)
                {
                    qList[i].progress = Quest.QProgress.AVAILABLE;
                }
            }
        }




    }



    //Add task count  functiom
    public void AddQItem(string qObj, int itemAmount)
    {
        for(int i = 0; i < currentQlist.Count; i++)
        {
            if(currentQlist[i].task == qObj && currentQlist[i].progress == Quest.QProgress.ONGOING)
            {
                currentQlist[i].taskCount += itemAmount;
            }

            if (currentQlist[i].taskCount >= currentQlist[i].taskReq && currentQlist[i].progress == Quest.QProgress.ONGOING)
            {
                currentQlist[i].progress = Quest.QProgress.COMPLETE;
            }    
        }
     
    }




    //bool functions to handle available quest state
    public bool requestAvailableQ(int questID)
    {
        for(int i = 0; i < qList.Count; i++)
        {
            if(qList[i].Qid == questID && qList[i].progress == Quest.QProgress.AVAILABLE)
            {
                return true;
            }
        }
        return false;
    }

	//bool functions to handle ongoing quest state
    public bool requestOngoingQ(int questID)
    {
        for (int i = 0; i < qList.Count; i++)
        {
            if (qList[i].Qid == questID && qList[i].progress == Quest.QProgress.ONGOING)
            {
                return true;
            }
        }
        return false;
    }

	//bool functions to handle complete quest state
    public bool requestCompleteQ(int questID)
    {
        for (int i = 0; i < qList.Count; i++)
        {
            if (qList[i].Qid == questID && qList[i].progress == Quest.QProgress.COMPLETE)
            {
                return true;
            }
        }
        return false;
    }

	//bool function for checking available quests
    public bool CheckAvailableQ(QuestObject NPC)
    {
        for(int i = 0; i < qList.Count; i++)
        {
            for(int j = 0; j < NPC.availableIDs.Count; j++)
            {
                if(qList[i].Qid == NPC.availableIDs[j] && qList[i].progress == Quest.QProgress.AVAILABLE)
                {
                    return true;
                }
            }          
        }
        return false;
    }
	//bool function for checking ongoing quests
    public bool CheckAcceptedQ(QuestObject NPC)
    {
        for (int i = 0; i < qList.Count; i++)
        {
            for (int j = 0; j < NPC.receivableIDs.Count; j++)
            {
                if (qList[i].Qid == NPC.receivableIDs[j] && qList[i].progress == Quest.QProgress.ONGOING)
                {
                    return true;
                }
            }
        }
        return false;
    }

	//bool function for checking completed quests
    public bool CheckCompleteQ(QuestObject NPC)
    {
        for (int i = 0; i < qList.Count; i++)
        {
            for (int j = 0; j < NPC.receivableIDs.Count; j++)
            {
                if (qList[i].Qid == NPC.receivableIDs[j] && qList[i].progress == Quest.QProgress.COMPLETE)
                {
                    return true;
                }
            }
        }
        return false;
    }

	//display current quests in quest log
	public void displayQuestLog(int questID)
	{
		for(int i = 0; i < currentQlist.Count; i++)
		{
			if(currentQlist[i].Qid == questID)
			{
				QuestUI.uiManager.DisplayQLog (currentQlist[i]);
			}
		}
	}









}
