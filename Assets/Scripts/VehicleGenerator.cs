using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleGenerator : MonoBehaviour {

    public GameObject[] vehicles;
    public Material[] vehiclesColours;
    public float spawnTime;
    public float distance;
    private float _maxVehicle = 100;
    private float[] _carZPositions;
    private bool _playing;
    private GameObject _player;
    private GameObject _latestVehicle;
    private bool endOfSession = false;
    
    // Use this for initialization
    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _carZPositions = new float[] { -2.7f, 0.1f, 2.9f };
        _playing = true;
        StartCoroutine("VehicleSpawner");

    }

    void Update()
    {
        //stops spawning vehicles at the end of the game
        if(endOfSession)
        {
            StopCoroutine("VehicleSpawner");
        }
    }

    IEnumerator VehicleSpawner()
    {
        GameObject newVehicle = null;
        GameObject vehicle;
        int oldPositionIndex = 0;
        int newPositionIndex = 0;
        int colourIndex = 0;
        int vehicleIndex = 0;
        Vector3 vehiclePos;
        var vehicleRot = Quaternion.Euler(0, 90f, 0);

        while (_playing)
        {
            //spawn vehicles every 0.5 seconds in front the player
            yield return new WaitForSeconds(spawnTime);

            //only spawn vehicles if max number hasnt been reached
            var activeVehicles = GameObject.FindGameObjectsWithTag("Vehicle");
            if (activeVehicles.Length <= _maxVehicle)
            {
                //randomized spawning of vehicle type, number and position
                int vehicleNum = Random.Range(1, 3);
                for (int i = 0; i < vehicleNum; i++)
                {
                    while (oldPositionIndex == newPositionIndex)
                        newPositionIndex = Random.Range(0, _carZPositions.Length);
                    vehicleIndex = Random.Range(0, vehicles.Length);
                    colourIndex = Random.Range(0, vehiclesColours.Length);
                    vehicle = vehicles[vehicleIndex];

                    if (_latestVehicle == null)
                        vehiclePos = new Vector3(_player.transform.position.x + 10f, 0.5f, _carZPositions[newPositionIndex]);
                    else
                        vehiclePos = new Vector3(_latestVehicle.transform.position.x + distance, 0.5f, _carZPositions[newPositionIndex]);

                    newVehicle = Instantiate(vehicle, vehiclePos, vehicleRot);
                    newVehicle.GetComponent<Renderer>().material = vehiclesColours[colourIndex];
                    oldPositionIndex = newPositionIndex;
                }
            }

            _latestVehicle = newVehicle;
        }
    }

    public void SetEndOfSession(bool endOfSession)
    {
        this.endOfSession = endOfSession;
    }
}
