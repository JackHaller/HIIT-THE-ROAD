using UnityEngine;
using System.Collections;

//attached to obstacles that the player passes through, in order to remove points from them and track that
//they failed to avoid the obstacle so that they don't get the obstacle avoidance points
public class PointsDenyTrigger : MonoBehaviour {

    public bool HitByPlayer { get; private set; }

    private float pingTime = 0.5f;

    private float timeSinceLastPing = 0.0f;

    private bool colliding = false;

    private bool pointsTaken = false;

	// Use this for initialization
	void Start () {
        HitByPlayer = false;
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && !HitByPlayer)
        {
            colliding = true;
            HitByPlayer = true;
            other.GetComponent<PlayerController>().TakePoints(20);
        }
        else if (other.tag == "PlayerHead" && !HitByPlayer)
        {
            colliding = true;
            HitByPlayer = true;
            GameObject.Find("Player").GetComponent<PlayerController>().TakePoints(20);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            colliding = false;
        }
        else if (other.tag == "PlayerHead")
        {
            colliding = false;
        }
    }

    void Update()
    {
        if (colliding)
        {
            timeSinceLastPing += Time.deltaTime;
            if (timeSinceLastPing >= pingTime)
            {
                GameObject.Find("Player").GetComponent<PlayerController>().TakePoints(20);
                timeSinceLastPing = 0.0f;
            }
        }
    }
}