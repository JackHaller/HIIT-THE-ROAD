using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingMatrials : MonoBehaviour {

    public Material[] possibleTextures;
    // Use this for initialization
    void Start()
    {
        //randomizes buidling colours
        GetComponent<Renderer>().material = possibleTextures[Random.Range(0,possibleTextures.Length)];

    }

    // Update is called once per frame
    void Update()
    {

    }
}
