using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

public class GlobalSettings : MonoBehaviour {
	
	public bool OverrideEditorSettingsWithConfigFile = false;

	//Gameplay Settings
    [Header("Gameplay Settings")]
	public bool EnableLives = false;
	public bool EnableResistanceChanges = false;
	public bool EnableSounds = false;
	public bool EnableFanFeedback = false;
    public GeneratorMode LevelType = GeneratorMode.STANDARD;
	//public bool EnableBuildings = true;
    //public bool EnableTrees = false;
    public EnvironmentType environmentType = EnvironmentType.BARREN;
    public int GameDuration = 15;

	//Player Settings
    [Header("Player Settings")]
	public string PlayerName = "Player";
	public int PlayerAge = 24;
	public double PlayerBMI = 25;
	public int PlayerAssumedFitness = 4;

	//Playback Settings
    [Header("Playback Settings")]
	public bool RecordPlayer = false;
    public bool RecordLevel = false;
	public bool UseDetailedPlaybackSettings = true;	//If this is true, the settings that start with SimplePlayback are ignored
	public bool SimplePlaybackEnableGhost = false;
	public bool SimplePlaybackEnableTrainer = false;

	//Hardware Settings
    [Header("Hardware Settings")]
    public bool EnableBike = true;
    public bool EnableKinect = false;
    public bool EnableCamera = true;
	public string BikePort = "COM3";
	public string ResistancePort = "COM1";
	public bool UseOculus = true;

    //Interface Settings
    [Header("Interface Settings")]
    public bool EnableRadar = false;

    //Interval Settings
    [Header("Interval Settings")]
    public bool UsingIntervals = false;
    public int WarmupMinutes = 5;
    public int IntervalMinutes = 4;
    public int RecoveryMinutes = 3;


	//I really hate having to reference other gameobjects in the global settings script
	//but untill Unity allows for finding inactive gameobjects, it appears to be necessary
	//As with the method EnableAndDisableGameObjects, DON'T REFERENCE OTHER OBJECTS HERE UNLESS YOU HAVE TO
    [Header("Object References")]
	public GameObject OVRRig;
	public GameObject MainCamera;

	
	// We use Awake() to make sure settings are loaded before ANYTHING else happens (other objects may rely on settings in their start methods)
	// Because of this, anything that is not instantiated at runtime cannot have anything dependent on global settings in its Awake() method
    // as we can't guarantee that GlobalSettings.Awake() will be called before the Awake() method of other objects
	void Awake () {
		if (OverrideEditorSettingsWithConfigFile) {
			//load global settings from a config file
			IDictionary<string, string> settings = new Dictionary<string, string>();
			using (StreamReader configFile = new StreamReader(Environment.CurrentDirectory + "\\eg2config.cfg"))
			{
				string line;
				while ((line = configFile.ReadLine()) != null)
				{
					int equalsIndex = line.IndexOf('=');
					if (equalsIndex >= 0) {
						settings.Add(line.Substring(0, equalsIndex), line.Substring(equalsIndex + 1, line.Length - (equalsIndex + 1)));	
					}
				}
			}
			//now that we've read all the settings into a dictionary, parse them into our specific settings
			SetSettingsFromDictionary(settings);
		}
		//Make some changes as required by our settings
		EnableAndDisableObjectsBasedOnSettings();
	}
				
	void SetSettingsFromDictionary(IDictionary<string, string> values) {
		//-------------------------Game Parameters-------------------------
		if (values.ContainsKey("EnableLives")) {
			EnableLives = Boolean.Parse(values["EnableLives"]);
		}
		if (values.ContainsKey("EnableResistanceChanges")) {
			EnableResistanceChanges = Boolean.Parse(values["EnableResistanceChanges"]);
		}
		if (values.ContainsKey("EnableSounds")) {
			EnableSounds = Boolean.Parse(values["EnableSounds"]);
		}
		if (values.ContainsKey("EnableFanFeedback")) {
			EnableFanFeedback = Boolean.Parse(values["EnableFanFeedback"]);
		}
        if (values.ContainsKey("LevelType"))
        {
            LevelType = (GeneratorMode)(int.Parse(values["LevelType"]));
        }
        //if (values.ContainsKey("EnableBuildings"))
        //{
        //    EnableBuildings = Boolean.Parse(values["EnableBuildings"]);
        //}
        //if (values.ContainsKey("EnableTrees"))
        //{
        //    EnableTrees = Boolean.Parse(values["EnableTrees"]);
        //}
        if (values.ContainsKey("EnvironmentType"))
        {
            environmentType = (EnvironmentType)(int.Parse(values["EnvironmentType"]));
        }
        if (values.ContainsKey("GameDuration"))
        {
            GameDuration = int.Parse(values["GameDuration"]);
        }
		//-------------------------Player Info-------------------------
		if (values.ContainsKey ("PlayerName")) {
			PlayerName = values["PlayerName"];
		}
		if (values.ContainsKey ("PlayerAge")) {
			PlayerAge = int.Parse(values["PlayerAge"]);
		}
		if (values.ContainsKey ("PlayerBMI")) {
			PlayerBMI = double.Parse(values["PlayerBMI"]);
		}
		if (values.ContainsKey ("PlayerAssumedFitness")) {
			PlayerAssumedFitness = int.Parse(values["PlayerAssumedFitness"]);
		}
		//-------------------------Playback Settings-------------------------
		if (values.ContainsKey("RecordPlayer")) {
			RecordPlayer = Boolean.Parse(values["RecordPlayer"]);
		}
        if (values.ContainsKey("RecordLevel"))
        {
            RecordLevel = Boolean.Parse(values["RecordLevel"]);
        }
		if (values.ContainsKey("UseDetailedPlaybackSettings")) {
			UseDetailedPlaybackSettings = Boolean.Parse(values["UseDetailedPlaybackSettings"]);
		}
		if (values.ContainsKey("SimplePlaybackEnableGhost")) {
			SimplePlaybackEnableGhost = Boolean.Parse(values["SimplePlaybackEnableGhost"]);
		}
		if (values.ContainsKey("SimplePlaybackEnableTrainer")) {
			SimplePlaybackEnableTrainer = Boolean.Parse(values["SimplePlaybackEnableTrainer"]);
		}
		//-------------------------Hardware Settings-------------------------
        if (values.ContainsKey("EnableBike"))
        {
            EnableBike = bool.Parse(values["EnableBike"]);
        }
        if (values.ContainsKey("EnableKinect"))
        {
            EnableKinect = bool.Parse(values["EnableKinect"]);
        }
        if (values.ContainsKey("EnableCamera"))
        {
            EnableCamera = bool.Parse(values["EnableCamera"]);
        }
		if (values.ContainsKey ("BikePort")) {
			BikePort = values["BikePort"];
		}
		if (values.ContainsKey ("ResistancePort")) {
			ResistancePort = values["ResistancePort"];
		}
		if (values.ContainsKey ("UseOculus")) {
			UseOculus = bool.Parse(values["UseOculus"]);
		}
        //-------------------------Interface Settings-------------------------
        if (values.ContainsKey("EnableRadar"))
        {
            EnableRadar = bool.Parse(values["EnableRadar"]);
        }
        //-------------------------Interval Settings--------------------------
        if (values.ContainsKey("UsingIntervals"))
        {
            UsingIntervals = bool.Parse(values["UsingIntervals"]);
        }
        if (values.ContainsKey("WarmupMinutes"))
        {
            WarmupMinutes = int.Parse(values["WarmupMinutes"]);
        }
        if (values.ContainsKey("IntervalMinutes"))
        {
            IntervalMinutes = int.Parse(values["IntervalMinutes"]);
        }
        if (values.ContainsKey("RecoveryMinutes"))
        {
            RecoveryMinutes = int.Parse(values["RecoveryMinutes"]);
        }
	}

	//Where possible, AVOID PUTTING THINGS IN THIS METHOD unless you really have to
	//This should just be used for enabling and disabling objects for which their state is relevant on game start
	void EnableAndDisableObjectsBasedOnSettings() {
		//MainCamera.SetActive(!UseOculus);
		//OVRRig.SetActive(UseOculus);
	}
}