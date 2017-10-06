using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QBtn : MonoBehaviour {

    public int questID;
    public Text questTitle;


	//Display quest info when player press the quest button in the panel
	public void DisplayInfos()
	{
		QuestUI.uiManager.displayQuestInfo (questID);

		//accept button
		if (QuestManager.qManager.requestAvailableQ(questID)) {
			QuestUI.uiManager.acceptBtn.SetActive (true);
			QuestUI.uiManager.acceptScript.questID = questID;

		} 
		else 
		{
			QuestUI.uiManager.acceptBtn.SetActive (false);
		}

		//decline button
		if (QuestManager.qManager.requestOngoingQ(questID)) {
			QuestUI.uiManager.giveupBtn.SetActive (true);
			QuestUI.uiManager.declineScript.questID = questID;
		} 
		else 
		{
			QuestUI.uiManager.giveupBtn.SetActive (false);
		}

		//complete button
		if (QuestManager.qManager.requestCompleteQ(questID)) {
			QuestUI.uiManager.completeBtn.SetActive (true);
			QuestUI.uiManager.completeScript.questID = questID;
		} 
		else 
		{
			QuestUI.uiManager.completeBtn.SetActive (false);
		}

	}



	public void acceptQuest()
	{
		QuestManager.qManager.AcceptQ (questID);
		QuestUI.uiManager.panelHide ();

		QuestUI.uiManager.acceptBtn.SetActive(false);
		QuestUI.uiManager.giveupBtn.SetActive(false);
		QuestUI.uiManager.completeBtn.SetActive(false);

		QuestObject[] currentQNPC = FindObjectsOfType (typeof(QuestObject)) as QuestObject[];

		foreach (QuestObject obj in currentQNPC) {
			obj.setQMarker ();
		}

	}
		
	public void declineQuest()
	{
		QuestManager.qManager.DeclineQ (questID);
		QuestUI.uiManager.panelHide ();

		QuestUI.uiManager.acceptBtn.SetActive(false);
		QuestUI.uiManager.giveupBtn.SetActive(false);
		QuestUI.uiManager.completeBtn.SetActive(false);

		QuestObject[] currentQNPC = FindObjectsOfType (typeof(QuestObject)) as QuestObject[];

		foreach (QuestObject obj in currentQNPC) {
			obj.setQMarker ();
		}

	}

	public void completeQuest()
	{
		QuestManager.qManager.CompleteQ (questID);
		QuestUI.uiManager.panelHide ();

		QuestUI.uiManager.acceptBtn.SetActive(false);
		QuestUI.uiManager.giveupBtn.SetActive(false);
		QuestUI.uiManager.completeBtn.SetActive(false);

		QuestObject[] currentQNPC = FindObjectsOfType (typeof(QuestObject)) as QuestObject[];

		foreach (QuestObject obj in currentQNPC) {
			obj.setQMarker ();
		}

	}

	public void closePanel()
	{
		QuestUI.uiManager.panelHide ();
		QuestUI.uiManager.acceptBtn.SetActive(false);
		QuestUI.uiManager.giveupBtn.SetActive(false);
		QuestUI.uiManager.completeBtn.SetActive(false);
	}













}
