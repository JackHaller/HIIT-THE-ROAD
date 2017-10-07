using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using AssemblyCSharp;
using Exergame;

public class AudioController : MonoBehaviour {
    
    public GameObject BikeManager;
    public GameObject Cheer;
    public GameObject Clap;
    
    public AudioClip clap1;
    public AudioClip clap1_1;
    public AudioClip clap1_2;
    public AudioClip clap1_3;
    public AudioClip clap1_4;
    public AudioClip clap1_5;
    public AudioClip clap1_6;
    public AudioClip clap1_7;
    public AudioClip clap1_8;
    public AudioClip clap1_9;
    public AudioClip clap2;


    AudioSource clap;
    AudioSource cheer;
    BikeController bike;


    // Use this for initialization
    void Start () {
        cheer = Cheer.GetComponent<AudioSource>();
        clap = Clap.GetComponent<AudioSource>();
        bike = BikeManager.GetComponent("BikeController") as BikeController;

        cheer.Play();
        
    }
	
	// Update is called once per frame
	void Update () {
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

        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            moveVertical *= 2;
        }

        if (!clap.isPlaying)
        {
            if (Mathf.Abs(moveVertical) > 2.0f)
            {
                clap.clip = clap2;
                clap.Play();
            }
            else if (Mathf.Abs(moveVertical) > 1.0f)
            {
                clap.clip = clap1;
                clap.Play();
            }
            
        }
        
        
    }
}
