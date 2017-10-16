using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class returnBikeController : MonoBehaviour
{


    private PlayerController player;
    // Use this for initialization
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    void OnCollisionEnter(Collision collision)
    {
        player.bikeOverRidden = false;
    }
}

