using Unity.VisualScripting;
using UnityEngine;

public class Coins : MonoBehaviour
{
    public AudioClip collectSound; // main pickup sound
    private AudioSource source; // object for playing pickup sound
    public float volume = 1.0f; // volume of pickup sound (optional)
    public float rotation = 150f; // coin spin speed (optional)
    private bool collected = false; // prevents duplicate pickups on collision

    void Start()
    {
        source = GetComponent<AudioSource>();
        if (source == null)
            source = gameObject.AddComponent<AudioSource>();
    }

    void Update()
    {
        //SpinCoin();
    }

    // main collision event to trigger a coin pickup
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!collected && other.CompareTag("Player"))
            AddCoin();
    }

    internal void AddCoin()
    {
        //prevents duplicate pickups on 1 coin
        collected = true;

        // removes coin form the scene once picked up
        Destroy(gameObject);

        // plays pickup sound
        if (collectSound != null)
            AudioSource.PlayClipAtPoint(collectSound, Camera.main.transform.position, volume);

        // updates coin counter
        ScoreCount coinCounter = FindObjectOfType<ScoreCount>();
        if (coinCounter != null)
        {
            int multiplyer = coinCounter.GetMultiplyer();
            coinCounter.AddCoin(multiplyer);
        }
        else
            coinCounter.AddCoin(1);
    }

    // (Optional) spinning animation for visuals
    void SpinCoin()
    {
        transform.Rotate(Vector3.up, rotation * Time.deltaTime);
    }
}
