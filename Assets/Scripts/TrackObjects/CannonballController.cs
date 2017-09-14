using UnityEngine;
using System.Collections;

public class CannonballController : MonoBehaviour {
	
	public Transform Blast;
	
	private float remainingDuration;
	
	// Use this for initialization
	void Start () {
		remainingDuration = 4.0f;
	}
	
	// Update is called once per frame
	void Update () {
		remainingDuration -= Time.deltaTime;
		if (remainingDuration <= 0.0f || this.transform.position.y <= -20.0f) {
			DestroyCannonball();
		}
	}
	
	void OnCollisionEnter(Collision other) {
		if (other.gameObject.tag == "Player") {
			DestroyCannonball();
		}
	}
	
	void DestroyCannonball() {
		//ideally, here we'd have a particle effect, maybe a small explosion sound
		//For now, we just remove it
		Instantiate(Blast, this.transform.position, Quaternion.identity);
		Destroy(gameObject);
	}
}
