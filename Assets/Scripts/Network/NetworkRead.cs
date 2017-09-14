using UnityEngine;
using System.Collections;

public class NetworkRead : MonoBehaviour {
	
	public Transform P2;
	
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
        syncPosition = P2.GetChild(0).position;
        stream.Serialize(ref syncPosition);
    }
    else
    {
        stream.Serialize(ref syncPosition);
        P2.GetChild(0).GetComponent<Rigidbody>().position = syncPosition;
    }
}
}
