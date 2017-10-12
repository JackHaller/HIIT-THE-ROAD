using UnityEngine;
using System.Collections;
using Exergame;

public class PowerupController : MonoBehaviour {
	
	public PowerupType powerupType;

	public Transform Icon;

	private GlobalSettings globalSettings;

    public AudioClip lifeSound;
    public AudioClip scoreSound;
    public AudioClip resistanceSound;

	void Start() {
		globalSettings = GameObject.Find ("GlobalSettings").GetComponent<GlobalSettings> ();
		//Create the radar icon for this powerup
		Transform instantiatedIcon = (Transform)Instantiate (Icon, Vector3.zero, Quaternion.identity);
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate (new Vector3(0.0f, 30.0f * Time.deltaTime, 0.0f));
	}
	
	void OnTriggerEnter(Collider other) {
		//If the player has just collided with the powerup, award it to the player then remove it
		if (other.tag == "Player") {
			PowerupType award = powerupType;
			if (powerupType == PowerupType.RandomPowerup) {
				if (globalSettings.EnableLives) {
					//lives enabled, any powerup is fine
					award = (PowerupType)Random.Range(0, 3);
				} else {
					//lives not enabled, as type 0 is life, we use 1 as the lower bound
					award = (PowerupType)Random.Range(1, 3);
				}
                //set the audio based on the reward type
                switch (award)
                {
                    case PowerupType.Life:
                        this.GetComponent<AudioSource>().clip = lifeSound;
                        break;
                    case PowerupType.Resistance:
                        this.GetComponent<AudioSource>().clip = resistanceSound;
                        break;
                    case PowerupType.Score:
                        this.GetComponent<AudioSource>().clip = scoreSound;
                        break;
                }
			}
			other.GetComponent<PlayerController>().AwardPowerup(award);
            
            this.GetComponent<AudioSource>().Play();
            //Remove the powerup
			GetComponent<Renderer>().enabled = false;
			GetComponent<Collider>().enabled = false;
		}
	}
}