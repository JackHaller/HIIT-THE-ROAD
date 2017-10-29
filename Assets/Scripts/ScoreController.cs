using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreController : MonoBehaviour {

    public BikeController bikeManager;
    public PlayerController player;
    public GlobalSettings globalSettings;
    public bool highSpeedZone = true;

    public int maxHR;

    private float currentTime;
    private float lastTime;

    // Use this for initialization
    void Start () {
        //this should be taken from config from calibration scene
        maxHR = globalSettings.MaxHR;
        lastTime = 6;
    }
	
    void FixedUpdate()
    {
        currentTime = Time.time;
        if (highSpeedZone && currentTime >= lastTime + 1)
        {
            if (bikeManager.heartRate != 0)
            {
                //calculating points to deduct based on current HR
                if ((maxHR - bikeManager.heartRate) / 2 > 5)
                {
                    player.TakePoints((int)((maxHR * 1.2 - bikeManager.heartRate) / 2));
                }
                else
                {
                    player.TakePoints(5);
                }                
            }
            else
            {
                //100 points deducted when HR not detected
                player.TakePoints(100);
            }
            lastTime = currentTime;
        }
    }
}
