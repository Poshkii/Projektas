using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Platform : MonoBehaviour
{
    float minGap = 5f;
    float baseYValue = -5f;
    float maxGap = 10f;
    float yOffset = 1.5f;
    public float platformSpeed = 0.7f;
    float minWidth = 0.8f;
    float maxWidth = 1.2f;
    Platform spawnedPlatform = null;
    GameObject coin;
    private float coinSpawnChance = 0.3f;

    // Start is called before the first frame update
    void Start()
    {
               
    }

    public void SetPosAndSpeed(Vector2 position, float speed)
    {
        coin = transform.GetChild(0).gameObject;
        coin.SetActive(false);

        platformSpeed = speed;
        float randomWidth = Random.Range(minWidth, maxWidth);
        coin.transform.parent = null;
        transform.localScale = new Vector3(randomWidth, 1, 0);
        coin.transform.SetParent(transform);
        Vector2 offset = new Vector2(Random.Range(randomWidth + minGap, maxGap), Random.Range(-yOffset, yOffset));
        position.y = baseYValue;
        transform.position = position + offset;

        TrySpawnCoin();
    }    
    
    public void TrySpawnCoin()
    {
        if (Random.value < coinSpawnChance)
        {            
            coin.SetActive(true);
        }
    }

    public void SpawnRecusively(int count)
    {
        if (count > 0)
        {
            count--;
            SpawnPlatform();
            spawnedPlatform.SpawnRecusively(count);
        }
    }

    void SpawnPlatform()
    {
        if (spawnedPlatform == null)
        {
            spawnedPlatform = Instantiate(gameObject).GetComponent<Platform>();
            spawnedPlatform.SetPosAndSpeed(transform.position, platformSpeed);
        }        
    }

    public void SpawnOnLast()
    {
        if (spawnedPlatform != null)
        {
            spawnedPlatform.SpawnOnLast();
        }
        else
        {
            SpawnPlatform();
        }
    }
   
    // Update is called once per frame
    void Update()
    {       
        if (transform.position.x < -13)
        {           
            SpawnOnLast();
            Destroy(gameObject);
        }
        transform.position = new Vector2(transform.position.x - platformSpeed * Time.deltaTime, transform.position.y);
    }

    IEnumerator waitToSpawn(float delay)
    {
        yield return new WaitForSeconds(delay);

        SpawnPlatform();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        collision.transform.SetParent(transform);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        collision.transform.SetParent(null);
    }
}
