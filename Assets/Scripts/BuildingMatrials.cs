using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingMatrials : MonoBehaviour {

    public Material[] possibleTextures;
    // Use this for initialization
    void Start()
    {
        GetComponent<Renderer>().material = possibleTextures[Random.Range(0,12)];

    }

    // Update is called once per frame
    void Update()
    {

    }
}
