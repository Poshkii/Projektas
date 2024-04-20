using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformSpawner : MonoBehaviour
{
    [SerializeField] GameObject platform;
    Vector2 startPos = new Vector2(-12, -2);

    // Start is called before the first frame update
    void Start()
    {
        SpawnPlatforms();
    }    

    public void SpawnPlatforms()
    {
        GameObject spawnedPlatform = Instantiate(platform);
        Platform platformScript = spawnedPlatform.GetComponent<Platform>();

        platformScript.SetPosAndSpeed(startPos, 0.7f);
        platformScript.SpawnRecusively(8);
    }

    private void DestroyAllPlatforms()
    {
        GameObject[] platforms = GameObject.FindGameObjectsWithTag("Platform");
        foreach (GameObject platform in platforms)
        {
            Destroy(platform);
        }
    }

    public void Restart()
    {
        DestroyAllPlatforms();
        SpawnPlatforms();
    }
}
