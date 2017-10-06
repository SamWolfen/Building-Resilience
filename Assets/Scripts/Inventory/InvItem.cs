using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class InvItem  {
    public string itemName;
    public int itemID;
    public string itemDesc;
    public Texture2D itemIcon;
    public int itemAmount;
    public ItemType itemType;


    public enum ItemType
    {
    Block,
    Consumable,
    QuestItem,
    Flag
    }
    public InvItem(string name, int id,string desc,int amount, ItemType type)
    {
        itemName = name;
        itemID = id;
        itemDesc = desc;
        itemAmount = amount;
        itemType = type;
        itemIcon = Resources.Load<Texture2D>("Item Icons/" +  name);
    }
    public InvItem()
    {

    }
}
