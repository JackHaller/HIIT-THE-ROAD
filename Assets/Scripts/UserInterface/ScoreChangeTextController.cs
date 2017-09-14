using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScoreChangeTextController : MonoBehaviour {

	public Color IncreaseColor = Color.blue;
	public Color DecreaseColor = Color.red;
	public float MovementTime = 0.8f;

	private int Score;
	private bool activated = false;
	private Vector3 originalPosition;
	private float elapsedTime = 0.0f;

	// Update is called once per frame
	void Update () {
		if (activated) {
			elapsedTime += Time.deltaTime;
			this.transform.localPosition = Vector3.Lerp(originalPosition, new Vector3(-100.0f, 75.0f, 0.0f), elapsedTime / MovementTime);
			if (elapsedTime >= MovementTime) {
				Destroy(this.gameObject);
				GameObject.Find ("WorldSpaceGUI").GetComponent<UIController>().ModScore(Score);
			}
		}
	}

	public void Activate(int score) {
		this.Score = score;
		this.transform.localScale = new Vector3 (1.0f, 1.0f, 1.0f);
		originalPosition = this.transform.localPosition;
		Text t = this.GetComponent<Text> ();
		t.text = string.Format ("{0}{1}", score >= 0 ? '+' : '-', score);
		t.color = score >= 0 ? IncreaseColor : DecreaseColor;
		activated = true;
	}
}