using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdSpawner : MonoBehaviour
{
    public GameObject bird;
    public float minSpawnRate = 1f;
    public float maxSpawnRate = 3f;
    public float moveSpeed = 6f;

    private float nextSpawn;
    private Camera mainCamera;

    //void Start()
    //{
    //    nextSpawn = Time.time + Random.Range(minSpawnRate, maxSpawnRate);
    //    mainCamera = Camera.main;
    //}

    //void Update()
    //{
    //    if (Time.time > nextSpawn)
    //    {
    //        SpawnObject();
    //        nextSpawn = Time.time + Random.Range(minSpawnRate, maxSpawnRate);
    //    }
    //}

    public void SpawnObject()
    {
        GameObject newBird = Instantiate(bird, transform.position, transform.rotation);
        BirdMovement mover = newBird.GetComponent<BirdMovement>();
        if (mover != null)
        {
            mover.moveSpeed = moveSpeed;
        }
    }
}

