﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System;

public class UIController : MonoBehaviour {

	//Other objects to reference
	public Transform playerBike;
    public Text GameOverText;
    public Text HighScoresText;
	public Image ChargeBar;

	private int score;
    private int lives;
    private List<GameObject> scoreTexts;  //holds instantiated high score displays

	//Prefabs to instantiate
	public GameObject scoreBonus;
    public GameObject HighScorePrefab;
    private Text startingText;
   

	// Use this for initialization
	void Start () {
        HighScoresText.enabled = false;
        startingText = GameObject.FindGameObjectWithTag("CalibrationText").GetComponentInChildren<Text>(); ;
        StartCoroutine(StartCountDown());
        scoreTexts = new List<GameObject>();
        score = 10000;
		HighScoresText.text = "Score: " + 10000;
        Cursor.visible = false;
	}
	
	// Update is called once per frame
	void Update () {
		this.transform.position = new Vector3 (playerBike.transform.position.x + 15.0f, playerBike.transform.position.y - 0.75f, playerBike.transform.position.z);
	}

	public void GiveScore(int points) {
		RectTransform scoreText = ((GameObject)Instantiate (scoreBonus)).GetComponent<RectTransform> ();
        scoreText.SetParent(this.transform);
		scoreText.localPosition = Vector3.zero;
		scoreText.localRotation = Quaternion.identity;
		scoreText.gameObject.GetComponent<ScoreChangeTextController> ().Activate (points);
	}

    public void ShowGameOver()
    {
        GameOverText.enabled = true;
        HighScoresText.enabled = true;
        List<int> topScores = (new int[] { 0, 0, 0, 0, 0 }).ToList();
        try
        {
            using (StreamReader reader = new StreamReader("scores.txt"))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] scoreSplit = line.Split(new string[] { "||" }, StringSplitOptions.RemoveEmptyEntries);
                    if (scoreSplit.Length == 2)
                    {
                        int score = int.Parse(scoreSplit[1].Trim());
                        if (score > topScores[4])
                        {
                            //search down the scores till we find the right place for this one
                            for (int i = 0; i < 5; i++)
                            {
                                if (score > topScores[i])
                                {
                                    topScores.Insert(i, score);
                                    topScores.RemoveAt(5);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
        catch (IOException e)
        {
            Debug.Log(e.ToString());
        }

        //build the scores prefabs
        GameObject item = (GameObject)(Instantiate(HighScorePrefab));
        RectTransform location = item.GetComponent<RectTransform>();
        location.SetParent(this.transform);
        Text itemText = item.GetComponent<Text>();
        itemText.text = HighScoresText.text;
        location.localPosition = new Vector3(0, 63f, 0);
        location.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        location.localRotation = Quaternion.identity;
        scoreTexts.Add(item);        
    }

    public void HideGameOver()
    {
        GameOverText.enabled = false;
        HighScoresText.enabled = false;
        foreach (GameObject scoreText in scoreTexts)
        {
            GameObject.Destroy(scoreText);
        }
        scoreTexts.Clear();
    }

    public void SetLives(int lives)
    {
        this.lives = lives;
        //LivesText.text = "Lives: "+ lives;
    }

	public void SetScore(int score) {
		this.score = score;
        HighScoresText.text = "Score: " + score;
	}

	public void ModScore(int amountToChange) {
		this.score += amountToChange;
		HighScoresText.text = "Score: " + this.score;
	}

	public void Resize(int x, int y) {
		//Default size is 600 * 240, which works great for Maximise on Play on my dev monitor
        //May 
	}

	public void SetRemainingCharge(int percentage) {
		ChargeBar.fillAmount = (float)percentage / 100.0f;
	}

    //Game start UI manager
    IEnumerator StartCountDown()
    {
        var count = 5;
        while (count != 0)
        {
            startingText.text = "Game Starting in " + count + ".";
            yield return new WaitForSeconds(1f);
            count--;
        }
        startingText.text = "GO! GO! GO!";
        yield return new WaitForSeconds(1f);

        GameObject.FindGameObjectWithTag("CalibrationText").SetActive(false);
        HighScoresText.enabled = true;
    }

}
