using UnityEngine;
using System.Collections;

//For the triggers that award points for avoiding obstacles
public class PointsAwardTrigger : MonoBehaviour 
{
    public PointsDenyTrigger[] DisableColliders; //Array of colliders that if the player hits them, this trigger will no longer give points

    private bool ableToGivePoints = true;   //Whether this trigger can still give points. Set to false if the player hits an associated obstacle, or if they get the points 

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (ableToGivePoints)
        {
            foreach (PointsDenyTrigger col in DisableColliders)
            {
                if (col.HitByPlayer)
                {
                    //Debug.Log("Disabling Award");
                    ableToGivePoints = false;
                    break;
                }
            }
        }
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && ableToGivePoints)
        {
            ableToGivePoints = false;
            other.GetComponent<PlayerController>().GivePoints(60);
        }
    }
}
