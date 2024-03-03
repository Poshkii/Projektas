using UnityEngine;
using UnityEngine.UI;

public class CoinCollect : MonoBehaviour
{
    public Text coinText;
    private int coinCount = 0;

    void Start()
    {
    }

    void Update()
    {
    }

    // Updates UI text tied to coin counter
    internal void IncreaseCoins()
    {
        coinCount++;
        if (coinText != null)
            coinText.text = "Coins: " + coinCount.ToString();
    }
}
