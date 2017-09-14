using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour
{
	private const string typeName = "ExerGameAKUni";
	private const string gameName = "TestRoom";
	public Transform player;
	public Transform p2;
	public bool enableMplayer;

	// Use this for initialization
	void Start ()
	{
		//MasterServer.ipAddress = "127.0.0.1";
		if (enableMplayer)
			p2.gameObject.GetComponent<Renderer>().enabled = false;
		if (enableMplayer)
			enabled = false;
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
 
	private void StartServer ()
	{
		Network.InitializeServer (2, 25000, !Network.HavePublicAddress ()); // Limit to 2 players at the moment.
		MasterServer.RegisterHost (typeName, gameName);
		
	}
	
	void OnServerInitialized ()
	{
		Debug.Log ("Server Initializied");
		//SpawnPlayer();
	}
	
	void OnGUI ()
	{
		if (!Network.isClient && !Network.isServer) {
			if (GUI.Button (new Rect (100, 100, 250, 100), "Start Server"))
				StartServer ();
 
			if (GUI.Button (new Rect (100, 250, 250, 100), "Refresh Hosts"))
				RefreshHostList ();
 
			if (hostList != null) {
				for (int i = 0; i < hostList.Length; i++) {
					if (GUI.Button (new Rect (400, 100 + (110 * i), 300, 100), hostList [i].gameName))
						JoinServer (hostList [i]);
				}
			}
		}
	}
	
	private HostData[] hostList;
 
	private void RefreshHostList ()
	{
		MasterServer.RequestHostList (typeName);
	}
 
	void OnMasterServerEvent (MasterServerEvent msEvent)
	{
		if (msEvent == MasterServerEvent.HostListReceived)
			hostList = MasterServer.PollHostList ();
	}
	
	private void JoinServer (HostData hostData)
	{
		Network.Connect (hostData);
	}
 
	void OnConnectedToServer ()
	{
		Debug.Log ("Server Joined");
	}
	
	public GameObject playerPrefab;
  
	private void SpawnPlayer ()
	{
		//player.GetComponent<NetworkPlayer>().P2Body = (Transform)(Network.Instantiate(playerPrefab, new Vector3(0f, 5f, 0f), Quaternion.identity, 0));
		
	}
	
}
