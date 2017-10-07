using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleController : MonoBehaviour {

    public float speed;
    public GameObject LFWheel;
    public GameObject RFWheel;
    public GameObject LRWheel;
    public GameObject RRWheel;
    private bool _hitByPlayer;
    private bool _colliding;
    private float _pingTime = 0.5f;
    private float _timeSinceLastPing = 0.0f;
    private Vector3 _xAxis = new Vector3(1, 0, 0);
    private float _rotation = 10f;
    private GameObject _player;
    private List<GameObject> _wheels;
    private AudioSource _horn;
    private string _lanePosition;
    private bool _hornPlaying;

    public static int SAND_RESISTANCE = 15;

    void Start()
    {
        _hitByPlayer = false;
        _hornPlaying = false;
        _horn = GetComponent<AudioSource>();
        _player = GameObject.Find("Player");
        _lanePosition = SetLanePosition();
    }

    // Update is called once per frame
    void FixedUpdate () {
        gameObject.transform.position = new Vector3(gameObject.transform.position.x + speed * 0.157f, transform.position.y, transform.position.z);
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
        
        if (_player.transform.position.x - transform.position.x < 10f &&
            _player.transform.position.x - transform.position.x > 0 &&
            _player.GetComponent<PlayerController>().GetLanePosition() == _lanePosition &&
            !_hornPlaying)
        {
            _hornPlaying = true;
            _horn.Play();
            StartCoroutine(AudioFinished());
        }

        LFWheel.transform.Rotate(_xAxis, _rotation);
        RFWheel.transform.Rotate(_xAxis, _rotation);
        LRWheel.transform.Rotate(_xAxis, _rotation);
        RRWheel.transform.Rotate(_xAxis, _rotation);
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
        if(gameObject.tag == "Player")
        {
            gameObejct.GetComponent<PlayerController>().BeginEnvironmentalResistanceOverride(SAND_RESISTANCE);
            yield return new WaitForSeconds(5.0f);
            gameObejct.GetComponent<PlayerController>().EndEnvironmentalResistanceOverride();
        }
    }

    string SetLanePosition()
    {
        var pos = transform.position.z;
        if (pos < -0.6f)
            return "left";
        else if (pos > 0.8)
            return "right";
        return "centre";
    }
    void horn()
    {
        _horn.enabled = true;
        _hornPlaying = true;
        _horn.Play();
    }

    IEnumerator AudioFinished()
    {
        yield return new WaitForSeconds(1.5f);
        _horn.Stop();
        _hornPlaying = false;
    }

}
