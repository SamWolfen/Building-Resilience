using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvItemDatabase : MonoBehaviour {
    public List<InvItem> items = new List<InvItem>();

 void Awake()
    {
        items.Add(new InvItem("RedBlock",0,"A crimson red block", 2,InvItem.ItemType.Block));
        items.Add(new InvItem("GreenBlock", 1, "A green block", 2, InvItem.ItemType.Block));
        items.Add(new InvItem("Buffpotion", 2, "This potion buffs you", 1, InvItem.ItemType.Consumable));
    }

    void Start()
    {

    }
}
