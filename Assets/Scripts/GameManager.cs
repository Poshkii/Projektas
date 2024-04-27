using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

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
    public TMP_Text pickupText;
    public GameObject player;    
    public GameObject starterPlatform;
    public GameObject birdSpawnerObj;
    public WindSpawner windSpawner;
    public Animator pickupTextAnim;
    private BirdSpawner birdSpawner;
    BoosterManager boosterManager;
    ScoreCount scoreCount;
    public GameObject camera;
    public GameObject[] characters;
    public Button[] buttonsBuy;
    public int[] isBought;
    private bool shakeCamera = false;
    private float duration = 0f;
    private Vector3 cameraStartPos;
    private const float baseShakeStrength = 0.2f;
    private float ShakeStrength;
    AudioManager audioManager;
    public ParticleSystem fogPartciles;
    public PlatformSpawner platformSpawner;
    private float startTimeScale;
    private float startFixedDeltaTime;
    internal bool gameStarted = false;

    private List<int> scores = new List<int>();
    private void Awake()
    {
        isBought = Enumerable.Repeat(0, characters.Length).ToArray();
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }
    // Start is called before the first frame update
    void Start()
    {
        boosterManager = GetComponent<BoosterManager>();
        StopFog();
        startTimeScale = Time.timeScale;
        startFixedDeltaTime = Time.fixedDeltaTime;
        Debug.Log(startTimeScale);
        Debug.Log(startFixedDeltaTime);
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
        if (PlayerPrefs.HasKey("default") || PlayerPrefs.HasKey("red") || PlayerPrefs.HasKey("violet") || PlayerPrefs.HasKey("yellow"))
        {
            LoadPrefsCharacter();
        }
        else
        {
            SetPrefsCharacter(isBought);
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
        coins = runCoins;
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
        StopFog();
        fogPartciles.Clear();
        platformSpawner.Restart();
        gameStarted = true;
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
        gameStarted = true;
        //GameObject[] platforms = GameObject.FindGameObjectsWithTag("Platform");
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
    public void SetPrefsCharacter(int[]x)
    {
        PlayerPrefs.SetInt("default", x[0]);
        PlayerPrefs.SetInt("red", x[1]);
        PlayerPrefs.SetInt("violet", x[2]);
        PlayerPrefs.SetInt("yellow", x[3]);

        isBought[0] = x[0];
        isBought[1] = x[1];
        isBought[2] = x[2];
        isBought[3] = x[3];
        ChangeButtonText(isBought[0], buttonsBuy[0], characters[0]);
        ChangeButtonText(isBought[1], buttonsBuy[1], characters[1]);
        ChangeButtonText(isBought[2], buttonsBuy[2], characters[2]);
        ChangeButtonText(isBought[3], buttonsBuy[3], characters[3]);

    }
    public void ChangeButtonText(int x, Button button, GameObject character)
    {
        TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
        if (x == 1)
        {
            buttonText.text = "SELECT";
            character.SetActive(false);
        }
        if (x == 2)
        {
            buttonText.text = "SELECTED";
            character.SetActive(true);
        }
    }
    public void LoadPrefsCharacter()
    {
        int[] x = new int[isBought.Length];
        x[0] = PlayerPrefs.GetInt("default");
        x[1] = PlayerPrefs.GetInt("red");
        x[2] = PlayerPrefs.GetInt("violet");
        x[3] = PlayerPrefs.GetInt("yellow");

        SetPrefsCharacter(x);
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
            Platform platformScript = plat.GetComponent<Platform>();

            if (!platformScript.dropped &&
                Math.Abs(plat.transform.position.x - player.transform.position.x) < 8f &&
                Math.Abs(plat.transform.position.x - player.transform.position.x) > 2f &&
                UnityEngine.Random.value < chanceToDropPlatform)
            {
                StartCoroutine(DelayedCameraShake(2.2f, 2.2f));
                plat.GetComponent<Platform>().DropPlatform();
                audioManager.PlaySFX(audioManager.earthquake);
                break;
            }
        }
    }

    public void ApplyBooster(string type)
    {
        float duration = 15f;
        if (type == "ExtraLife")
        {
            pickupText.text = "Extra Life Picked Up!";
            AddLife();
            duration = 1f;
        }
        else if (type == "ExtraJump")
        {
            pickupText.text = "Double Jump Picked Up!";
            duration = 15f;
            SetJump(2);
            StartCoroutine(DoubleJumpLastingTime());
        }
        else if (type == "DoubleCoins")
        {
            duration = 20f;
            pickupText.text = "Double Coins Picked Up!";
            scoreCount.SetMultiplier(2);
        }
        else if (type == "SlowTime")
        {
            duration = 5f;
            pickupText.text = "Slow Motion Picked Up!";
            Time.timeScale = 0.5f;
            Time.fixedDeltaTime = startFixedDeltaTime * 0.5f;
            StartCoroutine(SlowTimeLastingTime());
        }
        boosterManager.AddBooster(type, duration);
        pickupTextAnim.Play("BoosterPickupTextAnim", -1, 0f);
        audioManager.PlaySFX(audioManager.powerUp);
    }

    public void StartFog()
    {
        fogPartciles.Play();
        StartCoroutine(FogLastingTime());
    }
    public void StopFog()
    {
        fogPartciles.Stop();
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

    IEnumerator SlowTimeLastingTime()
    {
        yield return new WaitForSeconds(5f);
        Time.timeScale = startTimeScale;
        Time.fixedDeltaTime = startFixedDeltaTime;
        Debug.Log(Time.timeScale);
        Debug.Log(Time.fixedDeltaTime);
    }

    IEnumerator FogLastingTime()
    {
        yield return new WaitForSeconds(15f);
        StopFog();
    }
    public int GetCoins()
    {
        return PlayerPrefs.GetInt("coins");
    }
    IEnumerator DelayedCameraShake(float delay, float strength)
    {
        yield return new WaitForSeconds(delay);
        ShakeCamera(strength);
    }

    public void ChangePlayer(int index)
    {
        if (isBought[index] == 1)
        {
            for(int i = 0; i < characters.Length;i++)
            {
                if (characters[i].activeSelf)
                {
                    TextMeshProUGUI selectedText = buttonsBuy[i].GetComponentInChildren<TextMeshProUGUI>();
                    selectedText.text = "SELECT";
                    isBought[i] = 1;
                }
                characters[i].SetActive(false);
            }
            characters[index].SetActive(true);
            TextMeshProUGUI buttonText = buttonsBuy[index].GetComponentInChildren<TextMeshProUGUI>();
            buttonText.text = "SELECTED";
            isBought[index] = 2;
        }
        else if(isBought[index] != 2)
        {
            if(coins - 25 >= 0)
            {
                DecreaseCoin(25); ;
                SetPrefs(coins, highScore);
                LoadPrefs();
                isBought[index] = 1;
                TextMeshProUGUI buttonText = buttonsBuy[index].GetComponentInChildren<TextMeshProUGUI>();
                buttonText.text = "SELECT";
            }

        }
        SetPrefsCharacter(isBought);
    }
    internal void DecreaseCoin(int cost)
    {
        ScoreCount coinCounter = FindObjectOfType<ScoreCount>();
        if (coinCounter != null)
        {
            coinCounter.DecreaseCoin(cost);
        }
    }
}

