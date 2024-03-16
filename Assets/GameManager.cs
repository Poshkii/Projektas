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
    public TMP_Text scoreText;
    public TMP_Text highscoreText;
    public TMP_Text coinsText;
    public TMP_Text scoreboardText;
    public GameObject player;    
    public GameObject starterPlatform;
    public GameObject activeBefore;

    private List<int> scores = new List<int>();

    // Start is called before the first frame update
    void Start()
    {
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
    }

    // Update is called once per frame
    void Update()
    {
        
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
        Instantiate(starterPlatform);
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
}
