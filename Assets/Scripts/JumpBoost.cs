using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpBoost : MonoBehaviour
{
    public float duration = 10f; // Time how long the booster lasts
    public float rotation = 150f; // coin spin speed (optional)
    private bool collected = false; // prevents duplicate pickups on collision

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        SpinBooster();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!collected && other.CompareTag("Player"))
        {
            collected = true;

            // Destroy the collectible item
            Destroy(gameObject);

            // Trigger the increase in jump height
            other.GetComponent<Player>().IncreaseJump(duration);
        }
    }

    void SpinBooster()
    {
        transform.Rotate(Vector3.up, rotation * Time.deltaTime);
    }
}
