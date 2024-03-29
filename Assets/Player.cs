using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public float jumpForce = 10f;
    public float sideJump = 4f;
    public float maxTime = 1f;
    public float minJump = 2f;
    private int lives = 2;

    private float heldTime = 0f;
    private Rigidbody2D body;
    public PhysicsMaterial2D bounceMaterial;
    public PhysicsMaterial2D frictionMaterial;
    private EdgeCollider2D bounceMain;
    private BoxCollider2D box;

    private bool isJumping = false;
    private bool allowJump = true;
    private bool respawned = false;
    public Transform groundCheck;

    private Vector2 groundCheckBoxSize = new Vector2(1f, 0.5f);
    public LayerMask whatIsGround;
    public SpriteRenderer spriteRend;
    private bool flagInvisible = false;
    public GameObject gameManagerObj;
    GameManager gameManager;
    ScoreCount scoreCounter;
    Vector3 startPos = new Vector3(-10, 2, 0);

    public Image jumpIndicator;

    private void Start()
    {
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
        if (sideJump > 2)
        {
            sideJump -= 0.02f * Time.deltaTime;
        }

        //bounceMaterial.bounciness += 0.2f * (Time.deltaTime*0.3f);

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

        allowJump = Physics2D.OverlapBox(groundCheck.position, groundCheckBoxSize, 0f, whatIsGround);

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