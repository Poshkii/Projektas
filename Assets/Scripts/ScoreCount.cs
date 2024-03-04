using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreCount : MonoBehaviour
{
    public int score;
    public int highScore;
    public int coins;

    public TMP_Text scoreText;
    public Text highScoreText;
    public Text coinsText;
    float timer = 0;

    public void AddScore()
    {
        score++;
    }
    public void AddCoin()
    {
        coins++;
    }
    void Start()
    {
        highScore = PlayerPrefs.GetInt("Highscore");
        coins = PlayerPrefs.GetInt("Coins");
        //coins = PlayerPrefs.GetInt("Coins");
    }

    // Update is called once per frame
    void Update()
    {
        scoreText.text = "Score: " + score.ToString();
        //highScoreText.text = "Highscore: " + highScore.ToString();
        //coinsText.text = "Coins: " + coins.ToString();

        if (score > highScore)
        {
            PlayerPrefs.SetInt("Highscore", score);

        }
        PlayerPrefs.SetInt("Coins", coins);

        if (timer >= 1f)
        {
            score++;
            timer = 0f;
        }
        else
        {
            timer += Time.deltaTime;
        }
    }
}
