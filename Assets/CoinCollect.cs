using UnityEngine;
using UnityEngine.UI;

public class CoinCollect : MonoBehaviour
{
    public Text coinText;
    private int coinCount = 0;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    internal void IncreaseCoins()
    {
        coinCount++;
        if (coinText != null)
            coinText.text = "Coins: " + coinCount.ToString();
    }
}
