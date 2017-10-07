using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrowdSpawner : MonoBehaviour {


    public GameObject crowdList;
    public int crowdDensity = 10;
	// Use this for initialization
	void Start () {
        for (int i=0; i < crowdDensity; i++)
        {
            Transform test = Instantiate(crowdList.transform.GetChild(Random.Range(0, 7)));
            test.transform.position = new Vector3(Random.Range(0, 7), 0, Random.Range(0, 3));
            test.transform.rotation = Quaternion.Euler(0, 180, 0);
        }

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
