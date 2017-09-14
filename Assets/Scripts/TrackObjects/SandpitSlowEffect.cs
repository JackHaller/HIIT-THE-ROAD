using UnityEngine;
using System.Collections;

public class SandpitSlowEffect : MonoBehaviour {
	
	public static int SAND_RESISTANCE = 15;

	void OnTriggerEnter(Collider other) {
		if (other.tag == "Player") {
			other.GetComponent<PlayerController>().BeginEnvironmentalResistanceOverride(SAND_RESISTANCE);
		}
	}
	
	void OnTriggerExit(Collider other) {
		if (other.tag == "Player") {
			other.GetComponent<PlayerController>().EndEnvironmentalResistanceOverride();
		}
	}
}
