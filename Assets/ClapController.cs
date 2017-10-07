using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClapController : MonoBehaviour {

    public BikeController bike;
    public AudioClip[] claps;

    // Use this for initialization
    void Start () {

	}
	
	// Update is called once per frame
	void Update () {
        GameObject[] crowds = GameObject.FindGameObjectsWithTag("CrowdSound");

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

        if (moveVertical > 3.0f && !crowds[5].GetComponents<AudioSource>()[1].isPlaying)
        {
            foreach (GameObject crowd in crowds)
            {
                crowd.GetComponents<AudioSource>()[1].clip = claps[10];
                crowd.GetComponents<AudioSource>()[1].Play();
            }            
        }
        else if (moveVertical > 2.83f && !crowds[5].GetComponents<AudioSource>()[1].isPlaying)
        {
            foreach (GameObject crowd in crowds)
            {
                crowd.GetComponents<AudioSource>()[1].clip = claps[9];
                crowd.GetComponents<AudioSource>()[1].Play();
            }
        }
        else if (moveVertical > 2.66f && !crowds[5].GetComponents<AudioSource>()[1].isPlaying)
        {
            foreach (GameObject crowd in crowds)
            {
                crowd.GetComponents<AudioSource>()[1].clip = claps[8];
                crowd.GetComponents<AudioSource>()[1].Play();
            }
        }
        else if (moveVertical > 2.17f && !crowds[5].GetComponents<AudioSource>()[1].isPlaying)
        {
            foreach (GameObject crowd in crowds)
            {
                crowd.GetComponents<AudioSource>()[1].clip = claps[7];
                crowd.GetComponents<AudioSource>()[1].Play();
            }
        }
        else if (moveVertical > 2.0f && !crowds[5].GetComponents<AudioSource>()[1].isPlaying)
        {
            foreach (GameObject crowd in crowds)
            {
                crowd.GetComponents<AudioSource>()[1].clip = claps[6];
                crowd.GetComponents<AudioSource>()[1].Play();
            }
        }
        else if (moveVertical > 1.83f && !crowds[5].GetComponents<AudioSource>()[1].isPlaying)
        {
            foreach (GameObject crowd in crowds)
            {
                crowd.GetComponents<AudioSource>()[1].clip = claps[5];
                crowd.GetComponents<AudioSource>()[1].Play();
            }
        }
        else if (moveVertical > 1.66f && !crowds[5].GetComponents<AudioSource>()[1].isPlaying)
        {
            foreach (GameObject crowd in crowds)
            {
                crowd.GetComponents<AudioSource>()[1].clip = claps[4];
                crowd.GetComponents<AudioSource>()[1].Play();
            }
        }
        else if (moveVertical > 1.5f && !crowds[5].GetComponents<AudioSource>()[1].isPlaying)
        {
            foreach (GameObject crowd in crowds)
            {
                crowd.GetComponents<AudioSource>()[1].clip = claps[3];
                crowd.GetComponents<AudioSource>()[1].Play();
            }
        }
        else if (moveVertical > 1.33f && !crowds[5].GetComponents<AudioSource>()[1].isPlaying)
        {
            foreach (GameObject crowd in crowds)
            {
                crowd.GetComponents<AudioSource>()[1].clip = claps[2];
                crowd.GetComponents<AudioSource>()[1].Play();
            }
        }
        else if (moveVertical > 1.17f && !crowds[5].GetComponents<AudioSource>()[1].isPlaying)
        {
            foreach (GameObject crowd in crowds)
            {
                crowd.GetComponents<AudioSource>()[1].clip = claps[1];
                crowd.GetComponents<AudioSource>()[1].Play();
            }
        }
        else if (moveVertical > 1.0f && !crowds[5].GetComponents<AudioSource>()[1].isPlaying)
        {
            foreach (GameObject crowd in crowds)
            {
                crowd.GetComponents<AudioSource>()[1].clip = claps[0];
                crowd.GetComponents<AudioSource>()[1].Play();
            }
        }
    }
}
