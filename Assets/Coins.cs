using UnityEngine;

public class Coins : MonoBehaviour
{
    //private int value = 1;
    public AudioClip collectSound;
    private AudioSource source;
    public float volume = 1.0f;
    public float rotation = 150f;
    private bool collected = false;

    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
        if (source == null)
            source = gameObject.AddComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        SpinCoin();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!collected && other.CompareTag("Player"))
            AddCoin();
    }

    internal void AddCoin()
    {
        if (collectSound != null)
            AudioSource.PlayClipAtPoint(collectSound, Camera.main.transform.position, volume);

        CoinCollect coinCounter = FindObjectOfType<CoinCollect>();
        if (coinCounter != null)
            coinCounter.IncreaseCoins();

        collected = true;

        Destroy(gameObject);

    }

    void SpinCoin()
    {
        // Rotate the coin gradually over time
        transform.Rotate(Vector3.up, rotation * Time.deltaTime);
    }
}
