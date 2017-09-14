using UnityEngine;
using System.Collections;

public class CannonController : MonoBehaviour {
	
	public Rigidbody player = null;
	public Rigidbody projectilePrefab;
	
	private float secondsSinceLastShot = 10000.0f;
	
	public float shotSpeed = 900.0f;
	public float fireRate = 2.0f;
	public float activeRange = 20.0f;
	
	// Update is called once per frame
	void Update () {
		RotateToPlayer ();
		float distanceToPlayer = Vector3.Distance (player.transform.position, this.transform.position);
		if (distanceToPlayer <= activeRange && secondsSinceLastShot >= fireRate) {
			FireProjectile();
		}
		if (secondsSinceLastShot < float.MaxValue) {
			secondsSinceLastShot += Time.deltaTime;
		}
	}
	
	void RotateToPlayer() {
		Vector3 target = 2.0f * this.transform.position - GetTarget ();
		target.y = this.transform.position.y;
		this.transform.LookAt (target);
	}
	
	void FireProjectile() {
		this.GetComponent<AudioSource>().Play ();
		secondsSinceLastShot = 0.0f;
		Vector3 initialProjectileLocation = this.transform.position;
		initialProjectileLocation.y = this.transform.position.y + 3.5f;
		Vector3 toPlayer = (GetTarget() - initialProjectileLocation);
		toPlayer.Normalize ();
		initialProjectileLocation = initialProjectileLocation + (toPlayer * 0.75f);
		Rigidbody projectile = (Rigidbody)Instantiate (projectilePrefab, initialProjectileLocation, Quaternion.LookRotation(toPlayer));
		projectile.AddForce (projectile.transform.forward * shotSpeed);
	}
	
	//Consider's player's velocity in order to guess where to aim to hit the player
	private Vector3 GetTarget() {
		//Aim where we think the player will be, or where we want to encourage the player not to be
		//A flight time of 1 second is a good approximation of aiming where they will be
		//A lower flight time forces the player to pedal a little faster to avoid being hit
		float expectedFlightTime = 0.5f;
		return player.transform.position + (player.velocity * expectedFlightTime);
	}
}
