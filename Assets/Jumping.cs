using UnityEngine;
using UnityEngine.SceneManagement;
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
    private bool isJumping = false;
    private bool allowJump = true; // either allows or prevents performing a jump
    public Transform groundCheck;
    private float groundCheckRadius = 0.7f;
    public LayerMask whatIsGround;
    public SpriteRenderer spriteRend;
    private bool flagInvisible = false;
    public GameObject gameManagerObj;
    ScoreCount scoreCounter;
    Vector3 startPos = new Vector3(-10, 2, 0);

    private void Start()
    {
        body = GetComponent<Rigidbody2D>();
        spriteRend = GetComponent<SpriteRenderer>();

        // Initialize jump strength indicator
        if (jumpIndicator != null)
        {
            jumpIndicator.enabled = true;
            jumpIndicator.fillAmount = 0f;
        }
        scoreCounter = gameManagerObj.GetComponent<ScoreCount>();
    }

    private void Update()
    {        
        // checks if a touch input was performed during a jump before character landed.
        // if that was the case it prevents touch input from carrying over to prevent missclicks/unintentional jumps
        //if (!isJumping && Input.touchCount == 0 && !allowJump)
        //{
        //    allowJump = true;
        //    Debug.Log("jumping allowed");
        //}

        // start of touch input to reset touch duration measurement
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began && allowJump && !isJumping)
        {
            heldTime = 0f;
        }


        // times and updates held touch input, initializes jump strength indicator
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Stationary && allowJump && !isJumping)
        {
            heldTime += Time.deltaTime;
            heldTime = Mathf.Clamp(heldTime, 0f, maxTime);

            UpdateJumpIndicator();
        }

        // jumping is perfomed if conditions are met and indicator is reset
        // inverts certain checks to perform input control
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended && allowJump && !isJumping)
        {
            Jump();
            isJumping = true;
            if (jumpIndicator != null)
                jumpIndicator.fillAmount = 0f;
        }

        // If movement vector is 0 that means body has landed on ground and can jump again
        //if (body.velocity.magnitude == 0 && !allowJump)
        //    isJumping = false;

        //Checks for a platform under the player. If body has landed it can jump again
        //RaycastHit2D groundCheck = Physics2D.Raycast(transform.position, -transform.up, 1f);
        //Debug.DrawRay(transform.position, -transform.up, Color.red);
        //if (groundCheck.collider != null && groundCheck.collider.CompareTag("Platform"))
        //{
        //    isJumping = false;
        //}

        allowJump = Physics2D.OverlapCircle(groundCheck.transform.position, groundCheckRadius, whatIsGround);

        if (isJumping && allowJump && Input.touchCount == 0)
        {
            isJumping = false;
            heldTime = 0f;
            jumpIndicator.fillAmount = 0f;
        }

        if (!spriteRend.isVisible)
        {
            if (flagInvisible)
            {
                scoreCounter.Death();
                transform.position = startPos;
                body.velocity = Vector2.zero;
            }
                //SceneManager.LoadSceneAsync(2);
                
            else
                flagInvisible = true;
        }
    }    

    // Applies jumping vector to implement jumping and disables multi jumping mid-air 
    private void Jump()
    {
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