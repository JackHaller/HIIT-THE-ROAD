using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moveCar : MonoBehaviour {

    public float speed;
    private bool HitByPlayer;
    private bool colliding;
    private float pingTime = 0.5f;
    private float timeSinceLastPing = 0.0f;

    public static int SAND_RESISTANCE = 15;

    void Start()
    {
        HitByPlayer = false;
    }

    // Update is called once per frame
    void Update () {
        gameObject.transform.position = new Vector3(gameObject.transform.position.x + speed, transform.position.y, transform.position.z);
        if (colliding)
        {
            timeSinceLastPing += Time.deltaTime;
            if (timeSinceLastPing >= pingTime)
            {
                GameObject.Find("Player").GetComponent<PlayerController>().TakePoints(20);
                timeSinceLastPing = 0.0f;
            }
        }
    }

    void OnCollisionEnter(Collision other)
    {
        colliding = true;
        StartCoroutine(IncreaseResistance(other.gameObject));
    }

    void OnCollisionExit(Collision other)
    {
        colliding = false;
    }

    IEnumerator IncreaseResistance(GameObject gameObejct)
    {
        gameObejct.GetComponent<PlayerController>().BeginEnvironmentalResistanceOverride(SAND_RESISTANCE);
        yield return new WaitForSeconds(5.0f);
        gameObejct.GetComponent<PlayerController>().EndEnvironmentalResistanceOverride();
    }

}
