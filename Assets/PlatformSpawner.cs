using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformSpawner : MonoBehaviour
{
    [SerializeField] GameObject platform;
    Vector2 startPos = new Vector2(-12, -2);
    public bool screenIsFilled = false;

    // Start is called before the first frame update
    void Start()
    {
        GameObject spawnedPlatform = Instantiate(platform);
        Platform platformScript = spawnedPlatform.GetComponent<Platform>();
        platformScript.SetPosAndSpeed(startPos, 0.7f);
        platformScript.SpawnRecusively(8);
    }    

    // Update is called once per frame
    void Update()
    {
        
    }
}
