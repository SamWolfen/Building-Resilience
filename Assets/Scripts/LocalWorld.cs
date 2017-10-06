using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// A message sent by client to server advising of player position.
class LevelSyncMessage : MessageBase
{
    /// Position in space of the player.
    public Vector3 playerPosition;
    
    /// The distance that the player should be able to see. The server
    /// will register the player's interest in neighbouring region blocks
    /// as defined by this.
    public float   visibleRadius;
}

/// The message sent when the player is updating the level. 
class BlockAddMessage : MessageBase
{
    public float px;
    public float pz;
    
    public float height;

	public int blocktype;
}

/// Message sent to the server when resources are taken from a resource brick.
class ResourceTakeMessage : MessageBase
{
	public Vector3 position;
	public int amount;
}

/// Message for a player sending a single emote to the server.
/*class SendEmoteMessage : MessageBase
{
	public int emoteType;
}*/

class SendEmoteMessageAndClientID : MessageBase
{
	public int emoteType;
	public NetworkInstanceId netId;
}

class PlayerFlagMessage : MessageBase
{
	/// Position of the flag object.
	public Vector3 position;
	/// Connection ID for the player placing or removing the flag.
	public int connid;
	/// Return to client as false if the flag cannot be removed by that player.
	public bool removed;
}

/*class PlayerListMessage : MessageBase
{
	public int connectionId;
	//public ClientDetails cd;
}*/


/// A local level block represents a component of the environment
/// in which the player exists. The complete environment will consist
/// of several of these blocks, and any other environmentally global
/// components.
/// This class allow interrogation of the local world structure, as 
/// recently updated from the master copy on the server. It also matches
/// this to GameObjects in the scene holding geometry and other attributes.
public class LocalLevelBlock
{
    public RegionBlock region;
    public GameObject  gobject;
	//public int blocktype;
}

/// A client equivalent of the Level Structure and World Managers. Cached
/// copies of the regionblocks that the client can use for navigating about
/// thw world.
public class LocalWorld : NetworkBehaviour {

    /// The game object that will be the base object for each
    /// local level block. Presumably an empty.
    public GameObject localLevelElement;
    
    /// The game object used to represent a default grass/dirt brick.
	public GameObject localBrick;
	/// The game object used to represent a timber brick.
	public GameObject localTimberBrick;
	/// The game object used to represent a brick brick.
	public GameObject localBrickBrick;
	/// The game object used to represent a platinum brick.
	public GameObject localPlatinumBrick;
	/// The game object used to represent a wood resource brick.
	public GameObject localWoodResourceBrick;
	/// The game object used to represent a clay resource brick.
	public GameObject localClayResourceBrick;
	/// The game object used to represent a platinum resource brick.
	public GameObject localPlatinumResourceBrick;
    
    /// Define the distance about the player that we
    /// are interested in seeing things.
    public float viewRadius;

	//declare new emotedisplayclass for displaying incoming emotes.
	public static EmoteDisplayClass emoteDisplayer;
    
    /// Local level cache.
    private List<LocalLevelBlock> levelStructure; 
    
    /// The class will attempt to register with the player object. Once
    /// this has been achieved then this value will be set to true.
    private bool foundPlayer;
    
    // Use this for initialization
    void Start () {
        levelStructure = new List<LocalLevelBlock> ();
        
        foundPlayer = false;
        
        Debug.Log ("Local world level instance started");

		emoteDisplayer = new EmoteDisplayClass();

//        ClientScene.AddPlayer (0);
    }
    
    /// Identify the level block corresponding to a particular position.
    /// May return null if no such block is cached locally.
    public LocalLevelBlock findLevelBlock (Vector3 position)
    {
        foreach (LocalLevelBlock i in levelStructure)
        {
            if (i.region.contains (position.x, position.z))
            {
                return i;
            }
        }
        return null;
    }
    
    /// Return the value of the cell at the given position. Returns
    /// false if no cell exists within the local cache.
    public bool findBlock (Vector3 position, out int value)
    {
        LocalLevelBlock llb = findLevelBlock (position);
        if (llb != null)
        {
          int rx = (int) (position.x - llb.region.blockCoordX);
          int ry = (int) (position.z - llb.region.blockCoordY);
          int rz = (int) (position.y);
          
          value = llb.region.getBlock (rx, ry, rz);
          return true; 
        }
        value = 0;
        return false;
    }
    
    /// Create a level block corresponding to the given region block and
    /// add this to the cache of blocks. Assumes that such a block does
    /// not already exist, or a duplicate will be created.
    public LocalLevelBlock addLevelBlock (RegionBlock rb, Vector3 position)
    {
//         Debug.Log ("New local block " + position);
        LocalLevelBlock llb = new LocalLevelBlock ();
        llb.region = rb;
        llb.gobject = UnityEngine.Object.Instantiate (localLevelElement, position, Quaternion.identity);
        levelStructure.Add (llb);
        return llb;
    }                    
    
    /// Remove any cached region blocks that are outside the visible region.
    private void flushRegions ()
    {
        NetworkManager nm = NetworkManager.singleton;
        if (nm.client != null)
        {
            PlayerController player = ClientScene.localPlayers[0];
            
            Vector3 playerPosition = player.gameObject.transform.position;
            
            for (int i = levelStructure.Count - 1; i >= 0; i--)
            {
                if (!levelStructure[i].region.contains (playerPosition.x, playerPosition.z, viewRadius))
                {
                    UnityEngine.Object.Destroy (levelStructure[i].gobject);
                    levelStructure.RemoveAt (i);
                }
            }
        }
        
    }
    
    /// This object is deployed on the clients. When it starts:
    ///   - Register a network message handler for any level updates.
    public override void OnStartClient()
    {
        Debug.Log ("On Client start " + NetworkClient.allClients);
                
        NetworkClient.allClients[0].RegisterHandler (LevelMsgType.LevelResponse, ServerCommandHandler);
		NetworkClient.allClients[0].RegisterHandler (LevelMsgType.EmoteSingleSender, ServerCommandHandler); // Handle incoming emotes from server
		NetworkClient.allClients[0].RegisterHandler (LevelMsgType.PlayerFlagRequest, ServerCommandHandler);
    }
    
    // Update is called once per frame
    void Update () {
        // Check if the player has been created, and associate with that player if so.
        if (!foundPlayer)
        {
            if (ClientScene.localPlayers.Count > 0)
            {
                PlayerMove player = ClientScene.localPlayers[0].gameObject.GetComponent <PlayerMove>();
                player.setLocalWorld (this);
                foundPlayer = true;
            }
        }
        
        // Send player position to the server.
        NetworkManager nm = NetworkManager.singleton;
        if ((nm.client != null) && (foundPlayer))
        {
            if (ClientScene.localPlayers.Count > 0)
            {
                PlayerController player = ClientScene.localPlayers[0];
                
                LevelSyncMessage m = new LevelSyncMessage ();
                m.playerPosition = player.gameObject.transform.position;
                m.visibleRadius = viewRadius;
                
                NetworkManager.singleton.client.Send (LevelMsgType.LevelRequest, m);
            }
        }
    }
    
    /// Handle incoming updates from the server.
    void ServerCommandHandler (NetworkMessage netMsg)
    {
        switch (netMsg.msgType)
        {
            case LevelMsgType.LevelResponse:
                // Received an updated region block from the server. Update
                // the cache, and ensure that the local visual representation
                // is consistent.
            {
                RegionBlock rb = netMsg.ReadMessage<RegionBlock>();
//                 Debug.Log ("Server Got message: " + rb.blockSize);
                
                MeshFilter mf = GetComponent <MeshFilter>();
                rb.convertToMesh (mf.mesh);        
                
                Vector2 rbpos = rb.getPosition ();
                Vector3 llbpos = new Vector3 (rbpos.x, WorldManager.minLevelHeight, rbpos.y);
                LocalLevelBlock llb = findLevelBlock (llbpos);
                if (llb == null)
                {
                    llb = addLevelBlock (rb, llbpos);
                    
                    // llb should now be valid.
					llb.region.placeBlocks (llb.gobject.transform);
                }
                else
                {
                    // if version is newer than the one we already have, then update it.
                    if (rb.timeLastChanged > llb.region.timeLastChanged)
                    {
                        llb.region = rb;
//                         Debug.Log ("Got update ..................................>");
						llb.region.placeBlocks (llb.gobject.transform);
                    }
                }
                
                flushRegions ();
            }
            break;

			case LevelMsgType.PlayerList:
			/// Message containing list of active players.
			{
				
			}
			break;

			case LevelMsgType.EmoteSingleSender:
				/// Handle incoming emotes from server
			{
				SendEmoteMessageAndClientID m = netMsg.ReadMessage<SendEmoteMessageAndClientID> ();
				displayEmote(m.emoteType, m.netId);
				Debug.Log ("Incoming emote to client from server from network id: " + m.netId);
			}
			break;

			case LevelMsgType.PlayerFlagRequest:
			{
				PlayerFlagMessage m = netMsg.ReadMessage<PlayerFlagMessage> ();
				if (m.removed == false)
				{
					//Do UI message to player here informing them of failure to remove flag.
				}
				if (m.removed == true)
				{
					PlayerMove player = ClientScene.localPlayers[0].gameObject.GetComponent <PlayerMove>();
					player.playerFlagPlaced = false;
				}
			}
			break;
            
            default:
            {
                Debug.Log ("Unexpected message type in LocalWorld");                
            }
            break;
        }
    }
    
	public void displayEmote(int emoteType, NetworkInstanceId netId)
	{
		emoteDisplayer.displayEmote (emoteType, netId);
	}

	// Add a new block at the given position. The intent is to allow players to immediately
    // reflect actions in the local game, which may eventually be replaced when the update
    // is returned from the server.
	public void placeBlock (float x, float z, float height, int type)
    {
        // Coordinates for the region block - only horizontal elements.
        Vector3 llbpos = new Vector3 (x, 0.0f, z);
        LocalLevelBlock llb = findLevelBlock (llbpos);
        
        int blockHeight = (int) (height - WorldManager.minLevelHeight);
        if (llb != null)
        {
            Vector3 regionpos = new Vector3 (x - llb.region.blockCoordX, z - llb.region.blockCoordY, blockHeight);
            // llb should now be valid.
            // Make a local copy before signalling the change to the world.
            //llb.region.placeSingleBlock (localBrick, regionpos, llb.gobject.transform);
        }
        else
            // else no region - potential problem.
        {
            Debug.Log ("No level block at " + llbpos);
        }
        
        
        Debug.Log ("Add at " + x + " " + z + "    " + blockHeight);
        BlockAddMessage m = new BlockAddMessage ();
        m.px = x;
        m.pz = z;
        m.height = blockHeight;
		m.blocktype = type;
        NetworkManager.singleton.client.Send (LevelMsgType.LevelUpdate, m);        
    }
    
    /// Check the neighbouring blocks to see if there is an attachment candidate available.
    /// If so, return the height of that neighbour. 
    private bool validNearestBlockCandidate (int x, int z, int height, out int y)
    {
        
        y = 0;
        
        /// Include diagonal neighbours.
        //int [] offx = { -1, -1 , -1, 0, 1, 1, 1, 0 };
        //int [] offz = {  1, 0 , -1, -1, -1, 0, 1, 1 };
        
        /// Only 4 neighbours that share a side.
        int [] offx = {  -1 , 0, 1, 0 };
        int [] offz = {  0 , -1, 0, 1 };
        
        int numberNeighbours = offx.Length;
        for (int i = 0; i < numberNeighbours; i++)
        {
          int value;
          bool hasValue = findBlock (new Vector3 (x + offx[i], height, z + offz[i]), out value);
			if (hasValue && (value >= 1))
          {
//              y = (int) WorldManager.minLevelHeight;
              y = height;
              return true;
          }
        }
        return false;
    }
    
    /// Find the open position next to a block nearest the given position. 
    public bool findNearestBlock (Vector3 position, out Vector3 availablePosition)
    {
        int r = 0;
        // search in increasing radius about the player.
        while (r < viewRadius)
        {
            // scan in a ring of the given radius r.
            for (int offset = 0; offset <= r; offset++)
            {
                int cx;
                int cz;
                int cy;
                
                // search coordinates for a square ring.
                int [] offxfixed    = { -1, -1, +1, +1,  0,  0,  0,  0  };
                int [] offxvariable = {  0,  0,  0,  0,  -1, +1, -1, +1  };
                int [] offzfixed    = {  0,  0,  0,  0,  -1, -1, +1, +1 };
                int [] offzvariable = { -1, +1, -1, +1,   0,  0,  0,  0 };
                for (int i = 0; i < 8; i++)
                {
                    cx = offxfixed[i] * r + offxvariable[i] * offset + (int) (position.x + 0.5f);
                    cz = offzfixed[i] * r + offzvariable[i] * offset + (int) (position.z + 0.5f);
                    int height = 0;
                    
                    /// Check if the current block is an appropriate candidate. We assume this is empty
                    /// since it would have been previously checked as being a potential neighbour 
                    /// candidate. So just check to see if this has a valid neighbour, and get the height 
                    /// of the neighbouring block if it exists.
                    if (validNearestBlockCandidate (cx, cz, height, out cy))
                    {
                        availablePosition = new Vector3 (cx, cy + WorldManager.minLevelHeight, cz);
                        return true;
                    }
                }
            }
            r++;
        }
        // No block found.
        availablePosition = new Vector3 (0.0f, 0.0f, 0.0f);
        return false;
    }

	/// Method for use by EmoteWheel.cs to send an emote to the server.
	/*public static void sendEmote(int emote)
	{
		SendEmoteMessage m = new SendEmoteMessage ();
		m.emoteType = emote;
		NetworkManager.singleton.client.Send (LevelMsgType.EmoteSingleReceiver, m);
	}*/
}

public class EmoteDisplayClass : ScriptableObject
{
	public List<EmoteButton> buttons;
	private Vector2 Mouseposition;
	private Vector2 fromVector2M;
	private Vector2 centercirlce;
	private Vector2 toVector2M;
	public Sprite sprite1;
	public Sprite sprite2;
	public Sprite sprite3;
	public Sprite sprite4;
	public float EmoteLifetime;
	private bool menuon;
	public int menuItems;

	public void Awake()
	{
		sprite1 = Resources.Load<Sprite>("Sprites/Emotes/HappySprite");
		sprite2 = Resources.Load<Sprite>("Sprites/Emotes/SadSprite");
		sprite3 = Resources.Load<Sprite>("Sprites/Emotes/HelloSprite");
		sprite4 = Resources.Load<Sprite>("Sprites/Emotes/EmergencySprite");
		EmoteLifetime = 2;
		centercirlce = new Vector2(0.5f, 0.5f);
		fromVector2M = new Vector2(0.5f, 1.0f);
		buttons = new List<EmoteButton>();
	}

	public void displayEmote(int emoteType, NetworkInstanceId netId)
	{
		GameObject Player = ClientScene.FindLocalObject(netId);
		//int InstanceID = Player.GetComponent (NetworkInstanceId);
		//string netID = Player.GetComponent<NetworkIdentity>().netId.ToString();
		Debug.Log ("Player network ID Received by client: " + Player.GetComponent<NetworkIdentity>().netId.ToString());
		Vector3 offset = new Vector3(0.0f, 2.5f, 0.0f);

		if (emoteType == 0)
		{
			GameObject emoteobject = new GameObject("Emote");
			SpriteRenderer renderer = emoteobject.AddComponent<SpriteRenderer>();
			IconObject qwe = emoteobject.AddComponent<IconObject>();
			qwe.PlayerNetID (netId);
			renderer.sprite = sprite1;
			Destroy(emoteobject, EmoteLifetime);
		}
		else if (emoteType == 1)
		{
			GameObject emoteobject = new GameObject("Emote");
			SpriteRenderer renderer = emoteobject.AddComponent<SpriteRenderer>();
			IconObject qwe = emoteobject.AddComponent<IconObject>();
			qwe.PlayerNetID (netId);
			renderer.sprite = sprite2;
			Destroy(emoteobject, EmoteLifetime);
		}
		else if (emoteType == 2)
		{
			GameObject emoteobject = new GameObject("Emote");
			SpriteRenderer renderer = emoteobject.AddComponent<SpriteRenderer>();
			IconObject qwe = emoteobject.AddComponent<IconObject>();
			qwe.PlayerNetID (netId);
			renderer.sprite = sprite3;
			Destroy(emoteobject, EmoteLifetime);
		}
		else if (emoteType == 3)
		{
			GameObject emoteobject = new GameObject("Emote");
			SpriteRenderer renderer = emoteobject.AddComponent<SpriteRenderer>();
			IconObject qwe = emoteobject.AddComponent<IconObject>();
			qwe.PlayerNetID (netId);
			renderer.sprite = sprite4;
			Destroy(emoteobject, EmoteLifetime);
		}
	}
}