using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Platform : MonoBehaviour
{
    float minGap = 4f; 
    float baseYValue = -7f;
    float maxGap = 8f;
    float yOffsetUp = 1f;
    float yOffsetDown = 3f;
    float yOffset = 1.4f;
    public float platformSpeed = 0.7f;
    float minWidth = 0.8f;
    float maxWidth = 1.2f;
    Platform spawnedPlatform = null;
    GameObject coin;
    GameObject model;
    private float coinSpawnChance = 0.3f;
    float platformSpeedVertical = 0f;
    private bool hasCoin = false;
    private bool animationPlayed = false;

    public Sprite[] sprites;

    // Start is called before the first frame update
    void Start()
    {
        model = transform.GetChild(0).gameObject;
        coin = model.transform.GetChild(0).gameObject;

        if (hasCoin)
        {
            coin.SetActive(false);
        SpriteRenderer render = GetComponent<SpriteRenderer>();
        if (sprites.Length > 0 )
        {
            int random = Random.Range( 0,sprites.Length );
            render.sprite = sprites[random];
        }
    }

    public void SetPosAndSpeed(Vector2 position, float speed)
    {       
        platformSpeed = speed;
        float randomWidth = Random.Range(minWidth, maxWidth);
        transform.localScale = new Vector3(randomWidth, 1, 0);
        Vector2 offset = new Vector2(Random.Range(randomWidth + minGap, maxGap), Random.Range(-yOffsetDown, yOffsetUp));
        position.y = baseYValue;
        transform.position = position + offset;

        TrySpawnCoin();
    }      
    
    public void TrySpawnCoin()
    {
        if (Random.value < coinSpawnChance)
        {            
            hasCoin = true;
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

        if (transform.position.x < 12 && !animationPlayed)
        {
            animationPlayed = true;
            model.GetComponent<ModelScript>().PlayAnimation();
        }

        platformSpeed += 0.0005f * Time.deltaTime;

        transform.position = new Vector2(transform.position.x - platformSpeed * Time.deltaTime, transform.position.y - platformSpeedVertical * Time.deltaTime);
    }

    public void DropPlatform()
    {
        platformSpeedVertical = 10f;
    }    
}
