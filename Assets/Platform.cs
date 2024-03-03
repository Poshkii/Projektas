using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Platform : MonoBehaviour
{
    Vector2 startPos = new Vector2 (10, -7);
    float minGap = 1f;
    float maxGap = 4;
    float yOffset = 1.5f;
    float spawnDelay = 1.5f;
    float platformSpeed = 1.5f;
    float minWidth = 0.5f;
    float maxWidth = 2f;
    bool screenFilled = false;
    Platform spawnedPlatform = null;

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

        //StartCoroutine(waitToSpawn(spawnDelay));        
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
            spawnedPlatform.Spawn(transform.position);
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
        if (transform.position.x < -12)
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
}
