using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Jumping : MonoBehaviour
{
    public float jumpForce = 10f;
    public float sideJump = 43;
    public float maxTime = 1f;
    public float minJump = 2f;

    private float heldTime = 0f;
    private Rigidbody2D body;
    public PhysicsMaterial2D bounceMaterial;
    public PhysicsMaterial2D frictionMaterial;
    private EdgeCollider2D bounceMain;
    private BoxCollider2D box;

    private bool isJumping = false;
    private bool allowJump = true;
    public Transform groundCheck;

    private Vector2 groundCheckBoxSize = new Vector2(1f, 0.5f);
    public LayerMask whatIsGround;
    public SpriteRenderer spriteRend;
    private bool flagInvisible = false;
    public GameObject gameManagerObj;
    ScoreCount scoreCounter;
    Vector3 startPos = new Vector3(-10, 2, 0);

    public Image jumpIndicator;

    private void Start()
    {
        body = GetComponent<Rigidbody2D>();
        bounceMain = GetComponent<EdgeCollider2D>();
        box = GetComponent<BoxCollider2D>();

        sideJump = 3;
        bounceMaterial.bounciness = 1;
        
        groundCheckBoxSize.x = box.bounds.size.x-0.1f;
        groundCheckBoxSize.y = 0.5f;

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
        if (sideJump > 2)
        {
            sideJump -= 0.02f * Time.deltaTime;
        }

        bounceMaterial.bounciness += 0.2f * (Time.deltaTime*0.3f);

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

        if (allowJump)
        {
            bounceMain.sharedMaterial = frictionMaterial;
        }
        else
        {
            bounceMain.sharedMaterial = bounceMaterial;
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

    internal void ResetSettings()
    {
        sideJump = 3;
        bounceMaterial.bounciness = 1;
    }
}