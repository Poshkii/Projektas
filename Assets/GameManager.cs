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
    public TMP_Text scoreText;
    public TMP_Text highscoreText;
    public TMP_Text coinsText;
    public TMP_Text scoreboardText;
    public GameObject player;    
    public GameObject starterPlatform;

    private List<int> scores = new List<int>();

    // Start is called before the first frame update
    void Start()
    {
        Instantiate(starterPlatform);
        deathScreenUI.gameObject.SetActive(false);
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
        deathScreenUI.gameObject.SetActive(true);
        optionsPanelUI.gameObject.SetActive(false);
    }
    public void OpenOptions()
    {
        deathScreenUI.gameObject.SetActive(false);
        optionsPanelUI.gameObject.SetActive(true);
        gameUI.gameObject.SetActive(false);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
