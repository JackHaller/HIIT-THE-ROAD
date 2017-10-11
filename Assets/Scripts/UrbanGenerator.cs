using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class UrbanGenerator : MonoBehaviour {

	public List<GameObject> BuildingList = new List<GameObject>();
	private float currentXOfPlayer;
	public float ChanceOfBuildingSpawn;
	private int Density = 3;
    private int _initialSpacing = 20;
    private Transform _parentTransform = null;
    private float[] carPositions;

    public int crowdDensity = 50;
    public GameObject crowdList;
    public GameObject crowdAudio;
    public GameObject clapManager;
    private bool spawnAudio;


    private float[] powerUpLocations = new float[] { -2.7f, 0.1f, 2.9f };
    public GameObject powerUp;
    public int powerUpSpawnChance = 1;

public void UrbanLandscapeGeneration(Vector3 trackLocation, bool spawnCrowd, Transform parentTransform)
    {
        _parentTransform = parentTransform;


		CreateLeftBuildingCluster(trackLocation, spawnCrowd);
		CreateRightBuildingCluster(trackLocation, spawnCrowd);

        if(Random.Range(0,powerUpSpawnChance) == 0)
        {
            GameObject powerUpPostion = Instantiate(powerUp);
            powerUpPostion.transform.position = new Vector3(trackLocation.x, trackLocation.y + 1f, powerUpLocations[Random.Range(0, 3)]);
        }
        
	}
	
	private void CreateLeftBuildingCluster (Vector3 trackLocation, bool spawnCrowd) {

        // Add an extra tile to hide visible grass -- issue due to moving buildings far enough away from the track to clear the cannons
        GameObject newTile1 = Instantiate(BuildingList[Random.Range(0,BuildingList.Count-1)], new Vector3(trackLocation.x, trackLocation.y - 0.01f, trackLocation.z - 15.0f), Quaternion.identity) as GameObject;
        GameObject newTile2 = Instantiate(BuildingList[Random.Range(0, BuildingList.Count - 1)], new Vector3(trackLocation.x, trackLocation.y - 0.01f, trackLocation.z - 25.0f), Quaternion.identity) as GameObject;
        newTile1.transform.parent = _parentTransform;
        newTile2.transform.parent = _parentTransform;

        if(spawnCrowd)
        {        
            GameObject crowdSound = Instantiate(crowdAudio);
            crowdSound.transform.position = new Vector3(trackLocation.x + Random.Range(0, 20), trackLocation.y + 0.5f, trackLocation.z + Random.Range(0, 3) + 7);
            crowdSound.transform.parent = _parentTransform;

            int crowdCount = 0;
            int breakCount = 0;
            List<Vector3> crowdPositons = new List<Vector3>();
            while (crowdCount < crowdDensity && breakCount <20)
            {
                Vector3 currentPostion = new Vector3(trackLocation.x + Random.Range(0, 20), trackLocation.y + 0.5f, trackLocation.z + Random.Range(0, 3) + 7);
                breakCount++;
                if (!crowdPositons.Contains(currentPostion))
                {
                    crowdCount++;
                    breakCount = 0;
                    crowdPositons.Add(currentPostion);
                    GameObject crowdTest = Instantiate(crowdList);
                    crowdTest.transform.GetChild(Random.Range(0, 8)).gameObject.SetActive(true);
                    crowdTest.transform.position = currentPostion;
                    crowdTest.transform.rotation = Quaternion.Euler(0, 180, 0);
                    crowdTest.transform.parent = _parentTransform;

                }
            }
        }

        Vector3 location = new Vector3(trackLocation.x, trackLocation.y, trackLocation.z - _initialSpacing); // difference of 3
		
		for(int i = 0; i < Density; i++){
			AddTile(location, false);
			location.z = location.z - 20.0f;
		}
	}
	
	private void CreateRightBuildingCluster (Vector3 trackLocation, bool spawnCrowd) {

        // Add an extra tile to hide visible grass -- issue due to moving buildings far enough away from the track to clear the cannons
        GameObject newTile1 = Instantiate(BuildingList[Random.Range(0, BuildingList.Count - 1)], new Vector3(trackLocation.x, trackLocation.y - 0.01f, trackLocation.z + 15.0f), Quaternion.identity) as GameObject;
        GameObject newTile2 = Instantiate(BuildingList[Random.Range(0, BuildingList.Count - 1)], new Vector3(trackLocation.x, trackLocation.y - 0.01f, trackLocation.z + 25.0f), Quaternion.identity) as GameObject;
        newTile1.transform.parent = _parentTransform;
        newTile2.transform.parent = _parentTransform;
        newTile1.transform.rotation = Quaternion.Euler(0, 180, 0);
        newTile2.transform.rotation = Quaternion.Euler(0, 180, 0);


        if (spawnCrowd)
        {
            GameObject crowdSound = Instantiate(crowdAudio);
            crowdSound.transform.position = new Vector3(trackLocation.x + Random.Range(0, 20), trackLocation.y + 0.5f, trackLocation.z + Random.Range(0, 3) - 9);
            crowdSound.transform.parent = _parentTransform;


            int crowdCount = 0;
            int breakCount = 0;
            List<Vector3> crowdPositons = new List<Vector3>();
            while (crowdCount < crowdDensity && breakCount < 20)
            {
                Vector3 currentPostion = new Vector3(trackLocation.x + Random.Range(0, 20), trackLocation.y + 0.5f, trackLocation.z + Random.Range(0, 3) - 9);
                breakCount++;
                if (!crowdPositons.Contains(currentPostion))
                {
                    crowdCount++;
                    breakCount = 0;
                    crowdPositons.Add(currentPostion);
                    GameObject crowdTest = Instantiate(crowdList);
                    crowdTest.transform.GetChild(Random.Range(0, 8)).gameObject.SetActive(true);
                    crowdTest.transform.position = currentPostion;
                    crowdTest.transform.rotation = Quaternion.Euler(0, 0, 0);
                    crowdTest.transform.parent = _parentTransform;

                }
            }
        }


        Vector3 location = new Vector3(trackLocation.x, trackLocation.y, trackLocation.z + _initialSpacing);
		
		for(int i = 0; i < Density; i++){
			AddTile(location, true);
			location.z = location.z + 20.0f;
		}
	}
	
	private void AddTile (Vector3 location, bool isRight) {

        GameObject newTile = Instantiate(Resources.Load("Tile"), location, Quaternion.identity) as GameObject;
        newTile.transform.parent = _parentTransform;
       
		var test = Random.Range(0.0f, 1.0f);
		if(test < ChanceOfBuildingSpawn)
		{
			AddBuilding(new Vector3(location.x - 5.0f, location.y, location.z - 5.005f), isRight); // Add a slight stagger to stop shimmer from overlapping textures.
		}
		test = Random.Range(0.0f, 1.0f);
		if(test < ChanceOfBuildingSpawn)
		{
			AddBuilding(new Vector3(location.x + 5.0f, location.y, location.z - 4.095f), isRight);
		}
		test = Random.Range(0.0f, 1.0f);
		if(test < ChanceOfBuildingSpawn)
		{
			AddBuilding(new Vector3(location.x - 5.0f, location.y, location.z + 5.005f), isRight);
		}
		test = Random.Range(0.0f, 1.0f);
		if(test < ChanceOfBuildingSpawn)
		{
			AddBuilding(new Vector3(location.x + 5.0f, location.y, location.z + 4.095f), isRight);
		}
    }

	// Adds Building to certain location in scene
	private void AddBuilding (Vector3 location, bool isRight) {
		int indexOfBuilding = Random.Range (0, (BuildingList.Count - 1));
		
		if (isRight) {
            if (Time.time != 0) // Does not add script if part of initial setup.
            {
                Vector3 spawnLocation = new Vector3(location.x, location.y - 30f, location.z + 15f);
                //Transform newBuilding = Instantiate(BuildingList[indexOfBuilding], spawnLocation, Quaternion.identity) as Transform;
                //newBuilding.transform.parent = _parentTransform;
                //SpawnedObjectLerper lerper = newBuilding.gameObject.AddComponent<SpawnedObjectLerper>();
                //lerper.Initialise(30f, -15f);
            }
            else {
                //Transform newBuilding = Instantiate(BuildingList[indexOfBuilding], location, Quaternion.identity) as Transform;
                //newBuilding.transform.parent = _parentTransform;
            }

		} else {
            if (Time.time != 0)
            {
                Vector3 spawnLocation = new Vector3(location.x, location.y - 30f, location.z - 15f);
                //Transform newBuilding = Instantiate(BuildingList[indexOfBuilding], spawnLocation, Quaternion.Euler(0, 180, 0)) as Transform;
                //newBuilding.transform.parent = _parentTransform;
                //SpawnedObjectLerper lerper = newBuilding.gameObject.AddComponent<SpawnedObjectLerper>();
                //lerper.Initialise(30f, 15f);
            }
            else
            {
                //Transform newBuilding = Instantiate(BuildingList[indexOfBuilding], location, Quaternion.Euler(0, 180, 0)) as Transform;
                //newBuilding.transform.parent = _parentTransform;
            }
		}
	}
}
