using UnityEngine;
using System.Collections;
using System.IO;
using System;
using AssemblyCSharp;
using Exergame;

public class PlayerController : MonoBehaviour
{
    [Header("Oculus Based Turning")]
    public GameObject usersHead;
    public bool headTiltMovement;
    public bool headOffsetMovement;

    //References to scene objects
    [Header("Scene Objects")]
	public UIController ui;
	public Generator generator;
	public KinectController kinect;
	public PipeSystemController cameraTracker;
	public BikeController bike;
	public ResistanceController resistanceController;
	public LogController logController;
	public GlobalSettings globalSettings;
    public HRBarController hrBar;
	public Transform playerHead;
	public TerrainSelector terrain;
	public Transform bikeMesh;
    
    //Sounds
    [Header("Sounds")]
    public AudioSource bikeOn;
    public AudioSource bikeOff;
    public AudioSource pointsAward;
    public AudioSource pointsDeny;

	private static readonly int DEFAULT_RESISTANCE = 7;
	public static int STARTING_LIVES = 1;
	
	private GameState gameState = GameState.GAME_STATE_WAIT;
	private float speed = 220.0f;
    private float speedNoForce = 8.0f;
    private int score = 10000;
	private float resistancePowerupDurationRemaining = 0.0f;
	private bool environmentalResistanceOverride = false;
	private int environmentalResistance = 15;

	public int lives { get; private set; }

	public bool canDie = true;	//We use this for triggering a delayed death - can't have the player dying again while they are already dying
	private float timeToNewGame = 0.0f;
	private float timeToExit = 10.0f;
	public bool LimitGameLength;
	private int GameLengthInMinutes;
	private float gameLengthInSeconds, elapsedGameLength = 0f;
	private PlayerWriter playerWriter = null;
	private PlayerReader playerReader;
	private bool writePlayerData;
    private bool pedalling = false;     //Used for tracking whether the player is *actively* pedalling, as opposed to coasting. Needed for sounds
    private string _lanePosition;

	//RecordingInformations
	public bool StoreInformationAboutPlayerAsFilename;
	private string username;
	public int age;
	private double BMI;
	private int assumedFitness;
	
	//Gamemode
	public bool Cooperative, Competitive; // The two gametypes. 
	
	
	// Use this for initialization
	void Start ()
	{
		//pull the player's info from global settings
		username = globalSettings.PlayerName;
		age = globalSettings.PlayerAge;
		BMI = globalSettings.PlayerBMI;
		assumedFitness = globalSettings.PlayerAssumedFitness;
		writePlayerData = globalSettings.RecordPlayer;
        GameLengthInMinutes = globalSettings.GameDuration;
		//set the game parameters
		lives = STARTING_LIVES;
		
		if (writePlayerData) {
			if (StoreInformationAboutPlayerAsFilename) {
				playerWriter = new PlayerWriter (username, age, BMI, assumedFitness);	
			} else {
				playerWriter = new PlayerWriter ();
			}
		}
		gameLengthInSeconds = GameLengthInMinutes * 60;
	}
	
	// FixedUpdate is called once per physcics tick
	void FixedUpdate ()
	{
        if (Time.time > 5)
        {
            _lanePosition = SetLanePosition();
            //handle horizontal movement. Priority is Kinect > Camera > Keyboard
            float moveHorizontal = 0.0f;
            if (kinect.EnableKinect)
            {
                moveHorizontal = kinect.movement;
            }
            else if (cameraTracker.EnableCamera)
            {
                moveHorizontal = cameraTracker.PositionOffset.x * 2;

            }
            else if (headTiltMovement)
            {
                //Only start moving after a certain angle has be achieved as head naturally bobs side to side
                if (usersHead.transform.localRotation.eulerAngles.z < 270 && usersHead.transform.localRotation.eulerAngles.z > 15)
                {
                    moveHorizontal = usersHead.transform.localRotation.eulerAngles.z * -0.02f;
                }
                if (usersHead.transform.localRotation.eulerAngles.z > 270 && usersHead.transform.localRotation.eulerAngles.z < 345)
                {
                    moveHorizontal = (usersHead.transform.localRotation.eulerAngles.z - 360) * -0.02f;
                }

            }
            else if (headOffsetMovement)
            {

                moveHorizontal = (transform.position.z - usersHead.transform.position.z) * 10;

            }
            else
            {
                moveHorizontal = Input.GetAxis("Horizontal");
            }
            //Cap horizontal movement
            if (moveHorizontal > 1.2f)
            {
                moveHorizontal = 1.2f;
            }
            else if (moveHorizontal < -1.2f)
            {
                moveHorizontal = -1.2f;
            }

            //Handle vertical movement. Priority is Bike > Keyboard
            float moveVertical = 0.0f;
            if (bike.enableBike)
            {
                moveVertical = bike.speed;
            }
            else
            {
                moveVertical = Input.GetAxis("Vertical");
            }

            pedalling = Mathf.Abs(moveVertical) > 0.2f;

            //use shift keys to move fast when testing with the keyboard
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                moveVertical *= 2;
            }

            //we want to allow a quick exit if something goes bad
            if (Input.GetKey(KeyCode.Escape))
            {
                Debug.Log("Force Quitting");
                Application.Quit();
            }

            //Don't allow movement until the movement tracking system has finished initialising
            if ((kinect.EnableKinect && !kinect.calibrationFinished) || (cameraTracker.EnableCamera && !cameraTracker.CameraInitialised))
            {
                moveVertical = 0.0f;
                moveHorizontal = 0.0f;
            }

            //Update the game based on the game state
            if (gameState == GameState.GAME_STATE_PLAY)
            {

                if (gameLengthInSeconds <= elapsedGameLength && LimitGameLength)
                {
                    DoGameOver(); // End the game after 10 minutes.
                }
                elapsedGameLength += Time.deltaTime;
                //before we apply the player's movement, if they are trying to move in the opposite direction
                //to their current movement, we immediately reset their lateral velocity
                if ((GetComponent<Rigidbody>().velocity.z > 0 && moveHorizontal > 0) || (GetComponent<Rigidbody>().velocity.z < 0 && moveHorizontal < 0) || moveHorizontal == 0.0f)
                {
                    GetComponent<Rigidbody>().velocity = new Vector3(GetComponent<Rigidbody>().velocity.x, 0.0f, 0.0f);
                }

                float verticalFactor = moveVertical != 0.0f ? moveVertical * speedNoForce : GetComponent<Rigidbody>().velocity.x;
                float horizontalFactor = Mathf.Abs(moveHorizontal) > 0.1f ? -moveHorizontal * speedNoForce * 0.5f : GetComponent<Rigidbody>().velocity.z;
                GetComponent<Rigidbody>().velocity = new Vector3(verticalFactor, 0.0f, horizontalFactor);// *Time.deltaTime;
                                                                                                         //rigidbody.AddForce (new Vector3 (moveVertical, 0.0f, -moveHorizontal) * speed * Time.deltaTime);
                                                                                                         //crude way of handling it, but for now just jump the terrain ahead if we push too far
                if (terrain.currentTerrain != null && this.transform.position.x > terrain.currentTerrain.transform.position.x + 1400.0f)
                {
                    terrain.currentTerrain.transform.position = new Vector3(terrain.currentTerrain.transform.position.x + 1200.0f, 0f, -1000.0f);
                }

                if (writePlayerData)
                {
                    playerWriter.WritePositions(transform.position, playerHead.transform.position, score);
                }

                //if the player has fallen down a pit or off the side, kill 'em
                if (this.transform.position.y <= -20.0f && canDie)
                {
                    if (lives == 0)
                    {
                        DoGameOver();
                    }
                    else
                    {
                        Respawn();
                    }
                }

                //update the resistance of the bike based on what is going on in game
                if (resistancePowerupDurationRemaining >= 0.0f)
                {
                    resistancePowerupDurationRemaining -= Time.deltaTime;
                    ui.SetRemainingCharge((int)(resistancePowerupDurationRemaining * 10.0f));
                }

                int resistance = DetermineDesiredResistance();
                resistanceController.SetResistance(resistance);

                //update the speed of the fan based on how fast the player is going
                if (globalSettings.EnableFanFeedback)
                {
                    float speedCap = 3.0f;
                    float cappedSpeed = Mathf.Clamp(moveVertical, 0.0f, speedCap);
                    int speedPercentage = (int)(cappedSpeed * (100.0f / speedCap));
                    resistanceController.SetFanSpeed(speedPercentage);
                }

                PlayAppropriateAudio();

                //We can use the Q key to force a game over while testing
                if (Input.GetKeyDown(KeyCode.Q))
                {
                    DoGameOver();
                }

            }
            else if (gameState == GameState.GAME_STATE_OVER)
            {
                if (timeToNewGame <= 0.0f)
                {
                    if (moveVertical > 0.0f)
                    {
                        //Player is pedalling at the end of the five seconds, they are still going
                        StartNewGame();
                    }
                    else
                    {
                        //They've stopped pedalling, they're not keen to continue, close down
                        ExitGame();
                    }
                }
                else
                {
                    timeToNewGame -= Time.deltaTime;
                }
            }
            else if (gameState == GameState.GAME_STATE_WAIT)
            {
                if (moveVertical != 0.0f)
                {
                    gameState = GameState.GAME_STATE_PLAY;
                }
            }
            bikeMesh.transform.position = this.transform.position + new Vector3(0.325f, -0.15f, 0.0f);
        }
	}
	//Gets called when the player runs out of lives
	public void DoGameOver ()
	{
		gameState = GameState.GAME_STATE_OVER;
		SaveScore(score);
		ui.ShowGameOver ();
        if (bikeOn.isPlaying)
        {
            bikeOn.Stop();
        }
        if (bikeOff.isPlaying)
        {
            bikeOff.Stop();
        }
		generator.ClosePlaybackWriter ();
		if (writePlayerData) {
			playerWriter.ClosePlayerWriter (); // Close the playback writer to make sure it gets put to disk.
		}
		timeToNewGame = 6.0f;

		Debug.Log ("Game Over");

	}
	
	//Gets called when the player gets a game over and chooses to keep playing
	public void StartNewGame ()
	{
		Debug.Log ("Starting new game");
		score = 10000;
		this.GetComponent<Rigidbody>().velocity = new Vector3 (0.0f, 0.0f, 0.0f);
		this.transform.position = new Vector3 (0.0f, 1.0f, 0.0f);
        this.playerWriter = new PlayerWriter(username,age,BMI,assumedFitness); // Restart the playback with a new file.
		generator.ClearAllBlocks ();
		generator.ReopenPlaybackReader ();
        if (terrain.currentTerrain != null)
        {
            terrain.currentTerrain.transform.position = new Vector3(-200.0f, -5.0f, -1000.0f);
        }
		canDie = true;
		lives = STARTING_LIVES;
		ui.SetLives (lives);
		ui.SetScore (score);
		ui.HideGameOver ();
		gameState = GameState.GAME_STATE_PLAY;
		elapsedGameLength = 0f; // Reset the timer.
	}
	
	//We call this if the participant gets a game over and does not choose to keep playing
	public void ExitGame ()
	{
		Debug.Log ("Exiting game");
		gameState = GameState.GAME_STATE_EXIT;
		bike.ending = true;
		kinect.ending = true;
		logController.Finish ();
		Application.Quit ();
	}
	
	public void Respawn ()
	{
		//we only check the lives setting here so that if for some reason a death condition is entered (goes over the side or something)
		//the player will still respawn
		if (globalSettings.EnableLives) {
			lives--;
		}
		this.transform.position = generator.GetPlayerRespawnPosition ();
		this.GetComponent<Rigidbody>().velocity = new Vector3 (0.0f, 0.0f, 0.0f);
		ui.SetLives (lives);
		canDie = true;
	}
	
	public void AwardPowerup (PowerupType type)
	{
		switch (type) {
		case PowerupType.Life:
			lives++;
			break;
		case PowerupType.Resistance:
			resistancePowerupDurationRemaining = 10.0f;
			ui.SetRemainingCharge(100);
			break;
		case PowerupType.Score:
            int amount = (int)(1000 * hrBar.scoreMultiplier);
			score += amount;
			ui.GiveScore(amount);
			break;
		}
	}

    public void TakePoints(int amount)
    {
        //if they are under the effects of the resistance powerup, they can plough through obstacles without worry
        if (resistancePowerupDurationRemaining <= 0.0f)
        {
            score -= amount;
            pointsDeny.Play();
            ui.GiveScore(-amount);
        }
    }

    public void GivePoints(int amount)
    {
        float multiplier = hrBar.scoreMultiplier;
        amount = (int)(amount * multiplier);
        score += amount;
        pointsAward.Play();
        ui.GiveScore(amount);
    }
	
	void SaveScore (int score)
	{
		string text = string.Format ("{0} || {1}", DateTime.Now, score);
		try 
        {
			using (StreamWriter file = new StreamWriter("scores.txt", true)) 
            {
				file.WriteLine (text);
			}
		} 
        catch (IOException e) 
        {
			Debug.Log (e.ToString ());
		}
	}
	
	int DetermineDesiredResistance ()
	{
		//cut this short if we're not allowing changes to resistance (with no allowed changes to resistance, resistance always sits at the default level
		if (globalSettings.EnableResistanceChanges == false) {
			return DEFAULT_RESISTANCE;
		}

		int desiredResistance;
		//first up, if they have a powerup, the resistance is 1. This takes priority over all other factors
		if (environmentalResistanceOverride)
        {
            desiredResistance = environmentalResistance;
        }
        else if (resistancePowerupDurationRemaining > 0.0f)
        {
            desiredResistance = 1;
        }
        else
        {
            desiredResistance = DEFAULT_RESISTANCE;
        }
		return desiredResistance;
	}
	
	public void BeginEnvironmentalResistanceOverride(int resistance) {
		environmentalResistanceOverride = true;
		environmentalResistance = resistance;
	}
	
	public void EndEnvironmentalResistanceOverride() {
		environmentalResistanceOverride = false;
		environmentalResistance = DEFAULT_RESISTANCE;
	}

    public void PlayAppropriateAudio()
    {
        if (pedalling)
        {
            //if the player is pedalling, the pedalling clip should be playing
            if (bikeOff.isPlaying)
            {
                bikeOff.Stop();
            }
            if (!bikeOn.isPlaying)
            {
                bikeOn.Play();
            }
        }
        else if (GetComponent<Rigidbody>().velocity.x > 0.1f)
        {
            //if they are not pedalling, but still moving, the coasting clip should be playing
            if (bikeOn.isPlaying)
            {
                bikeOn.Stop();
            }
            if (!bikeOff.isPlaying)
            {
                bikeOff.Play();
            }
        }
        else
        {
            //if they are neither pedalling nor moving, no clip should be playing
            if (bikeOn.isPlaying)
            {
                bikeOn.Stop();
            }
            if (bikeOff.isPlaying)
            {
                bikeOff.Stop();
            }
        }
    }
	
	public GameState GetGameState ()
	{
		return gameState;
	}

	void OnApplicationQuit() {
		if (playerWriter != null) {
			playerWriter.ClosePlayerWriter();
			playerWriter = null;
		}
	}

    string SetLanePosition()
    {
        var pos = transform.position.z;
        if (pos < -0.6f)
            return "left";
        else if (pos > 0.8)
            return "right";
        return "centre";
    }

    public string GetLanePosition()
    {
        return _lanePosition;
    }
}