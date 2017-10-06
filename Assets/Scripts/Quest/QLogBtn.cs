using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QLogBtn : MonoBehaviour {

	public int questID;
	public Text questTitle;

	public void DisplayInfos()
	{
		QuestManager.qManager.displayQuestLog (questID);



	}

	public void declineQuest()
	{
		QuestManager.qManager.DeclineQ (questID);
		QuestUI.uiManager.hideQuestLogPanel ();

		QuestObject[] currentQNPC = FindObjectsOfType (typeof(QuestObject)) as QuestObject[];

		foreach (QuestObject obj in currentQNPC) {
			obj.setQMarker ();
		}

	}

	public void closePanel()
	{
		QuestUI.uiManager.hideQuestLogPanel ();
	}

}
