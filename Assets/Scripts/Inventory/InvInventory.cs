using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class InvInventory1 : MonoBehaviour {

    public List<InvItem> Inventory = new List<InvItem>();
    public GUISkin skin;
    public int SlotsX, SlotsY;
    public List<InvItem> Slots = new List<InvItem>();
    private bool showInventory;
    private InvItemDatabase database;
    private bool ShowTooltip;
    private string tooltip;

    private bool draggingItem; // Whether or not an item should be dragged
    private InvItem DraggedItem; //Current item being dragged
    private int PreviousIndex;

    bool setUIToFalse = true;

    //GameObject Player;
    PlayerMove pmove;
	PlayerState pstate;


	void Start ()
    {
        for (int i = 0; i < (SlotsX * SlotsY);i++)
        {
            Slots.Add(new InvItem());
            Inventory.Add(new InvItem());
        }

        database = GameObject.FindGameObjectWithTag("ItemDatabase").GetComponent<InvItemDatabase>();

        // Inventory.Add(database.items[0]);
        //Inventory.Add(database.items[1]);

        // Inventory[0] = database.items[0];
        // Inventory[1] = database.items[1];
        Additem(1);
        Additem(0);
        Additem(0);
        Additem(2);
        print(InventoryContains(3));

        //Player = this.transform.parent.gameObject;

    }

    void Awake()
    {
		pmove = this.GetComponent<PlayerMove>();

    }
	
    void OnGUI()
    {
        tooltip = "";
        GUI.skin = skin;
        if (showInventory == true)
        {
            DrawInventory();
              if (ShowTooltip == true) 
              {
                     GUI.Box(new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y,200,200), tooltip, skin.GetStyle("Tooltip"));
              } 
        }
        if (draggingItem)
        {
            GUI.DrawTexture(new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, 50, 50), DraggedItem.itemIcon);
        }
      
        
    }

    void Update()
    {
        if (Input.GetButtonDown("Inventory"))
            {
            
            showInventory = !showInventory;
            if (draggingItem)
            {
                Inventory[PreviousIndex] = DraggedItem;
                draggingItem = false;
                DraggedItem = null;
            }
            }
        if (showInventory == true) 
        {
            setUIToFalse = false;
            pmove.SetUIOpenTrue();
            print("UI TRUE");
        }
        if (showInventory == false && setUIToFalse == false)
        {
            setUIToFalse = true;
            pmove.SetUIOpenFalse();
            print("UI FALSE");
        }
        
    }

    void DrawInventory()
    {

        Event e = Event.current;


		//pstate = this.GetComponent<PlayerState>();
		//GameObject Player = pmove.gameObject;
		//pstate = Player.GetComponent<PlayerState>();
		pstate = gameObject.GetComponent<PlayerState>();
		pstate.resourceLevels.ToString();
		//Debug.LogError ("pstate: " + pstate.gameObject);
        for (int j = 0; j < GloopResources.NumberOfResources; j++)
        {
       		
	        Rect ResourceFrame = new Rect(Screen.width - 325f,(100f * j),100,100);
			float tempResourceLevel;
			tempResourceLevel = pstate.getResource(j + 2);
			tempResourceLevel = tempResourceLevel * 100;
			GUI.Box(ResourceFrame, tempResourceLevel.ToString("F0"), skin.GetStyle("Slot Background"));
        }
        
        int i = 0;
        for (int y = 0; y < SlotsY ;y++)
        {
            for (int x = 0; x < SlotsX; x++)
            {
                Rect slotRect = new Rect(Screen.width - (x * 55),  (y * 55), 50, 50);
                GUI.Box(slotRect, "", skin.GetStyle("Slot Background"));
                Slots[i] = Inventory[i];
               

                if (Slots[i].itemName != null)
                {
                    GUI.DrawTexture(slotRect,Slots[i].itemIcon);

                    if (slotRect.Contains(e.mousePosition))
                    {
                        if (e.isMouse && e.type == EventType.mouseDown && e.button == 1)
                        {
                            if (Slots[i].itemType == InvItem.ItemType.Consumable)
                            {
                                UseConsumable(Slots[i], i, true);
                            }
                        }
                        if (draggingItem == false)
                        {
                        tooltip = CreateTooltip(Slots[i]);
                        ShowTooltip = true;
                        }
                        
                        if (e.button == 0 && e.type == EventType.MouseDown && !draggingItem)
                        {
                            draggingItem = true;
                            PreviousIndex = i;
                            DraggedItem = Slots[i];
                            Inventory[i] = new InvItem();
                        }
                        else if (e.button == 0 && e.type == EventType.MouseDown && draggingItem == true)
                        {
                            Inventory[PreviousIndex] = Inventory[i];
                            Inventory[i] = DraggedItem;
                            draggingItem = false;
                            DraggedItem = null;
                        }
                    }
                }
                else
                {
                    if (slotRect.Contains(e.mousePosition))
                        {
                        if (e.type == EventType.MouseDown && draggingItem)
                            {
                            Inventory[i] = DraggedItem;
                            draggingItem = false;
                            DraggedItem = null;
                        }
                         }
                }
                if (tooltip == "")
                {
                    ShowTooltip = false;
                }
               
                i++;
                

            }
        }
    }

    string CreateTooltip(InvItem item)
    {
        tooltip = "";
        tooltip =  "<color=#ffffff>" + item.itemName + "</color>\n\n" + "<color=#ffffff>" + item.itemDesc + "</color>";

        return tooltip;

    }
    void UseConsumable(InvItem item, int Slot, bool RemoveItem)
    {
        switch (item.itemID)
        {
            case 2:
                {
                    print("USED CONSUMABLE: "+ item.itemName);
                    break;
                  //Player.Buff(STAT ID, BUFF AMOUNT, BUFF DURATION);
                }

       
        }
     if (RemoveItem == true)
                {
                    Inventory[Slot] = new InvItem();
                }

    }
    void Additem(int id)
    {
        for (int i = 0; i < Inventory.Count; i++)
        {
            if (Inventory[i].itemName == null)
            {
               for(int j = 0; j < database.items.Count; j++)
                {
                    if (database.items[j].itemID == id)
                    {
                        Inventory[i] = database.items[j];
                    }
                }
                break;
            }
        }
    }

    void RemoveItem(int id)
    {
        for (int i = 0; i < Inventory.Count;i++)
        {
            if (Inventory[i].itemID == id)
            {
                Inventory[i] = new InvItem();
                break;
            }
        }
    }
    
    bool InventoryContains (int id)
    {
        bool result = false;
        for (int i = 0; i < Inventory.Count; i++)
        {
            if (Inventory[i].itemID == id)
            {
                result = true;
                break;
            }
        }
        return result;
    }
	// Update is called once per frame
	
}
