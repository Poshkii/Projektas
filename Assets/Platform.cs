using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Platform : MonoBehaviour
{
    Vector2 startPos = new Vector2 (10, -7);
    float minGap = 0.5f;
    float maxGap = 3;
    float yOffset = 1.5f;
    float spawnDelay = 1.5f;
    float platformSpeed = 1.5f;
    float minWidth = 0.5f;
    float maxWidth = 2f;
    bool screenFilled = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void Spawn(Vector2 position)
    {        
        float randomWidth = Random.Range(minWidth, maxWidth);
        transform.localScale = new Vector3(randomWidth, 10, 0);
        Vector2 offset = new Vector2(Random.Range(randomWidth + minGap, maxGap), Random.Range(-yOffset, yOffset));
        position.y = -8f;
        transform.position = position + offset;

        StartCoroutine(waitToSpawn(spawnDelay));        
    }     

    public void SpawnRecusively(int count)
    {
        if (count > 0)
        {
            count--;
            SpawnPlatform().SpawnRecusively(count);
        }
    }

    Platform SpawnPlatform()
    {
        GameObject spawnedPlatform = Instantiate(gameObject);
        Platform platform = spawnedPlatform.GetComponent<Platform>();
        platform.Spawn(transform.position);
        return platform;
    }
   
    // Update is called once per frame
    void Update()
    {       
        if (transform.position.x < -12)
        {           
            Destroy(gameObject);
        }
        transform.position = new Vector2(transform.position.x - platformSpeed * Time.deltaTime, transform.position.y);
    }

    IEnumerator waitToSpawn(float delay)
    {
        yield return new WaitForSeconds(delay);

        SpawnPlatform();
    }
}
