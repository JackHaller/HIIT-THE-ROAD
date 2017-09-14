using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Exergame;

public class TrainerController : MonoBehaviour {

	public Transform Icon;

	private PlayerController player;

	private Block nextBlock = null;	//When pathing, this stores the next block that it needs to navigate through
									//thus when the current block is a branch entrance, this should be set to the Block corresponding to the chosen side to path down

	// Use this for initialization
	void Start () {
	
		//Find the radar controller and register with it
		Transform instantiatedIcon = (Transform)Instantiate (Icon, Vector3.zero, Quaternion.identity);
		GameObject.Find ("Radar").GetComponent<RadarController> ().RegisterObjectForTracking (this.transform, instantiatedIcon);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	//Fixed update is called once per physics tick
	void FixedUpdate() {
		if (player.GetGameState () == GameState.GAME_STATE_WAIT) { // Game is in wait state. Don't wanna start playback too early. 
			return;
		} else if (player.GetGameState () == GameState.GAME_STATE_PLAY) { //Game running.
			//move along the current navigation path
		} else {
			return; //Gameover/Exit state. Do nothing.
			
		}
	}

	//generates a path through the target block, taking into consideration the requirements of the block after it
	IList<Vector3> NavigateBlock(Vector3 entryPoint, Block blockToNavigate, Block followingBlock) {
		//we're assuming that in the case of a twinned block, this method is being called with the
		//subblocks rather than the overall twinned block
		//that is, when it is called with a branch entrance, it will pick out a side to target
		return null;
	}
}
