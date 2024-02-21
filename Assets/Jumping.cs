using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class Jumping : MonoBehaviour
{
    public float jumpForce = 10f;
    public float sideJump = 5f;
    public float maxTime = 1f;
    public float minJump = 5f;

    private float heldTime = 0f;
    private Rigidbody2D body;
    private bool isJumping = false; // checks if mid-air

    private void Start()
    {
        body = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        // start of touch input to reset touch length measurement
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began && !isJumping)
            heldTime = 0f;

        // times and updates held touch input
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Stationary && !isJumping)
        {
            heldTime += Time.deltaTime;
            heldTime = Mathf.Clamp(heldTime, 0f, maxTime);
        }

        // once touch input has ended jumping is initiated
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended && !isJumping)
            Jump();

        // If movement vector is 0 that means body has landed on ground and can jump again
        if (body.velocity.magnitude == 0)
            isJumping = false;
            
    }

    // Applies jumping vector to implement jumping and disables multi jumping mid-air 
    private void Jump()
    {
        isJumping = true;
        float jumpHeight = Mathf.Lerp(minJump, jumpForce, heldTime / maxTime);
        body.velocity = new Vector2(sideJump, jumpHeight);
    }
}