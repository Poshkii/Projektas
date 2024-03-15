using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdMovement : MonoBehaviour
{
    public float moveSpeed = 8f;

    void Update()
    {
        // Move the object from right to left
        transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);

        // Destroy the object if it goes off the screen
        if (transform.position.x < Camera.main.transform.position.x - Camera.main.orthographicSize * Camera.main.aspect - 1)
        {
            Destroy(gameObject);
        }
    }
}
