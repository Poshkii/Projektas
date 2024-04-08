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
    public Canvas optionsPanelUI;
    public Canvas startPanelUI;
    public Canvas shopUI;
    public TMP_Text scoreText;
    public TMP_Text highscoreText;
    public TMP_Text coinsText;
    public TMP_Text scoreboardText;
    public GameObject player;    
    public GameObject starterPlatform;
    public GameObject birdSpawnerObj;
    public WindSpawner windSpawner;
    private BirdSpawner birdSpawner;
    ScoreCount scoreCount;
    public GameObject camera;
    private bool shakeCamera = false;
    private float duration = 0f;
    private Vector3 cameraStartPos;
    private const float baseShakeStrength = 0.2f;
    private float ShakeStrength;
    AudioManager audioManager;

    private List<int> scores = new List<int>();
    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }
    // Start is called before the first frame update
    void Start()
    {
        scoreCount = GetComponent<ScoreCount>();
        Time.timeScale = 0f;
        optionsPanelUI.gameObject.SetActive(false);
        gameUI.gameObject.SetActive(false);
        deathScreenUI.gameObject.SetActive(false);
        if (PlayerPrefs.HasKey("coins") || PlayerPrefs.HasKey("highscore"))
        {
            LoadPrefs();
        }
        else
        {
            SetPrefs(coins, highScore);
        }
        birdSpawner = birdSpawnerObj.GetComponent<BirdSpawner>();
        cameraStartPos = camera.transform.position;
        ShakeStrength = baseShakeStrength;
        //GetComponent<Animator>().Play("Drop", -1, 0f);
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
        SetPrefs(coins, highScore);
    }

    public void PlayAgain()
    {
        Time.timeScale = 1f;
        deathScreenUI.gameObject.SetActive(false);
        gameUI.gameObject.SetActive(true);        
        //Debug.Log("Play");
        Instantiate(starterPlatform);
    }
    public void CancelOptions()
    {
        optionsPanelUI.gameObject.SetActive(false);
    }

    public void OpenOptions()
    {
        optionsPanelUI.gameObject.SetActive(true);
    }
    public void StartGame()
    {
        Time.timeScale = 1f;
        startPanelUI.gameObject.SetActive(false);
        gameUI.gameObject.SetActive(true);
        SpawnStarterPlatform();
        // GameObject[] platforms = GameObject.FindGameObjectsWithTag("Platform");
        //platforms[1].GetComponentInChildren<Animator>().Play("Drop", -1, 0f);
    }

    public void SetPrefs(int x, int y)
    {
        PlayerPrefs.SetInt("coins", x);
        PlayerPrefs.SetInt("highscore", y);
        coins = x;
        highScore = y;
    }
    public void LoadPrefs()
    {
        int x = PlayerPrefs.GetInt("coins");
        int y = PlayerPrefs.GetInt("highscore");

        SetPrefs(x, y);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    public void SpawnStarterPlatform()
    {
        Instantiate(starterPlatform);
    }
    public void OpenShop()
    {
        shopUI.gameObject.SetActive(true);
    }
    public void CloseShop()
    {
        shopUI.gameObject.SetActive(false);
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
                audioManager.PlaySFX(audioManager.earthquake);
                break;
            }
        }
    }

    public void ApplyBooster(string type)
    {
        if (type == "ExtraLife")
        {
            AddLife();
        }
        else if (type == "ExtraJump")
        {
            Debug.Log("Extra Jump activated");
            SetJump(2);
            StartCoroutine(DoubleJumpLastingTime());
        }
        else if (type == "DoubleCoins")
        {
            scoreCount.SetMultiplier(2);
            Debug.Log("Double Coins activated");
        }
    }

    private void AddLife()
    {
        player.GetComponent<Player>().AddLife();
    }

    private void SetJump(int count)
    {
        player.GetComponent<Player>().jumpCount = count;
    }

    public void SpawnWind()
    {
        windSpawner.SpawnWind();
    }

    IEnumerator DoubleJumpLastingTime()
    {
        yield return new WaitForSeconds(15f);
        SetJump(1);
    }
}
