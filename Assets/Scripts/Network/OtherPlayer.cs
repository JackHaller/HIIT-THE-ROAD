using UnityEngine;
using System.Collections;


public class OtherPlayer : MonoBehaviour {
	
	public Transform P2Body, P2Head;
	
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
		
	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
{
    Vector3 syncPosition = Vector3.zero;
    if (stream.isWriting)
    {
        syncPosition = GetComponent<Rigidbody>().position;
        stream.Serialize(ref syncPosition);
    }
    else
    {
        stream.Serialize(ref syncPosition);
        P2Body.GetComponent<Rigidbody>().position = syncPosition;
    }
}
}
