using UnityEngine;
using System.Collections.Generic;
using System;
using Exergame;

public class PlaybackManager : MonoBehaviour
{
	//Links to other objects
	public PlayerController player;
	public Transform Ghost;
	public GlobalSettings globalSettings;
	
	//Important for playback
	public int playbackNumber;
	private Transform[] ghosts;
	public string debugPlaybackData; //Fallback data if the specific data cannot be loaded
	private string PlaybackDataDirectory = Environment.CurrentDirectory + "\\WorldPlaybackData\\Player\\"; // Playback directory.
	public bool dataprovided; // Use this if we want to specify some particular data in the next 6 variables.
	public string ghost1Data, ghost2Data, ghost3Data, ghost4Data, ghost5Data, ghost6Data; // Just specify the filenames for the playback data. It gets added to the playbackdirectory.
	
	//GUI stuff
	public bool DisplayDistances;
	public bool DisplayGhostScore;
	public bool RemoveGuiElementOnEndPlayback;
	
	//Modes
	public bool PacingMode; //Char always stays in front. - auto sets correct errors if not set.
	public bool StayInFront; //Makes the character just always stay in front. Pacing Mode triggers this. 
	public bool NormalPlaybackMode; // Standard Replay - Used for playing against own ghost/other ghosts. Can be triggered without CorrectErrors
	public bool CompetitiveMode; // Similar to a race. Makes sure correct errors is false - errors are part of the race!. CAN BE TRIGGERED WITH WarnPlayerOfErrors.
	public bool CorrectErrors; // Corrects mistakes in ghost playback. Stops them falling down holes :3 AUTO SET IF PACING MODE IS SET!!!!
	public bool WarnPlayerOfErrors; // Causes ghost to signal the player that they are about to make a mistake. CAN BE SET WITH OR WITHOUT CORRECTERRORS.
	
	private bool setColours = true; // Have to do this in fixed update. I can't seem to set any of the information before the end of start().
	
	//Currently Player information.
	private string Username;
	private int Age;
	private int BMI;
	private int AssumedFitness;
	
	//When selecting playback data from the datapool - prefer my data. Uses the username to select ONLY FROM MY DATA.
	public bool PreferMyData;
	public bool DoNotUseMyData; // Makes it so we remove all my data from the set. Useful for mplayer.

	// Use this for initialization
	void Start ()
	{
		//If we are using detailed settings, use the editor defined values, otherwise set them from global settings
		if (!globalSettings.UseDetailedPlaybackSettings) {
			if (!globalSettings.SimplePlaybackEnableGhost && !globalSettings.SimplePlaybackEnableTrainer) {
				//No ghost or trainer, do no playback
				playbackNumber = 0;
			}
			else if (globalSettings.SimplePlaybackEnableGhost) {
				//we're using the player's replay
				playbackNumber = 1;
				dataprovided = false;
				PreferMyData = true;
				PacingMode = false;
				StayInFront = false;
				NormalPlaybackMode = true;
				CorrectErrors = false;
			}
			else if (globalSettings.SimplePlaybackEnableTrainer) {
				playbackNumber = 1;
				ghost1Data = "TrainerData.egp";
				dataprovided = true;
				PacingMode = true;
				StayInFront = true;
				NormalPlaybackMode = false;
				CorrectErrors = false;
				PreferMyData = false;
			}
		}

		//Retrive info from global settings
		Username = globalSettings.PlayerName;
		Age = globalSettings.PlayerAge;
		BMI = (int)globalSettings.PlayerBMI;
		AssumedFitness = globalSettings.PlayerAssumedFitness;

		//if (PacingMode && !CorrectErrors) { // Should be done anyway in CheckModeInputs
		//	CorrectErrors = true; // If in pacing mode, then MAKE SURE WE CORRECT ERRORS. DONT WANT THEM FALLING OFF CLIFFS.	
		//}
		if (playbackNumber > 6) {
			playbackNumber = 6;
			Debug.LogWarning ("Playback is capped at 6. Only using 6 recordings.");	
			
		}
		ghosts = new Transform[playbackNumber]; // Set up the array of ghosts. This is important if we decide to swap in more ghosts as the player passes them.
		//It's possible to instantiate multiple ghosts with different playback data so long as their PlaybackData string is set before Init.
		Ghost.GetComponent<GhostController> ().player = player;
		CheckModeInputs ();
		Ghost.GetComponent<GhostController> ().correctErrors = this.CorrectErrors;
		Ghost.GetComponent<GhostController> ().normalmode = this.NormalPlaybackMode;
		Ghost.GetComponent<GhostController> ().pacingmode = this.PacingMode;
		Ghost.GetComponent<GhostController> ().competitivemode = this.CompetitiveMode;
		Ghost.GetComponent<GhostController> ().warnOfErrors = this.WarnPlayerOfErrors;
		Ghost.GetComponent<GhostController> ().stayInFront = this.StayInFront;
		
		Ghost.GetComponent<GhostController> ().manager = this;
		if (!dataprovided) {
			DetermineDataToUse ();
		}
		//TODO: Refactor the ghost system so that it uses an array or a list, instead of hard coded numbers
		for (int i =0; i < playbackNumber; i++) {
			switch (i) {
			case 0:
				//Ghost.GetComponent<GhostController> ().ghostInfo = GetGhostInfo(ghost1Data);
				Ghost.GetComponent<GhostController> ().PlaybackData = PlaybackDataDirectory + ghost1Data;
				break;
			case 1:
				Ghost.GetComponent<GhostController> ().ghostInfo = GetGhostInfo (ghost2Data);
				Ghost.GetComponent<GhostController> ().PlaybackData = PlaybackDataDirectory + ghost2Data;
				break;
			case 2:
				Ghost.GetComponent<GhostController> ().ghostInfo = GetGhostInfo (ghost3Data);
				Ghost.GetComponent<GhostController> ().PlaybackData = PlaybackDataDirectory + ghost3Data;
				break;
			case 3:
				Ghost.GetComponent<GhostController> ().ghostInfo = GetGhostInfo (ghost4Data);
				Ghost.GetComponent<GhostController> ().PlaybackData = PlaybackDataDirectory + ghost4Data;
				break;
			case 4:
				Ghost.GetComponent<GhostController> ().ghostInfo = GetGhostInfo (ghost5Data);
				Ghost.GetComponent<GhostController> ().PlaybackData = PlaybackDataDirectory + ghost5Data;
				break;
			case 5:
				Ghost.GetComponent<GhostController> ().ghostInfo = GetGhostInfo (ghost6Data);
				Ghost.GetComponent<GhostController> ().PlaybackData = PlaybackDataDirectory + ghost6Data;
				break;
			default:
				Debug.LogWarning ("No unique playback data for this ghost. Using the debug data!");
				Ghost.GetComponent<GhostController> ().ghostInfo = new PlaybackInformation ();
				Ghost.GetComponent<GhostController> ().ghostInfo.username = "DEBUG";
				Ghost.GetComponent<GhostController> ().ghostInfo.recordTime = System.DateTime.Today;
				Ghost.GetComponent<GhostController> ().PlaybackData = debugPlaybackData;
				break;
			}
			ghosts [i] = (Transform)(Instantiate (Ghost, new Vector3 (0, 0.0f, 0), Quaternion.identity));
		}
	
		//Time to set the colours.
		//Debug.LogError(ghosts[0].GetComponent<GhostController>().ghostInfo.recordTime);
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
		if (setColours && ghosts.Length <0) {
			if (player.GetGameState () == GameState.GAME_STATE_PLAY) {
				SetPlaybackColor ();
			}
		}
		//Debug.LogError(ghosts[0].GetComponent<GhostController>().ghostInfo.recordTime);
		if (DisplayDistances) {
			for (int i = 0; i < ghosts.Length; i++) {
				if (ghosts [i] == null) {
					if (RemoveGuiElementOnEndPlayback) {
						//player.gui.RemoveGhostDistanceInfo (i);	
					} else {
						continue;
					}
				} else {
					//player.gui.UpdateGhostInfo (i, Vector3.Distance (player.transform.position, ghosts [i].GetChild (0).transform.position));
					//player.gui.UpdateGhostDistanceInfo (i, player.transform.position.x - ghosts [i].GetChild (0).transform.position.x);
				}
			}
		}
		if (DisplayGhostScore) {
			for (int i = 0; i < ghosts.Length; i++) {
				if (ghosts [i] == null) {
					if (RemoveGuiElementOnEndPlayback) {
						//player.gui.RemoveGhostScoreInfo (i);
					} else {
						continue;
					}
				} else {
					//player.gui.UpdateGhostInfo (i, Vector3.Distance (player.transform.position, ghosts [i].GetChild (0).transform.position));
					//player.gui.UpdateGhostScoreInfo (i, (int)ghosts [i].GetComponent<GhostController> ().score);
				}
			}
		}
	}
	//We prefer just normal playback mode. So that get ticked first. Then prefer pacingMode. Then competitvemode.
	private void CheckModeInputs ()
	{
		if (NormalPlaybackMode) {
			PacingMode = false;
			CompetitiveMode = false;
			return;
		}
		if (PacingMode) {
			CompetitiveMode = false;
			CorrectErrors = true; //Force pacing mode to correct errors.
			StayInFront = true; //Pacing mode must always stay in front. At least initially.
		}
		if (CompetitiveMode) {
			CorrectErrors = false;
		}
	}

	void SetPlaybackColor ()
	{
		setColours = false;
		List<PlaybackInformation> info = new List<PlaybackInformation> ();
		info.Add (ghosts [0].GetComponent<GhostController> ().ghostInfo);
		//foreach (PlaybackInformation s in info) {
			//Debug.LogError(s.recordTime);	
		//}
		//Older recordings should be closer to the end. But just getting this info now if I need to improve this later.
		for (int i = 0; i < ghosts.Length; i++) {
			ghosts [i].GetComponent<GhostController> ().OriginalColor -= new Color (0.1f * i, 0.1f * i, 0.1f * i, 1);
			ghosts [i].GetComponent<GhostController> ().ResetColor ();
		}
	}
	
	public PlaybackInformation GetGhostInfo (string filename)
	{
		string[] data = filename.Split ('_'); // split on the underscore char
		PlaybackInformation temp = new PlaybackInformation ();

		//The filename may not contain player info (e.g. for trainer data)
		//under these circumstances, we give it data based on the current player
		if (data.Length != 5) {
			temp.username = filename;
			temp.age = Age;
			temp.BMI = BMI;
			temp.assumedFitness = AssumedFitness;
			temp.recordTime = DateTime.Now;
			temp.filename = filename;
		} else {
			//filename contains our info, populate it
			temp.username = data [0];
			temp.age = int.Parse (data [1]);
			temp.BMI = int.Parse (data [2]);
			temp.assumedFitness = int.Parse (data [3]);
			temp.recordTime = new System.DateTime (long.Parse (data [4])); // IMPORTANT TO GET THE DATE.
			temp.filename = filename;
		}
		//Debug.LogError(temp.recordTime);
		return temp; // Return the strut with the info.
	}
	
	private void DetermineDataToUse ()
	{
		string[] files = System.IO.Directory.GetFiles (PlaybackDataDirectory); // Get the directory where the playback is apparently stored.
		List<PlaybackInformation> Info = new List<PlaybackInformation> ();
		
		foreach (string s in  files) {
			Debug.LogWarning (System.IO.Path.GetFileName (s));
			Info.Add (GetGhostInfo (System.IO.Path.GetFileName (s)));
		}
		//Yay we have all the data info.
		//Remove files that aren't mine if I only want my data.
		if (PreferMyData && DoNotUseMyData)
			DoNotUseMyData = false; // Both triggers are set. Set to prefer my data in this case. Can't have both.
		if (PreferMyData || DoNotUseMyData) {
			for (int i = 0; i < Info.Count; i++) {
				if (DoNotUseMyData) {
					if (Info [i].username.Equals (Username)) {
						Info.RemoveAt (i--);
					}
				}
			}
		}
		if (PreferMyData) {
			Info.Sort (delegate(PlaybackInformation a, PlaybackInformation b){
				if (a.username.Equals (Username)) {
					return 1;
				} else if (b.username.Equals (Username)) {
					return 1;
				} else {
					return 0;
				}
			});
		}
		foreach (PlaybackInformation p in Info) {
			Debug.Log (p.filename);	
		}
		
		for (int i = 0; i < playbackNumber; i++) {
			switch (i) {
			case 0:
				ghost1Data = Info [i].filename;
				break;
			case 1:
				ghost2Data = Info [i].filename;
				break;
			case 2:
				ghost3Data = Info [i].filename;
				break;
			case 3:
				ghost4Data = Info [i].filename;
				break;
			case 4:
				ghost5Data = Info [i].filename;
				break;
			case 5:
				ghost6Data = Info [i].filename;
				break;
			default:
				Debug.LogError ("Something when wrong in determining the playback data! Too many ghosts?");
				break;
			}
		}
	}

	int counter = 0;

	public Transform NextGhost ()
	{
		Transform temp;
		if (CheckAllForNull ())
			return null; // Stop us hitting inf loop if all ghosts are dead.
		do {
			if (counter >= ghosts.Length)
				counter = 0;
			temp = ghosts [counter];
			counter++;
		} while(temp == null);
		return temp;
	}
	//returns true if all ghosts are dead. Used to break out of loop;
	private bool CheckAllForNull ()
	{
		for (int i = 0; i < ghosts.Length; i++) {
			if (ghosts [i] != null)
				return false;	
		}
		return true; // all are null
	}
}

//Struct to contain the information about recorded player data.
public class PlaybackInformation
{
	public string filename;
	public string username;
	public int age;
	public int BMI;
	public int assumedFitness;
	public System.DateTime recordTime;
	
	public PlaybackInformation ()
	{
			
		
	}
	
	public PlaybackInformation (string filename, string name, int age, int b, int fitness, System.DateTime time)
	{
		this.filename = filename;
		this.username = name;
		this.age = age;
		this.BMI = b;
		this.assumedFitness = fitness;
		this.recordTime = time;
	}
}