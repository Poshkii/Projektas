using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreCount : MonoBehaviour
{
    public int score;
    public int coins;
    private int multiplyer=1;

    public TMP_Text scoreText;
    public TMP_Text coinsText;
    float timer = 0;
    public Player playerScript;
    GameManager gameManager;
    private bool birdReady = true;
    private bool earthquakeReady = true;
    private bool multiplyerReady = true;

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
        gameManager.DisplayDeathScreen(score, coins);
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

    internal void AddCoin(int multiplyer)
    {
        coins+=multiplyer;        
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

        if (score > 10f)
        {
            if (birdReady)
            {
                gameManager.SpawnBird();
                birdReady = false;
                StartCoroutine(BirdCooldown());
            }
            if (earthquakeReady)
            {
                gameManager.Earthquake(0.5f);
                earthquakeReady = false;
                StartCoroutine(EarthquakeCooldown());
            }
            if(multiplyerReady)
            {
                multiplyerReady = false;
                multiplyer = 2;
                StartCoroutine(MultiplyerLastingTime());
                StartCoroutine (MultiplyerCooldown());
            }
        }
    }
    public int GetMultiplyer()
    {
        return multiplyer;
    }

    IEnumerator BirdCooldown()
    {
        yield return new WaitForSeconds(Random.Range(5, 20));
        birdReady = true;
    }

    IEnumerator EarthquakeCooldown()
    {
        yield return new WaitForSeconds(Random.Range(5, 30));
        earthquakeReady = true;
    }
    IEnumerator MultiplyerCooldown()
    {
        yield return new WaitForSeconds(Random.Range(10, 30));
        multiplyerReady = true;
        multiplyer = 1;
    }
    IEnumerator MultiplyerLastingTime()
    {
        yield return new WaitForSeconds(3);
        multiplyer = 1;
    }
}
