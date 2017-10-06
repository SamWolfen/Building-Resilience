using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System;

/// Player object management.
public class PlayerMove : NetworkBehaviour
{
    bool UIOpen = false;
	/// A link to the local level representation,which can be
	/// queried for any player control operations.
	protected LocalWorld localWorld;

	/// Manage when a block is attached to an edge of the world.
	protected bool attached;
	protected Vector3 attachPoint;

	bool pickup = false;

	// Current block type selected by the player to be placed.
	int currentBlockType = 1;

	/// Is the player flag placed in the world.
	public bool playerFlagPlaced = false;

	bool inTrigger = false;

	GameObject flagShortest;

	/// Timer for flag check
	int flagTimer = 0;
	bool checkFlagRangeLast = false;
	PlayerState playerState;

	public Animator anim;
	public float speed = 2.0f;
	public float rotationSpeed = 75.0f;

	int timer = 0;

	public List<float> resourceLevelsPMove;


	//INVINVENTORY------------------
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
	//INVINVENTORY--------------------------------------

	/// Access method used when the local world registers itself
	/// with this player object.
	public void setLocalWorld (LocalWorld lw)
	{
		localWorld = lw;
		//       Debug.Log ("Add block " + localWorld); 
	}

	/// Actions on each update of the player:
	///   - handle key presses
	///   - check that the player stays in a valid region.
	void Update()
	{
		flagTimer -= 1;
		DontDestroyOnLoad (this.gameObject);
		;

		Ray ray = Camera.main.ScreenPointToRay(new Vector3((Screen.width/2),(Screen.height/2),0));
		RaycastHit hit;

		//Debug.Log ("Player update - " + isLocalPlayer + " - " + localWorld);

		if (!isLocalPlayer)
		{
			return;

		}

		if (localWorld == null)
		{
			return;
		}

		/// Jump action.
		var y = 0.0f;
		if (Input.GetKeyDown(KeyCode.Space) && (isGrounded () || attached))
		{
			y = 1.0f;
			attached = false;
		}

		Vector3 playerpos = transform.position;

		//Debug.LogError("PLAYERMOVE RESOURCE CHECK: " + resourceLevelsPMove[0]);

		//Vector3 forward = transform.forward;
		//Vector3 placement = transform.position + forward * 2;

		/// Place block action by right mouse click


		//if (Input.GetKeyDown(KeyCode.Mouse1) && (isGrounded () || attached))
		//{


			/*int px = (int) (hit.point.x);
			int py = Math.Max ((int) (playerpos.y), (int) WorldManager.minLevelHeight);
			int pz = (int) (playerpos.z);

			Debug.Log ("place at " + px + " " + pz + " " + py);
			localWorld.placeBlock (px, pz, py);

			//y = 0.2f;
			//transform.Translate(0, 1, 0);

			attached = false;


			int px = (int) (placement.x + 10.5);
			int py = Math.Max ((int) (placement.y), (int) WorldManager.minLevelHeight);
			int pz = (int) (placement.z + 0.5);
 

            Debug.Log ("place at " + px + " " + pz + " " + py);
            localWorld.placeBlock (px, pz, py);

            y = 0.2f;
            transform.Translate(0, 1, 0);
		
			attached = false;*/


		/*	if(Physics.Raycast(ray, out hit, 10))
			{


				int px = (int) (placement.x + 0.5);
				int py = Math.Max ((int) (placement.y), (int) WorldManager.minLevelHeight);
				int pz = (int) (placement.z + 0.5);


				Debug.Log ("place at " + px + " " + pz + " " + py);
				localWorld.placeBlock (px, pz, py, currentBlockType);

				y = 0.2f;
				transform.Translate(0, 1, 0);

				attached = false;



			}

		} */

		float translation = Input.GetAxis ("Vertical") * speed;
		float rotation = Input.GetAxis ("Horizontal") * rotationSpeed;
		translation *= Time.deltaTime;
		rotation *= Time.deltaTime;

		transform.Translate (0, 0, translation);
		transform.Rotate (0, rotation, 0);

		if (translation != 0) {
			anim.SetBool ("isWalking", true);
		} else {
			anim.SetBool ("isWalking", false);
		}

		if (Input.GetKey (KeyCode.G) || Input.GetKey (KeyCode.E)) {
			anim.SetBool ("isGathering", true);
			timer = 30;
		} else {
			timer -= 1;

			if (timer < 0) {
				anim.SetBool ("isGathering", false);
			}
		}

		/// Place block action.
		if (Input.GetKeyDown(KeyCode.LeftControl) && (isGrounded () || attached))
		{
			if (playerFlagPlaced == false || currentBlockType != 10)
			{
			if(!checkFlagRange() && (checkResource() || currentBlockType == 10 || currentBlockType == 1))
				{
					int px = (int)(playerpos.x + 0.5f);
					int py = Math.Max ((int)(playerpos.y), (int)WorldManager.minLevelHeight);
					int pz = (int)(playerpos.z + 0.5f);

					Debug.Log ("place at " + px + " " + pz + " " + py);
					if (attached) {	
						localWorld.placeBlock (px, pz, py, currentBlockType);
					} else {
						localWorld.placeBlock (px, pz, py + 1.0f, currentBlockType);
					}
				
					//Quest Manager log
					QuestManager.qManager.AddQItem ("Place a block", 1);
					y = 0.2f;
					if (currentBlockType == 10) {
						playerFlagPlaced = true;
						transform.Translate (0, 3.0f, 0);
						//Quest Manager log
						QuestManager.qManager.AddQItem ("Place a flag", 1);
					} else {
						transform.Translate (0, 1.5f, 0);
					}
					adjustResource();
					attached = false;
				}
			}
		}

		/// Take Resources action.
		if (Input.GetKeyDown (KeyCode.G) && !checkFlagRange())
		{
			PlayerState pstate = gameObject.GetComponent<PlayerState> ();
			pstate.takeResource ();
			//Debug.LogError ("Taking Resources from playermove!!!");
		}


		/// Change the currentBlockType

		if (Input.GetKey (KeyCode.Alpha1))
		{
			currentBlockType = 1;
		}
		if (Input.GetKey (KeyCode.Alpha2))
		{
			currentBlockType = 2;
		}
		if (Input.GetKey (KeyCode.Alpha3))
		{
			currentBlockType = 3;
		}
		if (Input.GetKey (KeyCode.Alpha4))
		{
			currentBlockType = 4;
		}
		if (Input.GetKey (KeyCode.Alpha0))
		{
			currentBlockType = 10;
		}

		//Pick up 

		if(Input.GetKey(KeyCode.E) && playerFlagPlaced == true)
		{
			/*if(Physics.Raycast(ray, out hit, 50))
			{
				pickup = true;
			}
		}
		else
		{
			pickup = false;*/

			if (checkFlagRange())
			{
				removeFlag ();
			}
			
		}

		//open inventory window
		if(Input.GetKey(KeyCode.I))
		{


		}
		
		/// Turn and move forward.
		//var x = Input.GetAxis("Horizontal") * Time.deltaTime * 150.0f;
		//var z = Input.GetAxis("Vertical") * Time.deltaTime * 3.0f;

		var x = (Input.GetAxis("Mouse X") * 5.0f);
		/// Update player state based on actions set.
		/// 

		var z = Input.GetAxis("Vertical") * Time.deltaTime * 3.0f;
		var strafe = Input.GetAxis("Horizontal") * Time.deltaTime * 3.0f;
		var testY = Input.GetAxis("Mouse Y") * Time.deltaTime * 5.0f;
        
		/// Update player state based on actions set.
        /// 

        if (UIOpen == false)
        {
            transform.Rotate(0, x, 0);
        }
	
	transform.Translate(strafe, 0, z);

	Rigidbody rb = GetComponent<Rigidbody> ();
	rb.AddForce (80.0f * transform.up * y);

		/// Check that movement is allowed.
		playerpos = transform.position;
		if (playerpos.y < WorldManager.minLevelHeight - 0.5f/* && Input.GetKeyDown(KeyCode.RightControl)*/)
		{
			//           Debug.Log ("Fallen and can't get up: " + playerpos);

			// Find a block that the player can attach to.
			Vector3 freepos;
			bool found = localWorld.findNearestBlock (playerpos, out freepos);
			if (found)
			{
				Debug.Log  ("Finding neighbour for: " + playerpos + " at " + freepos);
				attachPoint  = freepos;
				attached = true;
			}
			else
			{
				// emergency - no blocks available.
			}
		}

		if (attached)
		{
			//transform.position = attachPoint;
			transform.position = Vector3.MoveTowards(transform.position, attachPoint, 1.0f); //attachPoint;
			rb.velocity = new Vector3 (0.0f, 0.0f, 0.0f);
		}

	//INVINVENTORY--------------------------------------------------------



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
		SetUIOpenTrue();
		print("UI TRUE");
	}
	if (showInventory == false && setUIToFalse == false)
	{
		setUIToFalse = true;
		SetUIOpenFalse();
		print("UI FALSE");
	}
	//INVINVENTORY--------------------------------------------------------




	}

//INVINVENTORY--------------------------------------------------------	
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
//INVINVENTORY--------------------------------------------------------

	/// Some actions are only allowed when the player is on the ground. This method
	/// checks for that case.
	bool isGrounded()
	{
		var distToGround = 0.4f;
		//         Debug.Log ("distance " + Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.1f) + " " + transform.position + " - " + (-Vector3.up) + " - " + (distToGround + 0.1f));
		return Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.1f);
	}

	/// When a client side instance of the player is started then:
	///   - update player view.
	///   - attach the main camera to give a external, tracked view of the player.
	public override void OnStartLocalPlayer()
	{
		GameObject playerShape = transform.Find("FemaleShape").gameObject;
		Debug.LogError (playerShape.name);
		//playerShape.GetComponent<MeshRenderer>().material.color = Color.blue; 
		playerState = gameObject.GetComponent<PlayerState> ();
		anim = this.GetComponent<Animator> ();
		resourceLevelsPMove.Add (0.1f);
		resourceLevelsPMove.Add (0.1f);
		resourceLevelsPMove.Add (0.1f);
		if(isLocalPlayer)
		{ //if I am the owner of this prefab
			GameObject camera = GameObject.Find("Main Camera");
			Debug.Log ("Setup camera" + camera);
			camera.transform.parent = transform;
		}

	//INVINVENTORY--------------------------------------
	for (int i = 0; i < (SlotsX * SlotsY);i++)
	{
		Slots.Add(new InvItem());
		Inventory.Add(new InvItem());
	}

	database = GameObject.FindGameObjectWithTag("ItemDatabase").GetComponent<InvItemDatabase>();

	Additem(1);
	Additem(0);
	Additem(0);
	Additem(2);
	print(InventoryContains(3));
	//INVINVENTORY--------------------------------------
	}

	public void removeFlag()
	{
		inTrigger = false;
		PlayerFlagMessage m = new PlayerFlagMessage ();
		m.position = flagShortest.transform.position;
		NetworkManager.singleton.client.Send (LevelMsgType.PlayerFlagRequest, m);
	}

	// Returns true if a flag is within range.
	public bool checkFlagRange()
	{
		if (flagTimer < 1)
		{
			GameObject[] flagList = GameObject.FindGameObjectsWithTag ("Flag");
			float distanceShortest = 1000;
			foreach (GameObject flag in flagList) {
				float distance = Vector3.Distance (flag.transform.position, this.transform.position);
				if (distance < distanceShortest) {
					distanceShortest = distance;
					flagShortest = flag;
				}
			}
			if (distanceShortest < 3.5f)
			{
				checkFlagRangeLast = true;
				return true;
				flagTimer = 30;
			}
			flagTimer = 30;	
			checkFlagRangeLast = false;
			return false;
		}
		else
		{
			return checkFlagRangeLast;
		}
	}
	
	public void adjustResource()
	{
		playerState.expendResorce (currentBlockType, -0.1f);
	}

	public bool checkResource()
	{
		if(playerState.getResource (currentBlockType) >= 0.1f)
		{
			return true;
		}
		return false;
	}

	//when ray collide with blocks
	void OnCollisionEnter(Collision other)
	{
		if(pickup == true)
		{
			if(other.gameObject.CompareTag("Block"))
			{
                other.gameObject.SetActive(false);
			}
		}
	}

    public void SetUIOpenTrue()
    {
        UIOpen = true;
    }

    public void SetUIOpenFalse()
    {
        UIOpen = false;
    }





	//INVINVENTORY STUFF-----------------------------------------------------------------



void DrawInventory()
{

	Event e = Event.current;


	//pstate = this.GetComponent<PlayerState>();
	//GameObject Player = pmove.gameObject;
	//pstate = Player.GetComponent<PlayerState>();
	//pstate = gameObject.GetComponent<PlayerState>();
	//pstate.resourceLevels.ToString();
	//Debug.LogError ("pstate: " + pstate.gameObject);
	for (int j = 0; j < GloopResources.NumberOfResources; j++)
	{

		Rect ResourceFrame = new Rect(Screen.width - 325f,(100f * j),100,100);
		float tempResourceLevel;
		tempResourceLevel = resourceLevelsPMove[j];
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

}