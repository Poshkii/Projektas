using UnityEngine;
using UnityEngine.UI;

public class Jumping : MonoBehaviour
{
    public float jumpForce = 10f;
    public float sideJump = 5f;
    public float maxTime = 1f;
    public float minJump = 5f;

    public Image jumpIndicator;

    private float heldTime = 0f;
    private Rigidbody2D body;
    private bool isJumping = false; // checks if mid-air
    private bool jumpInit = false; // checks if jump was started
    private bool allowJump = true; // either allows or prevents performing a jump

    private void Start()
    {
        body = GetComponent<Rigidbody2D>();

        // Initialize jump strength indicator
        if (jumpIndicator != null)
        {
            jumpIndicator.enabled = true;
            jumpIndicator.fillAmount = 0f;
        }
    }

    private void Update()
    {
        // checks if a touch input was performed during a jump before character landed.
        // if that was the case it prevents touch input from carrying over to prevent missclicks/unintentional jumps
        if (!isJumping && Input.touchCount == 0 && !allowJump)
        {
            allowJump = true;
            Debug.Log("jumping allowed");
        }

        // start of touch input to reset touch duration measurement
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began && !isJumping)
        {
            heldTime = 0f;
            jumpInit = false;
        }


        // times and updates held touch input, initializes jump strength indicator
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Stationary && !isJumping && !jumpInit)
        {
            heldTime += Time.deltaTime;
            heldTime = Mathf.Clamp(heldTime, 0f, maxTime);

            UpdateJumpIndicator();
        }

        // jumping is perfomed if conditions are met and indicator is reset
        // inverts certain checks to perform input control
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended && !isJumping && allowJump)
        {
                Jump();
                jumpInit = true;
                allowJump = false;
                Debug.Log("jumping not allowed");
            

            if (jumpIndicator != null)
                jumpIndicator.fillAmount = 0f;
        }

        // If movement vector is 0 that means body has landed on ground and can jump again
        if (body.velocity.magnitude == 0 && !allowJump)
            isJumping = false;

    }

    // Applies jumping vector to implement jumping and disables multi jumping mid-air 
    private void Jump()
    {
        isJumping = true;
        float jumpHeight = Mathf.Lerp(minJump, jumpForce, heldTime / maxTime);
        body.velocity = new Vector2(sideJump, jumpHeight);
    }

    // Displays an indicator of how strong the jump is depending on how long touch input was
    private void UpdateJumpIndicator()
    {
        if (jumpIndicator != null)
            jumpIndicator.fillAmount = heldTime / maxTime;

    }
}