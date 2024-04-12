using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Platform : MonoBehaviour
{
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
    private float coinSpawnChance = 0.4f;
    private float boosterSpawnChance = 0.15f;

    private bool hasCoin = false;
    private bool hasExtraLife = false;
    private bool hasExtraJump = false;
    private bool hasDoubleCoins = false;
    private bool animationPlayed = false;

    // Start is called before the first frame update
    void Start()
    {
        model = transform.GetChild(0).gameObject;
        coin = model.transform.GetChild(0).gameObject;
        extraLife = model.transform.GetChild(1).gameObject;
        extraJump = model.transform.GetChild(2).gameObject;
        doubleCoins = model.transform.GetChild(3).gameObject;

        coin.SetActive(hasCoin);
        extraLife.SetActive(hasExtraLife);
        extraJump.SetActive(hasExtraJump);
        doubleCoins.SetActive(hasDoubleCoins);
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
            if (val < 0.3f)
            {
                hasExtraLife = true;
            }
            else if (val < 0.5f)
            {
                hasExtraJump = true;
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

        platformSpeed += 0.0005f * Time.deltaTime;

        transform.position = new Vector2(transform.position.x - platformSpeed * Time.deltaTime, transform.position.y - platformSpeedVertical * Time.deltaTime);
    }

    public void DropPlatform()
    {
        model.GetComponent<ModelScript>().DropAnimation();
    }    
}
