using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierController : MonoBehaviour {

    public GameObject explosion;

    //destroyes vehilces when colliding with barrier
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Vehicle")
        {
            Instantiate(explosion, transform.position, transform.rotation);
            Destroy(other.gameObject);
        }
    }
}
