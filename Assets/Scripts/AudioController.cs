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

    AudioSource clap;
    AudioSource cheer;
    BikeController bike;


    // Use this for initialization
    void Start () {
        cheer = Cheer.GetComponent<AudioSource>();
        clap = Clap.GetComponent<AudioSource>();
        bike = BikeManager.GetComponent("BikeController") as BikeController;
        
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


        if (Mathf.Abs(moveVertical) > 0.2f &&  !cheer.isPlaying)
        {
            cheer.Play();
        }
        print(clap);
        if (Mathf.Abs(moveVertical) > 1.0f && !clap.isPlaying)
        {
            clap.Play();
        }
        
        
    }
}
