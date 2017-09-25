using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleGenerator : MonoBehaviour {

    public GameObject[] vehicles;
    public Material[] vehiclesColours;
    public float spawnTime;
    public float distance;
    private float[] _carZPositions;
    private bool _playing;
    private GameObject _player;
    private GameObject _latestVehicle;
    
    // Use this for initialization
    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _carZPositions = new float[] { -2.7f, 0.1f, 2.9f };
        _playing = true;
        StartCoroutine(VehicleSpawner());

    }

    // Update is called once per frame
    void Update () {

	}

    IEnumerator VehicleSpawner()
    {
        while(_playing)
        {
            yield return new WaitForSeconds(spawnTime);
            var vehicleIndex = Random.Range(0, vehicles.Length);
            var colourIndex = Random.Range(0, vehiclesColours.Length);
            var zPositionIndex = Random.Range(0, _carZPositions.Length);
            var vehicle = vehicles[vehicleIndex];

            Vector3 vehiclePos;
            if (_latestVehicle == null)
                vehiclePos = new Vector3(_player.transform.position.x + 50f, 0.5f, _carZPositions[zPositionIndex]);
            else
                vehiclePos = new Vector3(_latestVehicle.transform.position.x + distance, 0.5f, _carZPositions[zPositionIndex]);

            var vehicleRot = Quaternion.Euler(0, 90f, 0);
            var newVehicle = Instantiate(vehicle, vehiclePos, vehicleRot);
            newVehicle.GetComponent<Renderer>().material = vehiclesColours[colourIndex];

            _latestVehicle = newVehicle;
        }
    }
}
