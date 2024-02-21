using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jumping : MonoBehaviour
{
    public float jumpForce = 10f;
    public float sideJump = 5f;
    public float maxTime = 1f;
    public float minJump = 5f;

    private float heldTime = 0f;
    private Rigidbody2D body;
    private bool jumping = false; // Checks if mid-air

    private void Start()
    {
        body = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began && !jumping)
            heldTime = 0f;

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Stationary && !jumping)
        {
            heldTime += Time.deltaTime;
            heldTime = Mathf.Clamp(heldTime, 0f, maxTime);
        }

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended && !jumping)
            Jump();       
    }

    private void Jump()
    {
        jumping = true;
        float jumpHeight = Mathf.Lerp(minJump, jumpForce, heldTime / maxTime);
        body.velocity = new Vector2(sideJump, jumpHeight);
    }

    // Unity callback method. Triggers when object colides with other objects with tag "Platform"
    // All generated platforms need to have added tag "Platform" for this to work
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
            // Allows to jump once landed
            jumping = false;

            // Removes horizontal sliding
            body.velocity = Vector2.zero;
        }
    }
}
