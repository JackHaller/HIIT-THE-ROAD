using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClapController : MonoBehaviour {

    public BikeController bike;
    public AudioClip[] claps;
    public GlobalSettings globalSettings;

    private int maxRPM = 100;
    private bool clap = true;
    private bool cheer = false;
    private float maxSpeed;

    // Use this for initialization
    void Start () {
        maxRPM = globalSettings.MaxRPM;
        maxSpeed = maxRPM / 60;
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

        //70% rpm threshold to hear the cheering sound
        if (moveVertical > 0.70f * maxSpeed && clap)
        {
            foreach (GameObject crowd in crowds)
            {
                crowd.GetComponent<Animator>().SetBool("Clap", true);
                crowd.GetComponent<Animator>().SetBool("Cheer", false);
            }
            clap = false;
            cheer = true;
        }
        else if (moveVertical < 0.70f * maxSpeed && cheer)
        {
            foreach (GameObject crowd in crowds)
            {
                crowd.GetComponent<Animator>().SetBool("Cheer", true);
                crowd.GetComponent<Animator>().SetBool("Clap", false);
            }
            cheer = false;
            clap = true;
        }

        //clapping beat manager
        if (moveVertical > 1.15f * maxSpeed && !crowdSounds[5].GetComponents<AudioSource>()[1].isPlaying)
        {
            foreach (GameObject crowdSound in crowdSounds)
            {
                crowdSound.GetComponents<AudioSource>()[1].clip = claps[10];
                crowdSound.GetComponents<AudioSource>()[1].Play();
            }            
        }
        else if (moveVertical > 1.10f * maxSpeed && !crowdSounds[5].GetComponents<AudioSource>()[1].isPlaying)
        {
            foreach (GameObject crowdSound in crowdSounds)
            {
                crowdSound.GetComponents<AudioSource>()[1].clip = claps[9];
                crowdSound.GetComponents<AudioSource>()[1].Play();
            }
        }
        else if (moveVertical > 1.05f * maxSpeed && !crowdSounds[5].GetComponents<AudioSource>()[1].isPlaying)
        {
            foreach (GameObject crowdSound in crowdSounds)
            {
                crowdSound.GetComponents<AudioSource>()[1].clip = claps[8];
                crowdSound.GetComponents<AudioSource>()[1].Play();
            }
        }
        else if (moveVertical > 1.00f * maxSpeed && !crowdSounds[5].GetComponents<AudioSource>()[1].isPlaying)
        {
            foreach (GameObject crowdSound in crowdSounds)
            {
                crowdSound.GetComponents<AudioSource>()[1].clip = claps[7];
                crowdSound.GetComponents<AudioSource>()[1].Play();
            }
        }
        else if (moveVertical > 0.95f * maxSpeed && !crowdSounds[5].GetComponents<AudioSource>()[1].isPlaying)
        {
            foreach (GameObject crowdSound in crowdSounds)
            {
                crowdSound.GetComponents<AudioSource>()[1].clip = claps[6];
                crowdSound.GetComponents<AudioSource>()[1].Play();
            }
        }
        else if (moveVertical > 0.90f * maxSpeed && !crowdSounds[5].GetComponents<AudioSource>()[1].isPlaying)
        {
            foreach (GameObject crowdSound in crowdSounds)
            {
                crowdSound.GetComponents<AudioSource>()[1].clip = claps[5];
                crowdSound.GetComponents<AudioSource>()[1].Play();
            }
        }
        else if (moveVertical > 0.85f * maxSpeed && !crowdSounds[5].GetComponents<AudioSource>()[1].isPlaying)
        {
            foreach (GameObject crowdSound in crowdSounds)
            {
                crowdSound.GetComponents<AudioSource>()[1].clip = claps[4];
                crowdSound.GetComponents<AudioSource>()[1].Play();
            }
        }
        else if (moveVertical > 0.80f * maxSpeed && !crowdSounds[5].GetComponents<AudioSource>()[1].isPlaying)
        {
            foreach (GameObject crowdSound in crowdSounds)
            {
                crowdSound.GetComponents<AudioSource>()[1].clip = claps[3];
                crowdSound.GetComponents<AudioSource>()[1].Play();
            }
        }
        else if (moveVertical > 0.75 * maxSpeed && !crowdSounds[5].GetComponents<AudioSource>()[1].isPlaying)
        {
            foreach (GameObject crowdSound in crowdSounds)
            {
                crowdSound.GetComponents<AudioSource>()[1].clip = claps[2];
                crowdSound.GetComponents<AudioSource>()[1].Play();
            }
        }
        else if (moveVertical > 0.70f * maxSpeed && !crowdSounds[5].GetComponents<AudioSource>()[1].isPlaying)
        {
            foreach (GameObject crowdSound in crowdSounds)
            {
                crowdSound.GetComponents<AudioSource>()[1].clip = claps[1];
                crowdSound.GetComponents<AudioSource>()[1].Play();
            }
        }
    }
}
