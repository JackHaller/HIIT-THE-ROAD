using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClapController : MonoBehaviour {

    public BikeController bike;
    

    // Use this for initialization
    void Start () {

	}
	
	// Update is called once per frame
	void Update () {
        GameObject[] claps = GameObject.FindGameObjectsWithTag("CrowdSound");

        //Handle vertical movement. Priority is Bike > Keyboard
        float moveVertical = 0.0f;
        if (bike.enableBike)
        {
            moveVertical = bike.speed;
        }
        else
        {
            moveVertical = Input.GetAxis("Vertical");
        }

        //use shift keys to move fast when testing with the keyboard
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            moveVertical *= 2;
        }

        if (moveVertical > 1.0f && !claps[5].GetComponents<AudioSource>()[1].isPlaying)
        {
            foreach (GameObject clap in claps)
            {
                clap.GetComponents<AudioSource>()[1].Play();
            }            
        } 
	}
}
