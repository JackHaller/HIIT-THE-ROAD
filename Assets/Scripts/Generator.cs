using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Exergame;

public enum GeneratorMode
{
    STANDARD = 0,
    INTERVAL = 1,
    REPLAY,
}

public class Generator : MonoBehaviour
{
	//Core gameplay design factors
	public bool PERMIT_BARRIER_GAPS = true;	//if false, there will be a solid barrier to the side of the track at all points
	public bool USE_SAND = true;			//if true, the pits in the game are filled with sand, rather than killing the player
	public bool USE_FATAL_OVERHEAD = true;	//if false, overhead barriers apply a slow, rather than killing the player.
    public bool PERMIT_RAMPS = false;
    public GeneratorMode generatorMode = GeneratorMode.STANDARD;
	//public bool EnableBuildings = true;
	public GlobalSettings globalSettings;
    public IntervalController intervalController;

    

    //Prefabs for building the game
    public Transform Straight;
    public Transform StraightBlockedStart;
    public Transform StraightBlockedZone;
	public Transform RespawnPlatform;
	//Powerups
	public Transform PowerupScore;
	public Transform PowerupResistance;
	public Transform PowerupLife;
	public Transform PowerupRandom;
	
	//GameObjects that the generator cares about
	public Rigidbody Player;
	
	//The list of blocks currently present. New blocks are added at the start, and old blocks are removed from the end
	private CircularLinkedList<IBlock> BlockList;

	private float blockSwapThreshold = 0.0f;		//The player x value at which the next block will be removed (needs rework if adding proper curving)
	private float nextBlockX = 0.0f;				//The world x value where which the next block will be placed
	private float currentHeight = 0.0f;				//the height offset at which new blocks are placed
	private int buildIndex = 0;						//the position in the level that the current block is being built at
	private bool leftEasy = false;					//if true, when building a branch, the left side is the low-risk low reward side. if false, right is
	public bool DestoryBlocks = true;
	public bool IgnoreThreshold = false;
	
	//Data playback
	public bool loadLevelFromRecording = false;
	private bool recordLevel = false;
	private BlockSerializer blockRecorder;
	private BlockDeserializer blockPlayback;
    private string playbackfile;
	private bool playbackbranchtrigger = false;
	public bool Newtutorial = false;
	public int LimitLengthByTileNumber;
	private int currentTileCount = 1;
	public bool LimitLengthByTile;
	public Transform backblock;
	private bool addTiles;
	
    // Variables for altering the environment
	public UrbanGenerator UrbanGen;
    public RuralGenerator RuralGen = null;
    public TerrainSelector terrainSelector = null;



    //testing time based spawning
    public int intervalTime = 10;
    private float _currentTime;
    private bool _startSpawned;
    private bool _endSpawned = false;
    private int _generatorType;

    public float maxRPM = 180;
    public float slowZoneSpeedRpm = 60;

    public int slowZoneLength = 8;
    public int fastZoneLength = 24;

	// Use this for initialization
	void Start ()
	{
        generatorMode = globalSettings.LevelType;
        
        switch (globalSettings.environmentType)
        {
            case (EnvironmentType.BARE):
                terrainSelector.DisableAllEnvironments();
                break;
            case (EnvironmentType.BARREN): 
                terrainSelector.EnableSandyTerrain();
                break;
            case (EnvironmentType.RURAL):
                terrainSelector.EnableGrassyTerrain();
                break;
            case (EnvironmentType.URBAN):
                terrainSelector.EnablePebbleTerrain();
                break;
        }
       
        recordLevel = globalSettings.RecordLevel;
        if (globalSettings.SimplePlaybackEnableGhost)
        {
            loadLevelFromRecording = true;
        }
        playbackfile = System.Environment.CurrentDirectory + "/WorldPlaybackData/Recording.egw";

		addTiles = true;
				
		BlockList = new CircularLinkedList<IBlock> ();
		if (loadLevelFromRecording) {
            Debug.Log("Loading level from recording");
			blockPlayback = new BlockDeserializer (playbackfile);
		}


        //dont know why i cant take out tutorial area without breaking game
		BuildTutorialArea ();
        if (recordLevel) {
			blockRecorder = new BlockSerializer ();
			blockRecorder.WriteTutorialData (BlockList);
		}
	}
	
	public void ClosePlaybackWriter ()
	{
		if (recordLevel && blockRecorder.isUsable) {
			blockRecorder.CloseBlockRecorder ();
		}
	}
	
	public void ClosePlaybackReader ()
	{
		if (loadLevelFromRecording && blockPlayback.isUsable) {
			blockPlayback.KillPlayback ();
		}
	}
	
	public void ReopenPlaybackReader ()
	{
		blockPlayback = new BlockDeserializer (playbackfile);
	}

	
	// Update is called once per frame
	void Update ()
    {
        //here we check the player's position, and if necessary remove the back block and add a new one
        if (Player.transform.position.x >= blockSwapThreshold || IgnoreThreshold) {
			//remove the oldest block
			DestroyLastBlock ();
            
            generatorMode = GeneratorMode.STANDARD;
            if (addTiles) // Make sure the generator is enabled.
            {
                if (generatorMode == GeneratorMode.STANDARD)
                {
                    //Add a new, random block
                    AddBlockStandardMode();
                }                
			}
			blockSwapThreshold += 20.0f;
			nextBlockX += 20.0f;
		}
	}

    void OnApplicationQuit()
    {
        ClosePlaybackReader();
        ClosePlaybackWriter();
    }

    void AddBlockStandardMode()
    {
        _currentTime = Time.time;
        if (_currentTime % (intervalTime * 2) < intervalTime)
        {
            _startSpawned = true;
            if (_endSpawned)
            {
                _generatorType = 2;
                _endSpawned = false;
            }
            else
            {
                _generatorType = 0;
            }
        }
        else if (_currentTime % (intervalTime * 2) > intervalTime)
        {
            _endSpawned = true;
            if (_startSpawned)
            {
                _generatorType = 1;
                _startSpawned = false;
            }
            else
            {
                _generatorType = 3;
            }
        }

        Block block = AddBasicBlock(new Vector3(nextBlockX, currentHeight, 0.0f), _generatorType);
        currentTileCount++;
    }

	/// <summary>
	/// Retrieves the vec3 location from string.
	/// </summary>
	/// <param name='data'>
	/// String that begins <BlockXYZ> and is splitable on the ':' character between the X/Y/Z values.
	/// </param>

	private Vector3 RetrieveVec3LocationFromString (string data)
	{
		string[] locationdata = data.Split (':');
		Vector3 location = new Vector3 (float.Parse (locationdata [1]), float.Parse (locationdata [2]), float.Parse (locationdata [3]));
		return location;
	}
	

	
	//Returns the location at which the player should reappear when they respawn
	//Also prepares that location for the player's respawn
	public Vector3 GetPlayerRespawnPosition ()
	{
		IBlock lastBlock = BlockList.Tail.Value;
		Vector3 spawnPos = lastBlock.GetSpawnPosition (Player.position);
		Transform platform = (Transform)(Instantiate (RespawnPlatform, new Vector3 (spawnPos.x, spawnPos.y - 0.45f, spawnPos.z), Quaternion.identity));
		lastBlock.AddSpawnPlatform (platform);
		return spawnPos;
	}
	
	//Adds a block with no special features
	private Block AddBasicBlock (Vector3 location, int type)
	{
        Transform track;
        Block block;
        bool spawnCrowd;


        float fastZoneLength = (maxRPM / 60f) * slowZoneLength;

        if (currentTileCount < fastZoneLength)
        {
            track = (Transform)(Instantiate(Straight, location, Quaternion.identity));
            block = new Block(track);
            BlockList.AddFirst(block);
            spawnCrowd = true;
        }
        
        else if (currentTileCount == fastZoneLength)
        {
            track = (Transform)(Instantiate(StraightBlockedStart, location, Quaternion.identity));
            block = new Block(track);
            BlockList.AddFirst(block);
            spawnCrowd = false;
        }
        else if (currentTileCount < currentTileCount+ fastZoneLength)
        {
            track = (Transform)(Instantiate(StraightBlockedStart, location, Quaternion.Euler(0,180,0)));
            block = new Block(track);
            BlockList.AddFirst(block);
            spawnCrowd = false;
        }
        else 
        {
            track = (Transform)(Instantiate(StraightBlockedZone, location, Quaternion.identity));
            block = new Block(track);
            BlockList.AddFirst(block);
            spawnCrowd = false;

            currentTileCount = 1;
        }
        switch (globalSettings.environmentType)
        {
            case (EnvironmentType.RURAL):
                RuralGen.RuralLandscapeGeneration(track.position, false, track);
                break;
            case (EnvironmentType.URBAN):
                UrbanGen.UrbanLandscapeGeneration(track.position, spawnCrowd, track);
                break;
        }
        return block;
	}

	public void BuildTutorialArea ()
	{
        
		//Start with 5 standard blocks, then a left pit, then a standard, then right, then a standard and a ramp down
		for (int i = 0; i < 20; i++) 
        {
			AddBasicBlock (new Vector3 (20.0f * i, 0.0f, 0.0f),0);
		}
        currentTileCount++;
        blockSwapThreshold = 60.0f;
		nextBlockX = 400.0f;
		buildIndex = 0;			//the first level begins at the end of the tutorial
		
	}
	
	public void ClearAllBlocks ()
	{
		while (BlockList.Count > 0) 
        {
			DestroyLastBlock ();
		}
		currentHeight = 0.0f;
	}
	
	private void DestroyLastBlock ()
	{
		if(DestoryBlocks)
        {
			IBlock last = BlockList.Tail.Value;
			BlockList.RemoveLast ();
			last.DestroyBlock ();
		}
	}
	//used by the spectator camera to force the game to generate all the blocks needed. 
	public void HijackBlockBuildThreshold(){
		DestoryBlocks = false;  // So we don't destroy any of the old blocks.
		IgnoreThreshold = true; // Make it huge so we never hit it under reasonable circumstances.
	}
	
	public string GetBlockInfo ()
	{
		//Block info key: D - drop, B - barrier, S - missing side, P - powerup, C - cannon, L - level change
		//First we need to determine the current location of the player
		//when they are at the block swap threshold, they are crossing from the end of the third block to the 4th
		//thus the player will always be in the third block except for the very start of the game
		int blockJumps = (int)((blockSwapThreshold - Player.position.x) / 20.0f);
		LinkedListNode<IBlock> currentNode = BlockList.Tail;
		for (int i = 0; i < blockJumps; i++) {
			currentNode = currentNode.Previous;	
		}
		IBlock currentBlock = currentNode.Value;
		return currentBlock.GetInfo (Player.position);
	}
}
