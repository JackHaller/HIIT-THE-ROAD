using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;

public class GuiController : MonoBehaviour {
	
	public GUIText DebugText;
	public GUIText LivesText;
	public GUIText ScoreText;
	public TextMesh OculusLives;
	public TextMesh OculusScore;
	public TextMesh GameOverText;
	public TextMesh GameOverInfoText;
	public TextMesh HighScoresText;
	public TextMesh HighScorePrefab;
	public Transform Radar;
	
	public GlobalSettings globalSettings;
	
	private List<TextMesh> scoreTexts;
	
	private List<TextMesh> ghostsDistances;
	private List<TextMesh> ghostScore;
	
	void Start() {
		scoreTexts = new List<TextMesh>();	
		ghostsDistances = new List<TextMesh>();
		ghostScore = new List<TextMesh>();
		if (!globalSettings.EnableLives) {
			LivesText.enabled = false;
			OculusLives.GetComponent<Renderer>().enabled = false;
		}
		//hide the cursor while playing
		Cursor.visible = false;
	}
	
	public void SetDebugText(string text) {
		DebugText.text = text;	
	}
	
	public void SetLives(int lives) {
		LivesText.text = string.Format("Lives: {0}", lives);
		OculusLives.text = string.Format("Lives: {0}", lives);
	}
	
	public void SetScore(int score) {
		ScoreText.text = string.Format("Score: {0}", score);
		OculusScore.text = string.Format("Score: {0}", score);
	}
	
	//Called when the game ends
	public void ShowGameOver() {
		GameOverText.GetComponent<Renderer>().enabled = true;
		GameOverInfoText.GetComponent<Renderer>().enabled = true;
		ShowHighScores();
	}
	
	//Called when the game is restarted after having earlier ended
	public void HideGameOver() {
		GameOverText.GetComponent<Renderer>().enabled = false;
		GameOverInfoText.GetComponent<Renderer>().enabled = false;
		HideHighScores ();
	}
	
	public void UpdateGhostDistanceInfo(int ghostNum, float dist){
		//int pos = -1;
		for(int i = 0; i < ghostsDistances.Count; i++){
			if(ghostsDistances[i].text.Contains("#"+ghostNum)){
				ghostsDistances[i].text = string.Format("#{0}: {1}",ghostNum, dist);
				return; // Found the ghost data in the GUI. Only need to update this.
			}
		}
		//If we get this far, it means the ghost IS NOT in the gui text. Add it.
		TextMesh item = (TextMesh)(Instantiate (OculusLives));
		ghostsDistances.Add(item);
		item.text = string.Format("#{0}: {1}",ghostNum, dist);
		item.GetComponent<Renderer>().enabled = true;
	}
	
	public void UpdateGhostScoreInfo(int ghostNum, int score){
		//int pos = -1;
		for(int i = 0; i < ghostScore.Count; i++){
			if(ghostScore[i].text.Contains("#"+ghostNum)){
				ghostScore[i].text = string.Format("#{0}: {1}",ghostNum, score);
				return; // Found the ghost data in the GUI. Only need to update this.
			}
		}
		//If we get this far, it means the ghost IS NOT in the gui text. Add it.
		TextMesh item = (TextMesh)(Instantiate (OculusScore));
		ghostScore.Add(item);
		item.text = string.Format("#{0}: {1}",ghostNum, score);
		item.GetComponent<Renderer>().enabled = true;
	}
	
	
	public void RemoveGhostDistanceInfo(int ghostNum){
		for(int i = 0; i < ghostsDistances.Count; i++){
			if(ghostsDistances[i].text.Contains("#"+ghostNum)){
				Destroy(ghostsDistances[i]);
				ghostsDistances.RemoveAt(i);
				return; // Found the ghost data in the GUI. Remove it.
			}
		}
	}
	
	public void RemoveGhostScoreInfo(int ghostNum){
		for(int i = 0; i < ghostsDistances.Count; i++){
			if(ghostScore[i].text.Contains("#"+ghostNum)){
				Destroy(ghostScore[i]);
				ghostScore.RemoveAt(i);
				return; // Found the ghost data in the GUI. Remove it.
			}
		}
	}
	
	public void ShowHighScores() {
		List<int> topScores = (new int[] {0, 0, 0, 0, 0}).ToList();
		try {
			using (StreamReader reader = new StreamReader("scores.txt")) {
				string line;
				while ((line = reader.ReadLine()) != null) {
					string[] scoreSplit = line.Split(new string[] {"||"}, StringSplitOptions.RemoveEmptyEntries);
					if (scoreSplit.Length == 2) {
						int score = int.Parse(scoreSplit[1].Trim());
						if (score > topScores[4]) {
							//search down the scores till we find the right place for this one
							for (int i = 0; i < 5; i++) {	
								if (score > topScores[i]) {
									topScores.Insert(i, score);
									topScores.RemoveAt(5);
									break;
								}
							}
						}
					}
				}
			}
		} catch (IOException e) {
			Debug.Log (e.ToString());	
		}
		//build the scores prefabs
		for (int i = 0; i < 5; i++) {
			TextMesh item = (TextMesh)(Instantiate(HighScorePrefab));
			scoreTexts.Add(item);
			item.text = topScores[i].ToString();
		}
		HighScoresText.GetComponent<Renderer>().enabled = true;
	}
	
	public void HideHighScores() {
		HighScoresText.GetComponent<Renderer>().enabled = false;
		for (int i = 0; i < scoreTexts.Count; i ++) {
			Destroy(scoreTexts[i]);
			scoreTexts[i] = null;
		}
		scoreTexts.Clear();
	}
	
	public void UpdateOculusUI(Vector3 playerPosition) {
		OculusLives.transform.position = new Vector3(playerPosition.x + 24.5f, playerPosition.y + 10.5f, playerPosition.z - 18.50f);
		OculusScore.transform.position = new Vector3(playerPosition.x + 24.5f, playerPosition.y + 10.5f, playerPosition.z + 23.5f);
		GameOverText.transform.position = new Vector3(playerPosition.x + 24.5f, playerPosition.y + 8.5f, playerPosition.z);
		GameOverInfoText.transform.position = new Vector3(playerPosition.x + 24.5f, playerPosition.y + 6.5f, playerPosition.z);
		HighScoresText.transform.position = new Vector3(playerPosition.x + 24.5f, playerPosition.y + 4.5f, playerPosition.z);
		Radar.position = new Vector3(playerPosition.x + 24.5f, playerPosition.y + 4.5f, playerPosition.z);
		for (int i = 0; i < scoreTexts.Count; i++) {
			scoreTexts[i].transform.position = new Vector3(playerPosition.x + 24.5f, playerPosition.y + 2.5f - (1.8f * i), playerPosition.z);
		}
		for(int i = 1; i < ghostsDistances.Count+1; i++){
			ghostsDistances[i-1].transform.position = new Vector3(playerPosition.x + 24.5f, playerPosition.y + 10.5f - (2f * i), playerPosition.z - 18.50f);
		}
		for(int i = 1; i < ghostScore.Count+1; i++){
			ghostScore[i-1].transform.position = new Vector3(playerPosition.x + 24.5f, playerPosition.y + 10.5f - (2f*i), playerPosition.z + 23.5f);
		}
	}
}
