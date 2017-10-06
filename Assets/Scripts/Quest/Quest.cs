using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Quest
{
    
    public enum QProgress {UNAVAILABLE, AVAILABLE, ONGOING, COMPLETE, DONE };

    public string Title; //Quest Title
    public int Qid; //Quest ID
    public QProgress progress; //Current Quest state
    public string desc; //Quest description
    public int nxtQuest; //in case the quest is a chain quest

    public string task; //Quest task
    public int taskCount; //Current number of task
    public int taskReq; //Required amount of objects for tasks

    public string Qreward; //Quest reward

}
