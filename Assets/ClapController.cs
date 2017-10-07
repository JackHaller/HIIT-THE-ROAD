using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClapController : MonoBehaviour {

    public BikeController bike;
    public AudioClip[] claps;
    private bool clap = true;
    private bool cheer = false;

    // Use this for initialization
    void Start () {

	}
	
	// Update is called once per frame
	void Update () {
        GameObject[] crowdSounds = GameObject.FindGameObjectsWithTag("CrowdSound");
        GameObject[] crowds = GameObject.FindGameObjectsWithTag("Crowd");


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

        if (moveVertical > 1.0f && clap)
        {
            Debug.Log("Clap");
            foreach (GameObject crowd in crowds)
            {=
                crowd.GetComponent<Animator>().SetBool("Clap", true);
                crowd.GetComponent<Animator>().SetBool("Cheer", false);
            }
            clap = false;
            cheer = true;
        }
        else if (moveVertical < 1.0f && cheer)
        {
            foreach (GameObject crowd in crowds)
            {
                crowd.GetComponent<Animator>().SetBool("Cheer", true);
                crowd.GetComponent<Animator>().SetBool("Clap", false);
            }
            cheer = false;
            clap = true;
        }


        if (moveVertical > 3.0f && !crowdSounds[5].GetComponents<AudioSource>()[1].isPlaying)
        {
            foreach (GameObject crowdSound in crowdSounds)
            {
                crowdSound.GetComponents<AudioSource>()[1].clip = claps[10];
                crowdSound.GetComponents<AudioSource>()[1].Play();
            }            
        }
        else if (moveVertical > 2.83f && !crowdSounds[5].GetComponents<AudioSource>()[1].isPlaying)
        {
            foreach (GameObject crowdSound in crowdSounds)
            {
                crowdSound.GetComponents<AudioSource>()[1].clip = claps[9];
                crowdSound.GetComponents<AudioSource>()[1].Play();
            }
        }
        else if (moveVertical > 2.66f && !crowdSounds[5].GetComponents<AudioSource>()[1].isPlaying)
        {
            foreach (GameObject crowdSound in crowdSounds)
            {
                crowdSound.GetComponents<AudioSource>()[1].clip = claps[8];
                crowdSound.GetComponents<AudioSource>()[1].Play();
            }
        }
        else if (moveVertical > 2.17f && !crowdSounds[5].GetComponents<AudioSource>()[1].isPlaying)
        {
            foreach (GameObject crowdSound in crowdSounds)
            {
                crowdSound.GetComponents<AudioSource>()[1].clip = claps[7];
                crowdSound.GetComponents<AudioSource>()[1].Play();
            }
        }
        else if (moveVertical > 2.0f && !crowdSounds[5].GetComponents<AudioSource>()[1].isPlaying)
        {
            foreach (GameObject crowdSound in crowdSounds)
            {
                crowdSound.GetComponents<AudioSource>()[1].clip = claps[6];
                crowdSound.GetComponents<AudioSource>()[1].Play();
            }
        }
        else if (moveVertical > 1.83f && !crowdSounds[5].GetComponents<AudioSource>()[1].isPlaying)
        {
            foreach (GameObject crowdSound in crowdSounds)
            {
                crowdSound.GetComponents<AudioSource>()[1].clip = claps[5];
                crowdSound.GetComponents<AudioSource>()[1].Play();
            }
        }
        else if (moveVertical > 1.66f && !crowdSounds[5].GetComponents<AudioSource>()[1].isPlaying)
        {
            foreach (GameObject crowdSound in crowdSounds)
            {
                crowdSound.GetComponents<AudioSource>()[1].clip = claps[4];
                crowdSound.GetComponents<AudioSource>()[1].Play();
            }
        }
        else if (moveVertical > 1.5f && !crowdSounds[5].GetComponents<AudioSource>()[1].isPlaying)
        {
            foreach (GameObject crowdSound in crowdSounds)
            {
                crowdSound.GetComponents<AudioSource>()[1].clip = claps[3];
                crowdSound.GetComponents<AudioSource>()[1].Play();
            }
        }
        else if (moveVertical > 1.33f && !crowdSounds[5].GetComponents<AudioSource>()[1].isPlaying)
        {
            foreach (GameObject crowdSound in crowdSounds)
            {
                crowdSound.GetComponents<AudioSource>()[1].clip = claps[2];
                crowdSound.GetComponents<AudioSource>()[1].Play();
            }
        }
        else if (moveVertical > 1.17f && !crowdSounds[5].GetComponents<AudioSource>()[1].isPlaying)
        {
            foreach (GameObject crowdSound in crowdSounds)
            {
                crowdSound.GetComponents<AudioSource>()[1].clip = claps[1];
                crowdSound.GetComponents<AudioSource>()[1].Play();
            }
        }
        else if (moveVertical > 1.0f && !crowdSounds[5].GetComponents<AudioSource>()[1].isPlaying)
        {
            foreach (GameObject crowdSound in crowdSounds)
            {
                crowdSound.GetComponents<AudioSource>()[1].clip = claps[0];
                crowdSound.GetComponents<AudioSource>()[1].Play();
            }
        }
    }
}
