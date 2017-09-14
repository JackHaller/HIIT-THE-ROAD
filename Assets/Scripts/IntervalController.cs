using UnityEngine;
using System.Collections;

public class IntervalController : MonoBehaviour {
    
    public enum IntervalState
    {
        WARMUP = 0,
        INTERVAL = 1,
        RECOVERY = 2,
        TRANSITION_TO_INTERVAL = 3, //these two states are used to make the generator start building blocks of a given state shortly before the gameplay actually switches to that state
        TRANSITION_TO_RECOVERY = 4,
        NO_INTERVAL = 5,
    }

    public GlobalSettings globalSettings;
    public HRBarController barController;

    public IntervalState intervalState;

    private float elapsedTimeInCurrentState = 0.0f;

    private float warmupDuration;
    private float intervalDuration;
    private float recoveryDuration;

    private int HRMax;
    private int HRLow;
    private int HRHigh;

	// Use this for initialization
	void Start () {
        elapsedTimeInCurrentState = 0.0f;
        intervalState = globalSettings.UsingIntervals ? IntervalState.WARMUP : IntervalState.NO_INTERVAL;

		if(!globalSettings.UsingIntervals){
			barController.transform.gameObject.SetActive(false);
		}

        warmupDuration = 60.0f * (float)globalSettings.WarmupMinutes;
        intervalDuration = 60.0f * (float)globalSettings.IntervalMinutes;
        recoveryDuration = 60.0f * (float)globalSettings.RecoveryMinutes;
        HRMax = 208 - (int)(0.7f * (float)globalSettings.PlayerAge);  //maximum heart rate calculated as: 208 - 0.7 * age (Tanaka et al)
        HRLow = 0;          //We start in a warmup, where it doesn't matter what their HR is
        HRHigh = HRMax;
        barController.SetZoneHRParameters(HRLow, HRHigh);
	}

	
	// Update is called once per frame
	void Update () 
    {
        elapsedTimeInCurrentState += Time.deltaTime;

        if (intervalState == IntervalState.WARMUP)
        {
            if (elapsedTimeInCurrentState >= warmupDuration)
            {
                Debug.Log("Swapping to Interval State");
                intervalState = IntervalState.INTERVAL;
                elapsedTimeInCurrentState = 0.0f;
                HRLow = (int)(0.9f * (float)HRMax);
                HRHigh = (int)(0.95f * (float)HRMax);
                barController.SetZoneHRParameters(HRLow, HRHigh);
            }
        }
        else if (intervalState == IntervalState.INTERVAL || intervalState == IntervalState.TRANSITION_TO_RECOVERY)
        {
            if (elapsedTimeInCurrentState >= intervalDuration)
            {
                Debug.Log("Swapping to Recovery State");
                intervalState = IntervalState.RECOVERY;
                elapsedTimeInCurrentState = 0.0f;
                HRLow = (int)(0.5f * (float)HRMax);
                HRHigh = (int)(0.75f * (float)HRMax);
                barController.SetZoneHRParameters(HRLow, HRHigh);
            }
            else if (elapsedTimeInCurrentState >= intervalDuration - 10)
            {
                intervalState = IntervalState.TRANSITION_TO_RECOVERY;
            }
        }
        else if (intervalState == IntervalState.RECOVERY || intervalState == IntervalState.TRANSITION_TO_INTERVAL)
        {
            if (elapsedTimeInCurrentState >= recoveryDuration)
            {
                Debug.Log("Swapping to Interval State");
                intervalState = IntervalState.INTERVAL;
                elapsedTimeInCurrentState = 0.0f;
                HRLow = (int)(0.9f * (float)HRMax);
                HRHigh = (int)(0.95f * (float)HRMax);
                barController.SetZoneHRParameters(HRLow, HRHigh);
            }
            else if (elapsedTimeInCurrentState >= recoveryDuration - 10)
            {
                intervalState = IntervalState.TRANSITION_TO_INTERVAL;
            }
        }
        else
        {
            //Nothing here, for NO_INTERVAL
        }
	}
}
