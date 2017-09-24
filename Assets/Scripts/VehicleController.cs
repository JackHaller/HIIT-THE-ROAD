using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleController : MonoBehaviour {

    public float speed;
    private bool _hitByPlayer;
    private bool _colliding;
    private float _pingTime = 0.5f;
    private float _timeSinceLastPing = 0.0f;
    private GameObject _player;

    public static int SAND_RESISTANCE = 15;

    void Start()
    {
        _hitByPlayer = false;
        _player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update () {
        gameObject.transform.position = new Vector3(gameObject.transform.position.x + speed, transform.position.y, transform.position.z);
        if (_colliding)
        {
            _timeSinceLastPing += Time.deltaTime;
            if (_timeSinceLastPing >= _pingTime)
            {
                _player.GetComponent<PlayerController>().TakePoints(20);
                _timeSinceLastPing = 0.0f;
            }
        }
        if (_player.transform.position.x - transform.position.x > 200f)
            Destroy(gameObject);
    }

    void OnCollisionEnter(Collision other)
    {
        _colliding = true;
        StartCoroutine(IncreaseResistance(other.gameObject));
    }

    void OnCollisionExit(Collision other)
    {
        _colliding = false;
    }

    IEnumerator IncreaseResistance(GameObject gameObejct)
    {
        gameObejct.GetComponent<PlayerController>().BeginEnvironmentalResistanceOverride(SAND_RESISTANCE);
        yield return new WaitForSeconds(5.0f);
        gameObejct.GetComponent<PlayerController>().EndEnvironmentalResistanceOverride();
    }

}
