using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    Vector2 startPos = new Vector2 (10, -7);
    float xOffset = 2;
    float yOffset = 3;
    float spawnDelay = 1;
    float platformSpeed = 0.05f;

    // Start is called before the first frame update
    void Start()
    {
        Vector2 offset = new Vector2 (Random.Range(0, xOffset), Random.Range(-xOffset, yOffset));
        transform.position = startPos + offset;
        //transform.localScale = new Vector3(1, 10, 0);

        StartCoroutine(waitToSpawn(spawnDelay));
        Destroy(gameObject, 15f);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = new Vector2(transform.position.x - platformSpeed, transform.position.y);
    }

    IEnumerator waitToSpawn(float delay)
    {
        yield return new WaitForSeconds(delay);

        Instantiate(gameObject);
    }
}
