using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreCount : MonoBehaviour
{
    public int score;
    public int coins;

    public TMP_Text scoreText;
    public TMP_Text coinsText;
    float timer = 0;

    public void AddScore()
    {
        score++;
    }
    public void AddCoin()
    {
        coins++;
    }

    public void Death()
    {
        gameObject.GetComponent<GameManager>().DisplayDeathScreen(score, coins);
        score = 0;
    }

    void Start()
    {
        //coins = PlayerPrefs.GetInt("Coins");
        //coins = PlayerPrefs.GetInt("Coins");
    }

    // Update is called once per frame
    void Update()
    {
        scoreText.text = "Score: " + score.ToString();
        //coinsText.text = "Coins: " + coins.ToString();
       
        //PlayerPrefs.SetInt("Coins", coins);

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
