using UnityEngine;
using System.Collections;

public class SpawnedObjectLerper : MonoBehaviour {

    Vector3 finalPosition;

    private float startTime;
    private float journeyTime = 0.0018f; // inverse
    private float lerpAmount = 0f;

    public void Initialise(float yDistanceToLerp, float zDistanceToLerp)
    {
        startTime = Time.time;

        //if (isRight)
        //{
        //    finalPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z - distanceToLerp);
        //}

        finalPosition = new Vector3(transform.position.x, transform.position.y + yDistanceToLerp, transform.position.z + zDistanceToLerp);
    }

    void Update()
    {
        if (lerpAmount < 1f)
        {
            //Debug.Log("Time: " + Time.time + " startTime: " + startTime);
            lerpAmount = (Time.time - startTime) * journeyTime;
            //Debug.Log("LerpAmount: " + lerpAmount);

            transform.position = Vector3.Lerp(transform.position, finalPosition, lerpAmount);
        }
    }
}
