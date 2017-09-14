using UnityEngine;
using System.Collections;

public class NetworkPlayer : MonoBehaviour
{
	
	public Transform P2Body;
	public Transform player;
	public Transform phead;

	// Use this for initialization
	void Start ()
	{
		if (Network.isClient) {
			enabled = false;	
			
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		UpdateGhostPOSOnServer ();
	}
	
	void OnSerializeNetworkView (BitStream stream, NetworkMessageInfo info)
	{
		Vector3 syncPosition = Vector3.zero;
		if (stream.isWriting) {
			syncPosition = GetComponent<Rigidbody>().position;
			stream.Serialize (ref syncPosition);
		} else {
			stream.Serialize (ref syncPosition);
			P2Body.GetChild (0).position = syncPosition;
		}
	}

	[RPC]
	public void GhostMove (Vector3 body, Vector3 head)
	{
		Debug.Log ("Trying to move ghost");
		P2Body.GetChild (0).transform.position = body;
		P2Body.GetChild (1).transform.position = head;
		
	}
	
	public void UpdateGhostPOSOnServer ()
	{
		if (Network.isClient) {
			GetComponent<NetworkView>().RPC ("GhostMove", RPCMode.Server, player.transform.position, phead.transform.position);
		}
		
	}
	
}
