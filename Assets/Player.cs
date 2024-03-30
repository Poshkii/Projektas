using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    // Control options
    public float jumpForce = 10f;
    public float sideJump = 4f;
    public float maxTime = 1f;
    public float minJump = 2f;
    private int lives = 2;
    private float heldTime = 0f;

    // Object options
    private Rigidbody2D body;
    public PhysicsMaterial2D bounceMaterial;
    public PhysicsMaterial2D frictionMaterial;
    private EdgeCollider2D bounceMain;
    private BoxCollider2D box;
    public Image jumpIndicator;

    // Jumping options
    private bool isJumping = false;
    private bool allowJump = true;

    // Jump boost options
    private bool isJumpBoosted = false;
    private float jumpBoostTimer = 0f;
    private float originalSideJump;

    // Game scene options
    private bool respawned = false;
    private bool flagInvisible = false;
    public GameObject gameManagerObj;
    GameManager gameManager;
    ScoreCount scoreCounter;
    Vector3 startPos = new Vector3(-8, 1, 0);

    // Ground check options
    private float raycastDistance = 0.1f;
    public LayerMask groundLayer;
    private int numberOfRaycasts = 3;
    private Vector2 groundCheckBoxSize = new Vector2(1f, 0.5f);
    public LayerMask whatIsGround;
    public Transform groundCheck;
    public SpriteRenderer spriteRend;


    AudioManager audioManager;
    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    private void Start()
    {
        //originalSideJump = sideJump;

        body = GetComponent<Rigidbody2D>();
        bounceMain = GetComponent<EdgeCollider2D>();
        box = GetComponent<BoxCollider2D>();
        
        ResetValues();
        
        groundCheckBoxSize.x = box.bounds.size.x;
        groundCheckBoxSize.y = 0.5f;

        spriteRend = GetComponent<SpriteRenderer>();

        // Initialize jump strength indicator
        if (jumpIndicator != null)
        {
            jumpIndicator.enabled = true;
            jumpIndicator.fillAmount = 0f;
        }
        scoreCounter = gameManagerObj.GetComponent<ScoreCount>();
        gameManager = gameManagerObj.GetComponent<GameManager>();
    }

    public void ResetValues()
    {
        sideJump = 3;
        //bounceMaterial.bounciness = 1;
    }

    private void Update()
    {
        bool touchingWall = !CastRaycasts();

        allowJump = Physics2D.OverlapBox(groundCheck.position, groundCheckBoxSize, 0f, whatIsGround);

        if (sideJump > 2)
        {
            sideJump -= 0.02f * Time.deltaTime;
        }

        if (isJumpBoosted)
        {
            jumpBoostTimer -= Time.deltaTime;
            if (jumpBoostTimer <= 0)
            {
                // Reset jump force to original value
                sideJump -= 2f;
                isJumpBoosted = false;
            }
        }

        //bounceMaterial.bounciness += 0.2f * (Time.deltaTime*0.3f);

        // start of touch input to reset touch duration measurement
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began && allowJump && !isJumping && !touchingWall)
        {
            heldTime = 0f;
        }


        // times and updates held touch input, initializes jump strength indicator
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Stationary && allowJump && !isJumping && !touchingWall)
        {
            heldTime += Time.deltaTime;
            heldTime = Mathf.Clamp(heldTime, 0f, maxTime);

            UpdateJumpIndicator();
        }

        // jumping is perfomed if conditions are met and indicator is reset
        // inverts certain checks to perform input control
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended && allowJump && !isJumping && !touchingWall)
        {
            Jump();
            isJumping = true;
            if (jumpIndicator != null)
                jumpIndicator.fillAmount = 0f;
        }

        if (isJumping && allowJump && Input.touchCount == 0 && !touchingWall)
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
            body.transform.position = startPos + transform.up * 2f;
            gameManager.SpawnStarterPlatform();
            body.velocity = Vector3.zero;
        }
    }

    // Applies jumping vector to implement jumping and disables multi jumping mid-air 
    private void Jump()
    {
        float jumpHeight = Mathf.Lerp(minJump, jumpForce, Mathf.Pow(heldTime / maxTime, (float)1.5));
        body.velocity = new Vector2(sideJump, jumpHeight);

        audioManager.PlaySFX(audioManager.jump);
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

    // Additional ground check validation using raycasts
    private bool CastRaycasts()
    {
        Vector2 bottomCenter = new Vector2(box.bounds.center.x, box.bounds.min.y); // Bottom center of the box collider
        float raycastSpacing = box.bounds.size.x / (numberOfRaycasts - 1); // Spacing between raycasts
        bool hitPlatform = false; // Check for raycast colliding with platform

        for (int i = 0; i < numberOfRaycasts; i++)
        {
            Vector2 raycastOrigin = bottomCenter + Vector2.right * (raycastSpacing * i - box.bounds.extents.x); // Position for a raycast
            RaycastHit2D hit = Physics2D.Raycast(raycastOrigin, Vector2.down, raycastDistance, groundLayer); // Collider for created raycast

            if (hit.collider != null)
            {
                hitPlatform = true; // Make raycast collider valid
                Debug.DrawRay(raycastOrigin, Vector2.down * raycastDistance, Color.green); // Visualize valid raycast
            }
            else
                Debug.DrawRay(raycastOrigin, Vector2.down * raycastDistance, Color.red); // Visualize invalid raycast
        }
        return hitPlatform;
    }

    // Jumping booster effect
    public void IncreaseJump(float duration)
    {
        // Increase jump force for a duration
        sideJump += 2; // Increase jump force
        isJumpBoosted = true;
        jumpBoostTimer = duration;
    }
}