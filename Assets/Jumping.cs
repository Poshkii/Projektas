using System.Collections;
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
    public int lives = 1;
    private bool respawned = false;

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

        //If player is invisible to camera
        if (!spriteRend.isVisible && !respawned)
        {
            if (flagInvisible)
            {
                OutOfScreen();
            }                
            else
                flagInvisible = true;
        }
    }   

    //Player is outside of the camera view
    private void OutOfScreen()
    {
        lives--;
        //Dies
        if (lives < 1)
        {
            scoreCounter.Death();
            transform.position = startPos;
            body.velocity = Vector2.zero;
        }
        //Respawns
        else
        {
            respawned = true;
            StartCoroutine(WaitAfterRespawn(2f));            
            body.transform.position = FindNearestPlatform();
            body.velocity = Vector3.zero;
        }
    }

    //Finds the nearest platform to the player
    private Vector3 FindNearestPlatform()
    {
        GameObject[] platforms = GameObject.FindGameObjectsWithTag("Platform");
        float minDistance = Vector3.Distance(transform.position, platforms[0].transform.position);
        Vector3 platformPos = platforms[0].transform.position;
        foreach (GameObject plat in platforms)
        {
            float distance = Vector3.Distance(transform.position, plat.transform.position);

            if (distance < minDistance)
            {
                minDistance = distance;
                platformPos = plat.transform.position;
                platformPos.y += 15f;
                platformPos.x -= 1.5f * plat.GetComponent<Platform>().platformSpeed;
            }
        }
        return platformPos;
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

    //Wait before checking if player is outside the view
    IEnumerator WaitAfterRespawn(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        respawned = false;
    }
}