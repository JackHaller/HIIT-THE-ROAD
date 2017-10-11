using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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
        calibrationText = GameObject.FindGameObjectWithTag("CalibrationText");

        clapManager.SetActive(false);
        StartCoroutine(Calibrate());
    }
	
	// Update is called once per frame
	void Update () {
        var crowdSounds = GameObject.FindGameObjectsWithTag("CrowdSound");
        var crowds = GameObject.FindGameObjectsWithTag("Crowd");
        var powerUps = GameObject.FindGameObjectsWithTag("Powerup");

        foreach (var crowdSound in crowdSounds)
        {
            Destroy(crowdSound);
        }
        foreach(var crowd in crowds)
        {
            Destroy(crowd);
        }
        foreach (var powerUp in powerUps)
        {
            Destroy(powerUp);
        }
    }

    IEnumerator Calibrate()
    {
        //duplicate code for countdown since IEnumerator method inside IEnumerator doesn't seem to work
        bool done = false;
        var count = 10;
        while (count != 0)
        {
            calibrationText.GetComponentInChildren<Text>().text = "Calibration starting in " + count + ".";
            count--;
            yield return new WaitForSeconds(1f);
        }
        startHR = 0;
        endHR = 0;
        while (!done)
        {
            if (lowZone)
            {
                //60seconds includes warmup
                count = 60;
                while (count != 0)
                {
                    startHR = bikeController.heartRate > startHR ? bikeController.heartRate : startHR;
                    calibrationText.GetComponentInChildren<Text>().text = "Pedal at casual speed for " + count + " seconds!";
                    count--;
                    yield return new WaitForSeconds(1f);
                }
                lowZone = false;
            }
            else
            {
                count = 30;
                while (count != 0)
                {
                    endHR = bikeController.heartRate > endHR ? bikeController.heartRate : endHR;
                    calibrationText.GetComponentInChildren<Text>().text = "Pedal as fast as you can for " + count + " seconds!";
                    count--;
                    yield return new WaitForSeconds(1f);
                }
                done = true;
            }
        }

        Debug.Log(startHR);
        Debug.Log(endHR);

        count = 10;
        while (count != 0)
        {
            calibrationText.GetComponentInChildren<Text>().text = "End of Calibration. Moving to the main game in " + count + ".";
            count--;
            yield return new WaitForSeconds(1f);
        }
        SceneManager.LoadScene("Base");
    }
}
