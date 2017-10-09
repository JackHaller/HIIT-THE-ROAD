using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CalibrationManager : MonoBehaviour {

    public BikeController bikeController;

    private GameObject clapManager;
    private GameObject vehicleGenerator;
    private GameObject worldSpaceGUI;
    private GameObject calibrationText;
    private int startHR;
    private int endHR;
    private bool lowZone;

	// Use this for initialization
	void Start () {
        startHR = 0;
        endHR = 0;
        lowZone = true;

        clapManager = GameObject.FindGameObjectWithTag("ClapManager");
        vehicleGenerator = GameObject.FindGameObjectWithTag("VehicleGenerator");
        worldSpaceGUI = GameObject.FindGameObjectWithTag("WorldSpaceGUI");
        calibrationText = GameObject.FindGameObjectWithTag("CalibrationText");

        print(clapManager.activeSelf);

        clapManager.SetActive(false);
        vehicleGenerator.SetActive(false);
        worldSpaceGUI.SetActive(false);
        calibrationText.SetActive(false);
        print(clapManager.activeSelf);
        StartCoroutine(Calibrate());
    }
	
	// Update is called once per frame
	void Update () {
        var crowdSounds = GameObject.FindGameObjectsWithTag("CrowdSound");
        var crowds = GameObject.FindGameObjectsWithTag("Crowd");

        foreach (var crowdSound in crowdSounds)
        {
            crowdSound.SetActive(false);
        }
        foreach(var crowd in crowds)
        {
            crowd.gameObject.SetActive(false);
        }
    }

    IEnumerator Calibrate()
    {
        bool done = false;

        while(!done)
        {
            if (lowZone)
            {
                calibrationText.SetActive(true);
                calibrationText.GetComponentInChildren<Text>().text = "Pedal at 60rpm for 10 seconds!";
                yield return new WaitForSeconds(10f);
                startHR = bikeController.heartRate;
                lowZone = false;
                Debug.Log("lowzone");
            }
            else
            {
                calibrationText.GetComponentInChildren<Text>().text = "Pedal as fast as you can for 30 seconds!";
                yield return new WaitForSeconds(30f);
                endHR = bikeController.heartRate;
                done = true;
                Debug.Log("highzone");

            }
        }
        calibrationText.GetComponentInChildren<Text>().text = "HIIT THE ROAD!";
        yield return new WaitForSeconds(2f);
        StartGame();
    }
    

    void StartGame()
    {
        clapManager.SetActive(true);
        vehicleGenerator.SetActive(true);
        worldSpaceGUI.SetActive(true);
        calibrationText.SetActive(false);
    }
}
