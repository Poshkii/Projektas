using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Platform : MonoBehaviour
{
    private float randomWidth = 1f;
    float minGap = 4f;
    float baseYValue = -6f;
    float maxGap = 8f;
    float yOffsetUp = 1f;
    float yOffsetDown = 2f;
    public float platformSpeed = 0.7f;
    float platformSpeedVertical = 0f;
    float minWidth = 0.8f;
    float maxWidth = 1.2f;
    Platform spawnedPlatform = null;
    GameObject coin;
    GameObject model;
    GameObject extraLife;
    GameObject extraJump;
    GameObject doubleCoins;
    GameObject slowTime;
    private float coinSpawnChance = 0.4f;
    private float boosterSpawnChance = 0.15f;

    private bool hasCoin = false;
    private bool hasExtraLife = false;
    private bool hasExtraJump = false;
    private bool hasDoubleCoins = false;
    private bool hasSlowTime = false;
    private bool animationPlayed = false;
    public bool dropped = false;

    // Start is called before the first frame update
    void Start()
    {
        model = transform.GetChild(0).gameObject;
        coin = transform.transform.GetChild(1).gameObject;
        extraLife = transform.transform.GetChild(2).gameObject;
        extraJump = transform.transform.GetChild(3).gameObject;
        doubleCoins = transform.transform.GetChild(4).gameObject;
        slowTime = transform.transform.GetChild(5).gameObject;

        model.transform.localScale = new Vector3(randomWidth, 1, 0);

        coin.SetActive(hasCoin);
        extraLife.SetActive(hasExtraLife);
        extraJump.SetActive(hasExtraJump);
        doubleCoins.SetActive(hasDoubleCoins);
        slowTime.SetActive(hasSlowTime);
    }

    public void SetPosAndSpeed(Vector2 position, float speed)
    {       
        platformSpeed = speed;
        randomWidth = Random.Range(minWidth, maxWidth);
        //transform.localScale = new Vector3(randomWidth, 1, 0);
        Vector2 offset = new Vector2(Random.Range(randomWidth + minGap, maxGap), Random.Range(-yOffsetDown, yOffsetUp));
        position.y = baseYValue;
        transform.position = position + offset;

        TrySpawnCoin();
        if (!hasCoin)
        {
            TrySpawnBooster();
        }
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

    private void TrySpawnBooster()
    {
        if (Random.value < boosterSpawnChance)
        {
            float val = Random.value;
            if (val < 0.1f)
            {
                hasExtraLife = true;
            }
            else if (val < 0.3f)
            {
                hasExtraJump = true;
            }
            else if (val < 0.6f)
            {
                hasSlowTime = true;
            }
            else
            {
                hasDoubleCoins = true;
            }
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

        float logFactor = 0.01f; 
        platformSpeed += logFactor * Mathf.Log(platformSpeed + 1) * Time.deltaTime;
        transform.position = new Vector2(transform.position.x - platformSpeed * Time.deltaTime, transform.position.y - platformSpeedVertical * Time.deltaTime);
    }

    public void DropPlatform()
    {
        if (!dropped)
        {
            dropped = true;
            model.GetComponent<ModelScript>().DropAnimation();
        }
    }    
}
