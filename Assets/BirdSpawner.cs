using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdSpawner : MonoBehaviour
{
    public GameObject bird;
    public float minSpawnRate = 1f;
    public float maxSpawnRate = 3f;
    public float moveSpeed = 2f;

    private float nextSpawn;
    private Camera mainCamera;

    void Start()
    {
        nextSpawn = Time.time + Random.Range(minSpawnRate, maxSpawnRate);
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (Time.time > nextSpawn)
        {
            SpawnObject();
            nextSpawn = Time.time + Random.Range(minSpawnRate, maxSpawnRate);
        }
    }

    void SpawnObject()
    {
        Vector3 spawnPosition = mainCamera.ViewportToWorldPoint(new Vector3(1.1f, 0.9f, 0));
        GameObject newBird = Instantiate(bird, transform.position, transform.rotation);
        newBird.tag = "Bird";
        newBird.SetActive(true);
        BirdMovement mover = newBird.GetComponent<BirdMovement>();
        if (mover != null)
        {
            mover.moveSpeed = moveSpeed;
        }
    }
}

