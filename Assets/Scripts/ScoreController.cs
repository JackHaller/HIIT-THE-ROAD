using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreController : MonoBehaviour {

    public BikeController bikeManager;
    public PlayerController player;
    public bool highSpeedZone = true;

    public float maxHR;

    private float currentTime;
    private float lastTime;

    // Use this for initialization
    void Start () {
        //this should be taken from config from calibration scene
        maxHR = 170;
        lastTime = Time.time;
    }
	
    void FixedUpdate()
    {
        currentTime = Time.time;
        if (highSpeedZone && currentTime >= lastTime + 1)
        {
            if (bikeManager.heartRate != 0f)
            {
                if ((maxHR - bikeManager.heartRate) / 2f > 5f)
                {
                    player.TakePoints((int)((maxHR * 1.2f - bikeManager.heartRate) / 2f));
                }
                else
                {
                    player.TakePoints(5);
                }
                lastTime = currentTime;
            }
            else
            {
                player.TakePoints(100);
            }            
        }
    }
}
