using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    int highScore = 0;
    int coins = 0;
    public Canvas gameUI;
    public Canvas deathScreenUI;
    public TMP_Text scoreText;
    public TMP_Text highscoreText;
    public TMP_Text coinsText;
    public TMP_Text scoreboardText;
    public GameObject player;    
    public GameObject starterPlatform;
    public GameObject birdSpawnerObj;
    private BirdSpawner birdSpawner;
    public GameObject camera;
    private bool shakeCamera = false;
    private float duration = 0f;
    private Vector3 cameraStartPos;
    private const float baseShakeStrength = 0.2f;
    private float ShakeStrength;

    private List<int> scores = new List<int>();

    // Start is called before the first frame update
    void Start()
    {
        SpawnStarterPlatform();
        deathScreenUI.gameObject.SetActive(false);
        birdSpawner = birdSpawnerObj.GetComponent<BirdSpawner>();
        cameraStartPos = camera.transform.position;
        ShakeStrength = baseShakeStrength;
    }

    // Update is called once per frame
    void Update()
    {
        if (shakeCamera)
        {
            if (duration > 0f)
            {
                if (duration < 1f)
                {
                    ShakeStrength *= 0.2f;
                }
                camera.transform.localPosition = cameraStartPos + UnityEngine.Random.insideUnitSphere * ShakeStrength;
                duration -= Time.deltaTime;
            }
            else
            {
                ShakeStrength = baseShakeStrength;
                shakeCamera = false;
                duration = 0f;
                camera.transform.localPosition = cameraStartPos;
            }
        }
    }

    public void DisplayDeathScreen(int runScore, int runCoins)
    {
        Time.timeScale = 0f;
        scores.Add(runScore);
        scores.Sort();
        scores.Reverse();
        if (scores.Count > 10 )
        {
            scores.RemoveRange(10, scores.Count - 10);
        }        
        scoreboardText.text = "Scoreboard\n";
        for (int i = 0; i < scores.Count; i++)
        {
            scoreboardText.text += (i+1) + ". " + scores[i] + "\n";
        }
        gameUI.gameObject.SetActive(false);
        deathScreenUI.gameObject.SetActive(true);
        if (runScore > highScore)
        {
            highScore = runScore;
        }
        coins += runCoins;
        scoreText.text = "Score: " + runScore;
        highscoreText.text = "Highscore: " + highScore;
        coinsText.text = "Coins: " + coins;
    }

    public void PlayAgain()
    {
        Time.timeScale = 1f;
        deathScreenUI.gameObject.SetActive(false);
        gameUI.gameObject.SetActive(true);        
        //Debug.Log("Play");
        Instantiate(starterPlatform);
    }

    public void SpawnStarterPlatform()
    {
        Instantiate(starterPlatform);
    }

    public void SpawnBird()
    {
        birdSpawner.SpawnObject();
    }

    private void ShakeCamera(float duration)
    {
        shakeCamera = true;
        this.duration = duration;
    }

    public void Earthquake(float chanceToDropPlatform)
    {
        GameObject[] platforms = GameObject.FindGameObjectsWithTag("Platform");
        foreach (GameObject plat in platforms)
        {
            if (Math.Abs(plat.transform.position.x - player.transform.position.x) < 8f &&
                Math.Abs(plat.transform.position.x - player.transform.position.x) > 2f &&
                UnityEngine.Random.value < chanceToDropPlatform)
            {
                ShakeCamera(2f);
                plat.GetComponent<Platform>().DropPlatform();
                break;
            }
        }
    }
}
