using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class PlayerController : MonoBehaviour
{
    // --- Configuration Variables (Accessible in Inspector) ---
    [Header("Movement Settings")]
    public float speed = 10.0f;          // Movement speed
    public float jumpForce = 5.0f;       // Vertical force for jumping
    public float dashForce = 50.0f;      // Burst of speed for dashing
    public float dashCooldown = 1.0f;    // Time to wait before dashing again

    [Header("Ground Detection")]
    public LayerMask groundLayer;        // Layers considered "ground" (e.g., Default, Ground)
    public float groundCheckDistance = 0.6f; // How far down to check for ground

    [Header("UI")]
    public TextMeshProUGUI countText;    // Displays current score
    public TextMeshProUGUI winText;      // Displays "You Win" message
    public TextMeshProUGUI loseText;     // Displays "You Lose" message
    public TextMeshProUGUI timerText;    // Displays current run time
    public TextMeshProUGUI bestTimeText; // Displays high score

    [Header("Effects")]
    public GameObject pickupEffectPrefab; // Particle effect spawned when collecting items

    // --- State Variables ---
    public bool isGameOver = false;      // Flag to stop input/movement when game ends

    private Rigidbody rb;                // Reference to the Rigidbody component
    private int count;                   // Current score/pickup count
    private bool canDash = true;         // Flag to prevent spamming dash
    private float currentTime = 0.0f;    // Timer for the current run
    private float bestTime = 0.0f;       // Loaded high score

    void Start()
    {
        // Get the Rigidbody component attached to this GameObject
        rb = GetComponent<Rigidbody>();
        
        // Initialize variables
        count = 0;
        isGameOver = false;
        currentTime = 0.0f;

        // Load the best time from player preferences (default to Infinity if no save exists)
        bestTime = PlayerPrefs.GetFloat("BestTime", Mathf.Infinity);
        
        // Setup initial UI states
        UpdateBestTimeUI();
        SetCountText();
        
        if (winText) winText.text = ""; 
        if (loseText) loseText.gameObject.SetActive(false); 
    }

    void Update()
    {
        // specific inputs or timer logic shouldn't run if the game is over
        if (isGameOver) return; 

        // Update timer every frame
        currentTime += Time.deltaTime; 
        if (timerText) timerText.text = "Time: " + currentTime.ToString("F2");

        // Handle Jump Input
        // We use IsGrounded() to prevent infinite mid-air jumping
        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            if(AudioManager.instance) AudioManager.instance.PlaySound(AudioManager.instance.jumpSound);
        }

        // Handle Dash Input (Right Shift or E)
        if (Input.GetKeyDown(KeyCode.RightShift) || Input.GetKeyDown(KeyCode.E))
        {
            if (canDash)
            {
                // Check if player is trying to move before applying dash
                float h = Input.GetAxisRaw("Horizontal");
                float v = Input.GetAxisRaw("Vertical");
                
                // Only dash if there is some input magnitude
                if (Mathf.Abs(h) > 0.1f || Mathf.Abs(v) > 0.1f)
                {
                    StartCoroutine(DashMechanic(h, v));
                    if(AudioManager.instance) AudioManager.instance.PlaySound(AudioManager.instance.dashSound);
                }
            }
        }
    }

    // FixedUpdate is used for physics calculations to ensure consistent behavior across frame rates
    void FixedUpdate()
    {
        if (isGameOver) return;

        // Get movement input (-1 to 1)
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        // Create movement vector and apply force
        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
        rb.AddForce(movement * speed);
    }

    // Helper method to check if player is touching the ground using a Raycast
    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer);
    }

    // Coroutine to handle Dash physics and cooldown
    IEnumerator DashMechanic(float h, float v)
    {
        canDash = false; // Disable dashing
        
        // Calculate direction and apply immediate velocity change
        Vector3 dashDir = new Vector3(h, 0, v).normalized;
        rb.AddForce(dashDir * dashForce, ForceMode.VelocityChange);
        
        // Wait for cooldown duration
        yield return new WaitForSeconds(dashCooldown);
        
        canDash = true; // Re-enable dashing
    }

    // Triggered when entering a collider marked as "Is Trigger"
    void OnTriggerEnter(Collider other)
    {
        // Check if the object we hit is a pickup
        if (other.gameObject.CompareTag("Pick Up"))
        {
            // --- 1. SPAWN PARTICLES ---
            if (pickupEffectPrefab != null)
            {
                // Instantiate the particle effect at the pickup's position
                Instantiate(pickupEffectPrefab, other.transform.position, Quaternion.identity);
            }

            // --- 2. PLAY SOUND ---
            if(AudioManager.instance) AudioManager.instance.PlaySound(AudioManager.instance.collectSound);

            // Deactivate the pickup object (simulate collection)
            other.gameObject.SetActive(false);
            
            // Increment score and update UI
            count = count + 1;
            SetCountText();
        }
    }

    // Handles Game Over logic (Winning or Losing)
    public void GameOver(bool didWin)
    {
        isGameOver = true;
        
        // Stop the player's movement immediately
        rb.linearVelocity = Vector3.zero; 
        rb.angularVelocity = Vector3.zero;

        if (didWin)
        {
            winText.text = "You Win!";
            if(AudioManager.instance) AudioManager.instance.PlaySound(AudioManager.instance.winSound);

            // Check if current time is better (lower) than the saved best time
            if (currentTime < bestTime)
            {
                bestTime = currentTime;
                
                // Save new high score to disk
                PlayerPrefs.SetFloat("BestTime", bestTime);
                PlayerPrefs.Save(); 
                
                winText.text = "New Best Time!\n" + bestTime.ToString("F2") + "s";
                UpdateBestTimeUI();
            }
        }
        else
        {
            if (loseText)
            {
                loseText.gameObject.SetActive(true);
                loseText.text = "You Lose!";
            }
            if(AudioManager.instance) AudioManager.instance.PlaySound(AudioManager.instance.loseSound);
        }
    }

    // Update the UI text for the best time
    void UpdateBestTimeUI()
    {
        if (bestTimeText)
        {
            // If infinity, it means no game has been played/saved yet
            if (bestTime == Mathf.Infinity) bestTimeText.text = "Best: --";
            else bestTimeText.text = "Best: " + bestTime.ToString("F2");
        }
    }

    // Update the score UI and check for win condition
    void SetCountText()
    {
        countText.text = "Count: " + count.ToString();
        
        // Win condition: Collected 7 or more items
        if (count >= 7) GameOver(true);
    }
}