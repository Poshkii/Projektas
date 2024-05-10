using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreCount : MonoBehaviour
{
    public int score;
    public int coins;
    private int multiplyer = 1;
    private float maxSpeed = 4f;
    private float currentSpeed = 0.7f;

    public TMP_Text scoreText;
    public TMP_Text coinsText;
    float timer = 0;
    public Player playerScript;
    GameManager gameManager;
    BoosterManager boosterManager;
    private bool birdReady = true;
    private bool earthquakeReady = true;
    private bool multiplyerReady = true;
    private bool fogReady = true;
    private bool windReady = true;

    private void Start()
    {
        gameManager = GetComponent<GameManager>();
        boosterManager = GetComponent<BoosterManager>();
        coins = gameManager.GetCoins();
    }

    public void AddScore()
    {
        score++;
    }

    public void Death()
    {
        boosterManager.RemoveAllBoosters();
        gameManager.DisplayDeathScreen(score, coins);
        score = 0;
        //coins = 0;
        ResetSpeed();
        playerScript.ResetValues();
    }

    void SpeedUp()
    {
        GameObject[] platforms = GameObject.FindGameObjectsWithTag("Platform");
        foreach (GameObject platform in platforms)
        {
            platform.GetComponent<Platform>().platformSpeed += 0.1f;
            currentSpeed = platform.GetComponent<Platform>().platformSpeed;
        }
    }

    void ResetSpeed()
    {
        GameObject[] platforms = GameObject.FindGameObjectsWithTag("Platform");
        foreach (GameObject platform in platforms)
        {
            platform.GetComponent<Platform>().platformSpeed = 0.7f;
            currentSpeed = platform.GetComponent<Platform>().platformSpeed;
        }

    }

    internal void AddCoin(int multiplyer)
    {
        coins += multiplyer;
    }
    internal void DecreaseCoin(int cost)
    {
        coins -= cost;
    }

    // Update is called once per frame
    void Update()
    {
        scoreText.text = "Score: " + score.ToString();
        coinsText.text = coins.ToString();

        //PlayerPrefs.SetInt("Coins", coins);

        if (timer >= 1f)
        {
            score++;
            if (currentSpeed < maxSpeed)
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
        }
        if (score > 15f)
        {
            if (fogReady)
            {
                gameManager.StartFog();
                fogReady = false;
                StartCoroutine(FogCooldown());
            }
        }
        if (score > 20f)
        {
            if (windReady)
            {
                gameManager.SpawnWind();
                windReady = false;
                StartCoroutine(WindCooldown());
            }
        }
        if (score > 25f)
        {
            if (earthquakeReady)
            {
                gameManager.Earthquake(0.5f);
                earthquakeReady = false;
                StartCoroutine(EarthquakeCooldown());
            }
        }
    }
    public int GetMultiplyer()
    {
        return multiplyer;
    }

    public void SetMultiplier(int value)
    {
        multiplyer = value;
        //StartCoroutine(MultiplyerLastingTime());
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
        //multiplyer = 1;
    }
    IEnumerator MultiplyerLastingTime()
    {
        yield return new WaitForSeconds(15);
        multiplyer = 1;
    }

    IEnumerator WindCooldown()
    {
        yield return new WaitForSeconds(Random.Range(15, 30));
        windReady = true;
    }

    IEnumerator FogCooldown()
    {
        yield return new WaitForSeconds(Random.Range(25, 45));
        fogReady = true;
    }
}

