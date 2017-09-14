using UnityEngine;
using System.Collections;
using AssemblyCSharp;
using System;
using System.IO;
using Exergame;

public class GhostController : MonoBehaviour
{
	public string PlaybackData;
	public PlaybackManager manager;
	public PlaybackInformation ghostInfo;
	private PlayerReader reader;
	public bool normalmode, pacingmode, competitivemode; // Game modes. See PlaybackManager.
	public bool correctErrors, warnOfErrors, warnThisBlock;
	public bool stayInFront;
	public Transform ghostbody, ghosthead, ghostBike;
	public PlayerController player;
	private bool Alive;
	private ArrayList buffer;
	//private static readonly int GAME_STATE_WAIT = 0;
	//private static readonly int GAME_STATE_PLAY = 1;
	//private static readonly int GAME_STATE_OVER = 2;
	//private static readonly int GAME_STATE_EXIT = 3;		//ensures nothing more gets done once we exit
	
	public float bufferThreshold;
	public Color OriginalColor;

	public Transform Icon;
	
	//Warning trigger.
	private bool colourtrig;
	//Flashing timespan
	private float flashDelay = 0.05f, delayDelta = 0f;
	
	//PacingModeVars;
	public float playbackRate;
	
	//Score;
	public float score;
	
	// Use this for initialization
	void Start ()
	{
		Alive = true;
		reader = new PlayerReader (PlaybackData);
		ghostInfo = manager.GetComponent<PlaybackManager> ().GetGhostInfo (Path.GetFileName (PlaybackData)); 
		Debug.Log ("Data from file: " + PlaybackData + " Time recorded: " + ghostInfo.recordTime.ToString () + " username: " + ghostInfo.username
			+ " age: " + ghostInfo.age + " bmi" + ghostInfo.BMI + " Assumed Fitness: " + ghostInfo.assumedFitness
			
			);
		buffer = new ArrayList ();
		bufferThreshold = 10f;
		OriginalColor = ghostbody.GetComponent<Renderer>().material.GetColor ("_Color");
		warnThisBlock = false;
		playbackRate = 1f;

		//Find the radar controller and register with it
		Transform instantiatedIcon = (Transform)Instantiate (Icon, Vector3.zero, Quaternion.identity);
		GameObject.Find ("Radar").GetComponent<RadarController> ().RegisterObjectForTracking (this.transform, instantiatedIcon);
	}
	
	//Update is called once per frame
	// visual effects here.

	void Update ()
	{
		if (warnThisBlock && delayDelta >= flashDelay) {
			delayDelta = 0f;
			if (colourtrig) {
				ChangeColorToBlack ();
				colourtrig = !colourtrig;
			
			} else {
				ResetColor ();
				colourtrig = !colourtrig;
			}
		} else {
			delayDelta += Time.deltaTime;
		}
		SetTransparency();
	}
	
	// FixedUpdate is called once per physics simulation
	void FixedUpdate ()
	{
		if (player.GetGameState () == GameState.GAME_STATE_WAIT) { // Game is in wait state. Don't wanna start playback too early. 
			return;
		} else if (player.GetGameState () == GameState.GAME_STATE_PLAY && Alive) { //Game running.
			if (normalmode)
				NormalPlayback ();
			if (pacingmode)
				PacingPlayback ();
			if (competitivemode)
				NormalPlayback (); // No difference between the two.
		} else {
			return; //Gameover/Exit state. Do nothing.
	
		}
	}

	/// <summary>
	/// Buffers the player data.
	/// </summary>

	void BufferPlayerData ()
	{
		if (buffer.Count <= 0) { //Buffer is empty. Fill it with the files for the next block.
//			Debug.Log ("Playback buffer empty. Loading");
			BodyHeadScoreDATA temp = new BodyHeadScoreDATA ();
			do {
				if (reader.isUsable) {
					temp = (BodyHeadScoreDATA)reader.ReadFrame ();
					buffer.Add (temp);
				} else { // At the end of the playback data; Break;
					break;
				}
			} while(bufferThreshold >= temp.body.x || bufferThreshold >= temp.head.x || temp.body.y <= -4.2f); // We haven't passed this one block yet. The ORIGIN OF THE BLOCK IN CENTRED. ONLY HALF LENGTH TO REACH START OF BLOCK
//			Debug.LogWarning(temp.body.x +":"+ temp.head.x + ":thres:" +bufferThreshold);
			bufferThreshold += 20f;
			if (warnOfErrors) {
				warnThisBlock = false; //Set this to false so we don't carry the warning over multiple blocks.
				ResetColor ();
				DetectDeath ();
			}
			if (correctErrors) {
				//ProcessFinalPathfinding ();
			}
		}
	}

	void ChangeColorToBlack ()
	{
		ghostbody.GetComponent<Renderer>().material.SetColor ("_Color", Color.black);
		ghosthead.GetComponent<Renderer>().material.SetColor ("_Color", Color.black);
		for (int i = 0; i < ghostBike.childCount; i++) {
			ghostBike.GetChild(i).GetComponent<Renderer>().material.SetColor ("_Color", Color.black);
		}
	}
	
	public void ResetColor ()
	{
		ghostbody.GetComponent<Renderer>().material.SetColor ("_Color", OriginalColor);
		ghosthead.GetComponent<Renderer>().material.SetColor ("_Color", OriginalColor);
		for (int i = 0; i < ghostBike.childCount; i++) {
			ghostBike.GetChild(i).GetComponent<Renderer>().material.SetColor ("_Color", OriginalColor);
		}
	}
	
	public void SetTransparency() {
		//at distance < 5, we have transparency of alpha 51
		//for every 5 distance above 5, add another 51 alpha, up to the max of 255
		Color currentColor = ghostbody.GetComponent<Renderer>().material.GetColor("_Color");
		float distance = (player.gameObject.transform.position - this.gameObject.transform.position).magnitude;
		float desiredAlpha = Mathf.Clamp(distance / 25.0f, 0.2f, 1.0f);
		Color desiredColor = new Color(currentColor.r, currentColor.g, currentColor.b, desiredAlpha);
		ghostbody.GetComponent<Renderer>().material.SetColor ("_Color", desiredColor);
		ghosthead.GetComponent<Renderer>().material.SetColor ("_Color", desiredColor);
		for (int i = 0; i < ghostBike.childCount; i++) {
			ghostBike.GetChild(i).GetComponent<Renderer>().material.SetColor ("_Color", desiredColor);
		}
	}
	
	private void DetectDeath ()
	{
		for (int i = 0; i < buffer.Count; i++) {
			if (((BodyHeadScoreDATA)buffer [i]).body.y <= -4.2f || ((BodyHeadScoreDATA)buffer [i]).head.x <= ((BodyHeadScoreDATA)buffer [i]).body.x - 4) { // We've fallen off the end or hit a barrier
				warnThisBlock = true; // set the warn trigger.
				break; // No need to keep looking. We're hurt this block so just warn until we pass it.
			}
		}	
	}
	
	//Generates a corrected pathfinding.
	
	private void ProcessFinalPathfinding ()
	{
		Debug.Log ("Attempting to correct errors");
		//Need to check if we ever fall
		int fallpos;
		for (fallpos = 0; fallpos < buffer.Count; fallpos++) {
			if (((BodyHeadScoreDATA)buffer [fallpos]).body.y <= -4.2f) { // We've fallen off the end
				break;
			}
		}
		if (fallpos == buffer.Count)
			return; // No errors here. No need to repair the pathing.
		
		if (!reader.isUsable)
			return; // We've died and DO NOT make it to the next block. Thus we shouldn't repair the data and actually die.
		
		//Ok, so we have an error.
		//Need to find the respawn.
		int respawnpos;
		for (respawnpos = fallpos; respawnpos < buffer.Count; respawnpos++) {
			if (!(((BodyHeadScoreDATA)buffer [respawnpos]).body.y <= -4.2f)) {
				break;
			}
		}
		buffer.RemoveRange (0, respawnpos); // remove everything until respawn.
		//Catch us back up to the current block.
		for (int i = 0; i < buffer.Count; i++) {
			if (((BodyHeadScoreDATA)buffer [i]).body.x <= bufferThreshold - 40) {
				buffer.RemoveAt (i--);
			}
		}
		TweenBetweenRespawnAndStart ();
		ProcessFinalPathfinding (); // GOtta recheck it now.
		
	}

	private void TweenBetweenRespawnAndStart ()
	{
		int framestweened = 10;
		Vector3 currentPos = transform.position;
		Vector3 nextPos = ((BodyHeadScoreDATA)buffer [framestweened]).body; // Lets just say 10 frames into it.
		float xdiff, ydiff, zdiff;
		
		xdiff = nextPos.x - currentPos.x;
		ydiff = nextPos.y - currentPos.y;
		zdiff = nextPos.z - currentPos.z;
		xdiff = xdiff / framestweened;
		ydiff = ydiff / framestweened;
		zdiff = zdiff / framestweened * 2;
		buffer.RemoveRange (0, framestweened); // Get rid of first 10;
		BodyHeadScoreDATA temp = new BodyHeadScoreDATA();
		for (int i = 1; i < framestweened/2; i++) {
			temp.body = new Vector3 (currentPos.x += xdiff * i, currentPos.y += ydiff * i, currentPos.z += zdiff * i);
			temp.head = Vector3.zero;
			buffer.Insert (i, temp);
			
		}
	}
	
	void NormalPlayback ()
	{
		BufferPlayerData ();
		if (buffer.Count == 0) { // YAY OUT OF DATA. KILL THIS. Can't use reader.isUsable because that gets flicked off after the final block is read BEFORE it gets played back.
			Debug.LogWarning ("Ghost Killed cause no more data");
			Alive = false;
			//UnityEngine.Object.Destroy (this.gameObject);
			return;
		}
		BodyHeadScoreDATA movement = DetermineNextMovementPosition ();
		ghostbody.GetComponent<Rigidbody>().MovePosition (movement.body);
		ghostBike.transform.position = ghostbody.transform.position + new Vector3 (0.325f, -0.15f, 0.0f);
		ghosthead.GetComponent<Rigidbody>().MovePosition (movement.head);
		this.score = movement.score;
	}
	
	
	float minuteIntervals = 10f, minuteDelta = 0f;
	void PacingPlayback ()
	{
		BufferPlayerData ();
		if (buffer.Count == 0) { // YAY OUT OF DATA. KILL THIS.
			Alive = false;
			ChangeColorToBlack();
			return;
		}
		BodyHeadScoreDATA movement = (BodyHeadScoreDATA)buffer [0]; // get movement this frame;
		buffer.RemoveAt (0);
		if(minuteDelta >= minuteIntervals){ 
			Debug.Log("Adjusting trainer based on current heart rate");
			DetectHeartRate ();
			minuteDelta = 0f;
		}
		minuteDelta += Time.deltaTime; // Start timing up to a minute.
		if (stayInFront)
			EnsureInFront (movement);
		transform.position = movement.body;
		ghosthead.position = movement.head;
		this.score = movement.score;
	}

	private bool halfratetrigger = true;

	private BodyHeadScoreDATA DetermineNextMovementPosition ()
	{
		if (playbackRate == 0.5) {
			if (halfratetrigger) {
				BodyHeadScoreDATA movement = (BodyHeadScoreDATA)buffer [0]; // get movement this frame.
				buffer.RemoveAt (0); // Remove it cause we're gonna use it.
				halfratetrigger = false;
				return movement;
			} else {
				halfratetrigger = true;
				BodyHeadScoreDATA movement = (BodyHeadScoreDATA)buffer [0];
				return movement;
				
			}
		} else if (playbackRate == 1f) { // normal playback
			BodyHeadScoreDATA movement = (BodyHeadScoreDATA)buffer [0]; // get movement this frame.
			buffer.RemoveAt (0); // Remove it cause we're gonna use it.
			return movement;
		} else if (playbackRate == 1.5f) { // Double Playback rate. SPEED UP.
			/*
			BodyHeadPOS movement = (BodyHeadPOS)buffer [0]; // get movement this frame.
			buffer.RemoveAt (0); // Remove it cause we're gonna use it.
			if (halfratetrigger) {
				movement.head = Vector3.Lerp (movement.head, ((BodyHeadPOS)buffer [0]).head, 0.5f);
				movement.body = Vector3.Lerp (movement.body, ((BodyHeadPOS)buffer [0]).body, 0.5f);
				halfratetrigger = false;
				return movement;
			} else {
				halfratetrigger = true;	
				return movement;
			}
			
			BodyHeadPOS movement = new BodyHeadPOS();
			if(halfratetrigger){
			movement = (BodyHeadPOS)buffer [0]; // get movement this frame.
			buffer.RemoveAt (0); // Remove it cause we're gonna use it.
			movement.head = Vector3.Lerp (ghosthead.position, ((BodyHeadPOS)buffer [0]).head, 0.5f);
				movement.body = Vector3.Lerp (ghostbody.position, ((BodyHeadPOS)buffer [0]).body, 0.5f);
				
				halfratetrigger = false;
			}else{
				movement = (BodyHeadPOS)buffer [0]; // get movement this frame.
			buffer.RemoveAt (0); // Remove it cause we're gonna use it.
				halfratetrigger = true;
				return movement;
			}
			return movement;
			*/
			BodyHeadScoreDATA movement = new BodyHeadScoreDATA();
			buffer.RemoveAt(0);
			if(halfratetrigger){
			movement.head = Vector3.Lerp (ghosthead.position, ((BodyHeadScoreDATA)buffer [1]).head, 0.5f);
			movement.body = Vector3.Lerp (ghostbody.position, ((BodyHeadScoreDATA)buffer [1]).body, 0.5f);
				halfratetrigger = false;
			}else{
				if(buffer.Count >1){
				movement = 	(BodyHeadScoreDATA)buffer [1];
				}else{
					movement = 	(BodyHeadScoreDATA)buffer [0];
				}
				buffer.RemoveAt(0);
				halfratetrigger = true;
			}
			return movement;
			
		} else if (playbackRate == 2f) {
			BodyHeadScoreDATA movement = new BodyHeadScoreDATA ();
			if (buffer.Count >= 2) {
				movement = (BodyHeadScoreDATA)buffer [1];
				buffer.RemoveAt (1);
			} else if (buffer.Count == 1) {
				movement = (BodyHeadScoreDATA)buffer [0];
			}	// get movement this frame.
			buffer.RemoveAt (0); // Remove it cause we're gonna use it.
			return movement; // get movement this frame.
		}
		return new BodyHeadScoreDATA ();
	}
	
	/* 	Target Heart Rates - American Heart Association - http://www.heart.org/HEARTORG/GettingHealthy/PhysicalActivity/Target-Heart-Rates_UCM_434341_Article.jsp
	 * 	Age			Target 50-85% zone		Avg Max Heart rate, 100%		
	 	20 years	100-170 beats per minute 200 beats per minute
		30 years	95-162 beats per minute	190 beats per minute
		35 years	93-157 beats per minute	185 beats per minute
		40 years	90-153 beats per minute	180 beats per minute
		45 years	88-149 beats per minute	175 beats per minute
		50 years	85-145 beats per minute	170 beats per minute
		55 years	83-140 beats per minute	165 beats per minute
		60 years	80-136 beats per minute	160 beats per minute
		65 years	78-132 beats per minute	155 beats per minute
		70 years	75-128 beats per minute	150 beats per minute
	 */
	//Checks the age of the player against the list above.
	private void DetectHeartRate ()
	{
		//Changing the way this is handled
		//We now calculate maximum as 208 - 0.7 * age (more accurate across most age ranges, comparable for the rest)
		//and lowbeat as 64% of max
		//and highbeat as 90% of max

		int maxHR = 208 - (int)(0.7f * (float)player.age);
		int lowBeat = (int)((float)maxHR * 0.64f);
		int highBeat = (int)((float)maxHR * 0.9f);
		SetModeViaHeartRate(player.age, lowBeat, highBeat, maxHR);



		/*if (player.age < 20) {
			
		} else if (player.age >= 20 && player.age < 30) {
			SetModeViaHeartRate (player.age, 100, 170, 200);
		} else if (player.age >= 30 && player.age < 35) {
			SetModeViaHeartRate (player.age, 95, 162, 190);
		} else if (player.age >= 35 && player.age < 40) {
			SetModeViaHeartRate (player.age, 93, 157, 185);
		} else if (player.age >= 40 && player.age < 45) {
			SetModeViaHeartRate (player.age, 90, 153, 180);
		} else if (player.age >= 45 && player.age < 50) {
			SetModeViaHeartRate (player.age, 88, 149, 175);
		} else if (player.age >= 50 && player.age < 55) {
			SetModeViaHeartRate (player.age, 85, 145, 170);
		} else if (player.age >= 55 && player.age < 60) {
			SetModeViaHeartRate (player.age, 83, 140, 165);
		} else if (player.age >= 60 && player.age < 65) {
			SetModeViaHeartRate (player.age, 80, 136, 160);
		} else if (player.age >= 65 && player.age < 70) {
			SetModeViaHeartRate (player.age, 78, 132, 155);
		} else {
			Debug.LogError ("Falling past all detect heart rate measures.	");
		}*/
	}
	
	private void SetModeViaHeartRate (int age, int lowbeat, int highbeat, int max)
	{
		int hrate = player.GetComponent<PlayerController> ().bike.heartRate;
		if (hrate < lowbeat) {
			//Debug.Log("Heartrate is below the low threshold");
			playbackRate = 1.5f;
			if (!stayInFront)
				stayInFront = true;
			
		} else if (hrate >= lowbeat && hrate <= highbeat) {
			//Debug.Log("Heartrate is inside the target burn rate.");
			playbackRate = 1f; // Return to normal playback rate.
			stayInFront = true;
			
		} else if (hrate > highbeat) {
			//Heart rate has exceeded the max threshold. Need to reduce it.
			playbackRate = 0.5f;
			if (stayInFront)
				stayInFront = false;
			
		}
	}
	
	private void EnsureInFront (BodyHeadScoreDATA movement)
	{
		while (player.transform.position.x+5 >= movement.body.x) {
			if (buffer.Count == 0)
				BufferPlayerData ();
			if (!reader.isUsable) {
				stayInFront = false;
				return;	
			}// Time to end staying in front :)
			movement = (BodyHeadScoreDATA)buffer [0];
			buffer.RemoveAt (0);
		}
	}
}