using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HRBarController : MonoBehaviour {

	private int lowBeat = 0; 
	private int highBeat = 200;
	private int exactTarget = 100;

    private static float BAR_MOVEMENT = 31.0f;  //the distance the gradient bar must be translated to go from centred to the green-red transition point

	private int currentHR = 100;

	public float scoreMultiplier = 1.0f;

    public BikeController bikeController;
    public Text ScoreMultiplierText;
    public IntervalController intervalController;

	public Image Bar;
	public Sprite Gradient;
	public Sprite Grey;

	public void SetZoneHRParameters(int lowBeat, int highBeat)
	{
		this.lowBeat = lowBeat;
		this.highBeat = highBeat;
		exactTarget = (highBeat + lowBeat) / 2;
	}
	
	// Update is called once per frame
	void Update () 
    {
		//get current heart rate
        currentHR = bikeController.heartRate;
		if (currentHR == 0)
		{
			//no HR data from the bike, grey out the bar
			Bar.sprite = Grey;
			Bar.rectTransform.localPosition = Vector3.zero;	//reset the position in case of sudden loss of heart rate data
			SetMultiplier(1.0f);	//if we have no heart rate data don't mess with the score
		}
		else
		{
			Bar.sprite = Gradient;
			//adjust the position of the sprite based on the target
			float difference = (float)(exactTarget - lowBeat);
            float unitsPerHeartBeat = BAR_MOVEMENT / difference;
			float currentDifference = (float)(exactTarget - currentHR);	//number of heart beats from target to current (et - chr because when currentHR is too low, posX must increase)
			float movementOffset = unitsPerHeartBeat * currentDifference;
			if (movementOffset > 50.0f)
			{
				movementOffset = 50.0f;
			}
			else if (movementOffset < -50.0f)
			{
				movementOffset = -50.0f;
			}
			Bar.rectTransform.localPosition = new Vector3(movementOffset, 0.0f, 0.0f);
			//now calculate what kind of multiplier this gives for the score (in zone: 3x, near zone: 2x, far from zone, 1x)
            if (intervalController.intervalState == IntervalController.IntervalState.WARMUP || intervalController.intervalState == IntervalController.IntervalState.NO_INTERVAL)
            {
                SetMultiplier(1.0f);
            }
            else
            {
                if (currentHR >= lowBeat && currentHR <= highBeat)
                {
                    SetMultiplier(3.0f);
                }
                else if (currentHR >= lowBeat * 0.8f && currentHR <= highBeat * 1.2f)
                {
                    SetMultiplier(2.0f);
                }
                else
                {
                    SetMultiplier(1.0f);
                }
            }
		}
	}

    void SetMultiplier(float multiplier)
    {
        scoreMultiplier = multiplier;
        ScoreMultiplierText.text = string.Format("{0}x", (int)multiplier);
    }
}
