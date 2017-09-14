using UnityEngine;
using System.Collections;

public class Destroy : MonoBehaviour {

	private GameObject _player;
	
	void Start () {
		_player = GameObject.Find("Player");
	}
	
	// Update is called once per frame
	void Update () {
		
		float currentXOfPlayer = _player.transform.position.x;
		
		float xDistance = gameObject.transform.position.x - currentXOfPlayer;

		if(xDistance < (-60)){
			Destroy(gameObject);
		}
		
	}
}
