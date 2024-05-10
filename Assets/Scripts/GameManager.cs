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
    public TMP_Text coinsTextShop;
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
    public GameObject[] worlds;
    public Button[] buttonsBuy;
    public Button[] buttonsWorldBuy;
    public int[] isBought;
    public int[] worldsBought;
    public int[] pricesForCharacters;
    private bool shakeCamera = false;
    private float duration = 0f;
    private Vector3 cameraStartPos;
    private const float baseShakeStrength = 0.2f;
    private float ShakeStrength;
    AudioManager audioManager;
    public ParticleSystem selectedParticles;
    public ParticleSystem[] allParticles;
    public PlatformSpawner platformSpawner;
    private float startTimeScale;
    private float startFixedDeltaTime;
    internal bool gameStarted = false;

    public Sprite[] sprites_mountains;
    public Sprite[] sprites_winter;
    public Sprite[] sprites_desert;
    private Sprite[] sprites_active;
    PlatformSpawner spawner;

    private List<int> scores = new List<int>();
    private void Awake()
    {
        isBought = Enumerable.Repeat(0, characters.Length).ToArray();
        pricesForCharacters = new int[] { 5, 10, 15 };
        worldsBought = Enumerable.Repeat(0, worlds.Length).ToArray();
        worldsBought[0] = 2;
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
        birdSpawner = birdSpawnerObj.GetComponent<BirdSpawner>();
    }
    // Start is called before the first frame update
    void Start()
    {
        spawner = FindObjectOfType<PlatformSpawner>();
        selectedParticles = allParticles[0];
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
        if (PlayerPrefs.HasKey("mountains") || PlayerPrefs.HasKey("desert") || PlayerPrefs.HasKey("winter"))
        {
            LoadPrefsWorld();
            LoadPlatformPrefs();
        }
        else
        {
            SetPrefsWorld(worldsBought);
            SetPlatformPrefs(0);
        }
        
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
        coinsTextShop.text = "Coins: " + coins;
        SetPrefs(coins, highScore);
    }

    public void PlayAgain()
    {
        Time.timeScale = 1f;
        deathScreenUI.gameObject.SetActive(false);
        gameUI.gameObject.SetActive(true);        
        Instantiate(starterPlatform);
        StopFog();
        foreach (ParticleSystem ps in allParticles)
        {
            ps.Clear();
        }
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
            if (character.CompareTag("Player"))
                player = character;
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

    public void SetPrefsWorld(int[] x)
    {
        PlayerPrefs.SetInt("mountains", x[0]);
        PlayerPrefs.SetInt("desert", x[1]);
        PlayerPrefs.SetInt("winter", x[2]);
    }

    public void LoadPrefsWorld()
    {
        int[] x = new int[worldsBought.Length];
        x[0] = PlayerPrefs.GetInt("mountains");
        x[1] = PlayerPrefs.GetInt("desert");
        x[2] = PlayerPrefs.GetInt("winter");        

        for(int i = 0; i < 3; i++)
        {
            worldsBought[i] = x[i];
            ChangeButtonText(x[i], buttonsWorldBuy[i], worlds[i]);
            if (x[i] == 2)
            {
                selectedParticles = allParticles[i];
            }
        }
    }

    public void SetPlatformPrefs(int index)
    {
        if (index == 0)
        {
            sprites_active = sprites_mountains;
            Debug.Log("default");
        }
        else if (index == 1)
        {
            sprites_active = sprites_desert;
            Debug.Log("desert");
        }
        else if (index == 2)
        {
            sprites_active = sprites_winter;
            Debug.Log("winter");
        }
        else
        {
            sprites_active = sprites_mountains;
            Debug.Log("default");
        }
        Debug.Log(sprites_active.Length);
    }

    public void LoadPlatformPrefs()
    {
        int[] x = new int[worldsBought.Length];
        x[0] = PlayerPrefs.GetInt("mountains");
        x[1] = PlayerPrefs.GetInt("desert");
        x[2] = PlayerPrefs.GetInt("winter");
        if (x[0] == 2)
        {
            sprites_active = sprites_mountains;
            Debug.Log("default");
        }
        else if (x[1] == 2)
        {
            sprites_active = sprites_desert;
            Debug.Log("desert");
        }
        else if (x[2] == 2)
        {
            sprites_active = sprites_winter;
            Debug.Log("winter");
        }
        else
        {
            sprites_active = sprites_mountains;
            Debug.Log("default");
        }
        Debug.Log(sprites_active.Length);
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
            //StartCoroutine(DoubleJumpLastingTime());
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
            //StartCoroutine(SlowTimeLastingTime());
        }
        boosterManager.AddBooster(type, duration);
        pickupTextAnim.Play("BoosterPickupTextAnim", -1, 0f);
        audioManager.PlaySFX(audioManager.powerUp);
    }

    public void RemoveBooster(string type)
    {
        switch (type)
        {
            case "ExtraLife":

                break;
            case "ExtraJump":
                SetJump(1);
                break;
            case "DoubleCoins":
                scoreCount.SetMultiplier(1);
                break;
            case "SlowTime":
                Time.timeScale = startTimeScale;
                Time.fixedDeltaTime = startFixedDeltaTime;
                break;
            default:
                break;
        }
    }

    public void StartFog()
    {
        selectedParticles.Play();
        StartCoroutine(FogLastingTime());
    }
    public void StopFog()
    {
        foreach (var particles in allParticles)
        {
            particles.Stop();
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
            player = characters[index];
            TextMeshProUGUI buttonText = buttonsBuy[index].GetComponentInChildren<TextMeshProUGUI>();
            buttonText.text = "SELECTED";
            isBought[index] = 2;
        }
        else if(isBought[index] != 2)
        {
            if(coins - pricesForCharacters[index] >= 0)
            {
                DecreaseCoin(pricesForCharacters[index]); ;
                SetPrefs(coins, highScore);
                LoadPrefs();
                isBought[index] = 1;
                TextMeshProUGUI buttonText = buttonsBuy[index].GetComponentInChildren<TextMeshProUGUI>();
                buttonText.text = "SELECT";
            }

        }
        SetPrefsCharacter(isBought);
    }

    public void SelectWorld(int index)
    {
        //Buy
        if (worldsBought[index] == 0)
        {
            if (coins >= 50)
            {
                DecreaseCoin(50);
                SetPrefs(coins, highScore);
                LoadPrefs();
                worldsBought[index] = 1;
                buttonsWorldBuy[index].GetComponentInChildren<TMP_Text>().text = "SELECT";
            }
        }

        //Select
        else if (worldsBought[index] == 1)
        {
            for(int i = 0; i < worlds.Length; i++)
            {
                worlds[i].SetActive(false);
                if (worldsBought[i] == 2)
                {
                    worldsBought[i] = 1;
                    buttonsWorldBuy[i].GetComponentInChildren<TMP_Text>().text = "SELECT";
                }
            }
            worldsBought[index] = 2;
            worlds[index].SetActive(true);
            buttonsWorldBuy[index].GetComponentInChildren<TMP_Text>().text = "SELECTED";
            selectedParticles = allParticles[index];
            SetPlatformPrefs(index);
            spawner.Restart();
        }
        SetPrefsWorld(worldsBought);
    }

    internal void DecreaseCoin(int cost)
    {
        ScoreCount coinCounter = FindObjectOfType<ScoreCount>();
        if (coinCounter != null)
        {
            coinCounter.DecreaseCoin(cost);
        }
    }

    internal Sprite SetSprite()
    {
        int random = UnityEngine.Random.Range(0, sprites_active.Length);
        return sprites_active[random];
    }
}

