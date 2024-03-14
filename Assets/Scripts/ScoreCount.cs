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
    public Character playerScript;
    GameManager gameManager;
    private bool earthquakeReady = true;

    private void Start()
    {
        gameManager = GetComponent<GameManager>();
    }

    public void AddScore()
    {
        score++;
    }
    

    public void Death()
    {
        gameObject.GetComponent<GameManager>().DisplayDeathScreen(score, coins);
        score = 0;
        coins = 0;
        ResetSpeed();
        playerScript.ResetValues();
    }

    void SpeedUp()
    {
        GameObject[] platforms = GameObject.FindGameObjectsWithTag("Platform");
        foreach (GameObject platform in platforms)
        {
            platform.GetComponent<Platform>().platformSpeed += 0.1f;
        }
    }

    void ResetSpeed()
    {
        GameObject[] platforms = GameObject.FindGameObjectsWithTag("Platform");
        foreach (GameObject platform in platforms)
        {
            platform.GetComponent<Platform>().platformSpeed = 0.7f;
        }

    }

    internal void AddCoin()
    {
        coins++;        
    }

    // Update is called once per frame
    void Update()
    {
        scoreText.text = "Score: " + score.ToString();
        coinsText.text = "Coins: " + coins.ToString();
       
        //PlayerPrefs.SetInt("Coins", coins);

        if (timer >= 1f)
        {
            score++;
            SpeedUp();
            timer = 0f;
        }
        else
        {
            timer += Time.deltaTime;
        }

        if (score > 10f && earthquakeReady && Random.value < 0.01f)
        {
            StartCoroutine(StartEarthquake());
            earthquakeReady = false;
        }
    }

    IEnumerator StartEarthquake()
    {
        yield return new WaitForSeconds(5f);
        gameManager.Earthquake(0.2f);
    }
}
