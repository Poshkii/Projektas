using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    // Control options
    public float jumpForce = 10f;
    public float sideJump = 4f;
    public float maxTime = 1f;
    public float minJump = 2f;
    public int lives = 1;
    private float heldTime = 0f;
    public int jumpCount = 1;
    private int jumpsAvailable = 1;

    // Object options
    private Rigidbody2D body;
    public PhysicsMaterial2D bounceMaterial;
    public PhysicsMaterial2D frictionMaterial;
    private BoxCollider2D box;
    public Image jumpIndicator;

    // Jumping options
    private bool allowJump = true;
    private bool allowChecks = true;

    // Jump boost options
    private bool isJumpBoosted = false;
    private float jumpBoostTimer = 0f;

    // Game scene options
    private bool respawned = false;
    private bool flagInvisible = false;
    public GameObject gameManagerObj;
    GameManager gameManager;
    ScoreCount scoreCounter;
    Vector3 startPos = new Vector3(-10, 2, 0);

    // Ground check options
    private float raycastDistance = 0.05f;
    public LayerMask groundLayer;
    private int numberOfRaycasts = 3;
    private Vector2 groundCheckBoxSize = new Vector2(1f, 0.5f);
    public LayerMask whatIsGround;
    public Transform groundCheck;

    // Character sprite options
    public SpriteRenderer spriteRend;
    public Sprite[] jumpSprites;
    public Sprite jumpingSprite;
    public Sprite defaultSprite;

    AudioManager audioManager;
    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    private void Start()
    {
        body = GetComponent<Rigidbody2D>();
        box = GetComponent<BoxCollider2D>();

        ResetValues();

        groundCheckBoxSize.x = box.bounds.size.x - 0.1f;
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
        // Checks if ground check raycasts are valid
        bool raycastCheck = true;
        if (allowChecks)
        {
            raycastCheck = !CastRaycasts();
            allowJump = Physics2D.OverlapBox(groundCheck.position, groundCheckBoxSize, 0f, whatIsGround);
        }        
        if (!raycastCheck && allowJump)
        {
            jumpsAvailable = jumpCount;
        }

        if (raycastCheck)
        // Player is in the air, show jump in progress sprite
        {
            spriteRend.sprite = jumpingSprite;
        }
        else if (!raycastCheck && Input.touchCount == 0)
        {
            spriteRend.sprite = defaultSprite;
        }
        else
        // Player is not jumping, update based on charge level
        {
            UpdatePlayerSprite();
        }

        if (sideJump > 2)
        {
            sideJump -= 0.02f * Time.deltaTime;
        }

        // Returns jump strength back to normal after jump boost time expires
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
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began && jumpsAvailable > 0)
        {
            heldTime = 0f;
        }


        // times and updates held touch input, initializes jump strength indicator
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Stationary && jumpsAvailable > 0)
        {
            heldTime += Time.deltaTime;
            heldTime = Mathf.Clamp(heldTime, 0f, maxTime);

            UpdateJumpIndicator();
        }

        // jumping is perfomed if conditions are met and indicator is reset
        // inverts certain checks to perform input control
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended && jumpsAvailable > 0)
        {
            Jump();
            if (jumpIndicator != null)
                jumpIndicator.fillAmount = 0f;
        }

        if (Input.touchCount == 0 && jumpsAvailable > 0)
        {
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
            if (isJumpBoosted)
            {
                // Reset jump force to original value
                sideJump -= 2f;
                isJumpBoosted = false;
            }
        }
    }

    // Applies jumping vector to implement jumping and disables multi jumping mid-air 
    private void Jump()
    {
        if (jumpsAvailable > 0)
        {
            float jumpHeight = Mathf.Lerp(minJump, jumpForce, Mathf.Pow(heldTime / maxTime, (float)1.5));
            heldTime = 0;
            body.velocity = new Vector2(sideJump, jumpHeight);

            audioManager.PlaySFX(audioManager.jump);
            jumpsAvailable--;
            allowChecks = false;
            StartCoroutine(DelayChecks());
        }        
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
            {
                Debug.DrawRay(raycastOrigin, Vector2.down * raycastDistance, Color.red); // Visualize invalid raycast
            }
        }
        return hitPlatform;
    }

    // Jumping booster effect
    public void IncreaseJump(float duration)
    {
        // Increase side jump force for a duration
        sideJump += 2;
        isJumpBoosted = true;
        jumpBoostTimer = duration;
    }

    private void UpdatePlayerSprite()
    {
        // Calculate the charge percentage of the jump
        float chargePercentage = heldTime / maxTime;
        // Calculate the charge level based on 25% intervals
        int chargeLevel = Mathf.FloorToInt(chargePercentage / 0.25f);

        // Ensure charge level is within bounds
        chargeLevel = Mathf.Clamp(chargeLevel, 0, jumpSprites.Length - 1);

        // Set the sprite based on charge level
        if (chargeLevel < jumpSprites.Length)
        {
            spriteRend.sprite = jumpSprites[chargeLevel];
        }
    }

    private void ResizeCollider()
    {
        if (spriteRend == null || spriteRend.sprite == null || box == null)
        {
            return;
        }

        // Get the size of the sprite
        Vector2 spriteSize = spriteRend.sprite.bounds.size;

        // Set the size of the collider to match the sprite's size
        box.size = spriteSize;
    }

    IEnumerator DelayChecks()
    {
        yield return new WaitForSeconds(0.1f);
        allowChecks = true;
    }
}