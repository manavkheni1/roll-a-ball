using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement; // Required for reloading the scene

public class PlayerController : MonoBehaviour
{
    // --- Configuration Variables ---
    [Header("Movement Settings")]
    public float speed = 10.0f;          
    public float jumpForce = 5.0f;       
    public float dashForce = 50.0f;      
    public float dashCooldown = 1.0f;    

    [Header("Game Flow")]
    public float restartDelay = 3.0f;    // --- NEW: Time to wait before restarting ---

    [Header("Ground Detection")]
    public LayerMask groundLayer;        
    public float groundCheckDistance = 0.6f; 

    [Header("UI")]
    public TextMeshProUGUI countText;    
    public TextMeshProUGUI winText;      
    public TextMeshProUGUI loseText;     
    public TextMeshProUGUI timerText;    
    public TextMeshProUGUI bestTimeText; 

    [Header("Effects")]
    public GameObject pickupEffectPrefab; 

    // --- State Variables ---
    public bool isGameOver = false;      

    private Rigidbody rb;                
    private int count;                   
    private bool canDash = true;         
    private float currentTime = 0.0f;    
    private float bestTime = 0.0f;       

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        // Initialize variables
        count = 0;
        isGameOver = false;
        currentTime = 0.0f;

        // Load the best time (default to Infinity if no save exists)
        bestTime = PlayerPrefs.GetFloat("BestTime", Mathf.Infinity);
        
        // Setup initial UI states
        UpdateBestTimeUI();
        SetCountText();
        
        if (winText) winText.text = ""; 
        if (loseText) loseText.gameObject.SetActive(false); 
    }

    void Update()
    {
        if (isGameOver) return; 

        // Update timer every frame
        currentTime += Time.deltaTime; 
        if (timerText) timerText.text = "Time: " + currentTime.ToString("F2");

        // Handle Jump Input
        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            if(AudioManager.instance) AudioManager.instance.PlaySound(AudioManager.instance.jumpSound);
        }

        // Handle Dash Input (Left Shift or E)
        // Note: Changed RightShift to LeftShift as per standard controls, but kept E
        if (Input.GetKeyDown(KeyCode.RightShift) || Input.GetKeyDown(KeyCode.E))
        {
            if (canDash)
            {
                float h = Input.GetAxisRaw("Horizontal");
                float v = Input.GetAxisRaw("Vertical");
                
                // Only dash if moving
                if (Mathf.Abs(h) > 0.1f || Mathf.Abs(v) > 0.1f)
                {
                    StartCoroutine(DashMechanic(h, v));
                    if(AudioManager.instance) AudioManager.instance.PlaySound(AudioManager.instance.dashSound);
                }
            }
        }
    }

    void FixedUpdate()
    {
        if (isGameOver) return;

        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
        rb.AddForce(movement * speed);
    }

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer);
    }

    IEnumerator DashMechanic(float h, float v)
    {
        canDash = false; 
        
        Vector3 dashDir = new Vector3(h, 0, v).normalized;
        rb.AddForce(dashDir * dashForce, ForceMode.VelocityChange);
        
        yield return new WaitForSeconds(dashCooldown);
        
        canDash = true; 
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Pick Up"))
        {
            // Spawn Particles
            if (pickupEffectPrefab != null)
            {
                Instantiate(pickupEffectPrefab, other.transform.position, Quaternion.identity);
            }

            // Play Sound
            if(AudioManager.instance) AudioManager.instance.PlaySound(AudioManager.instance.collectSound);

            other.gameObject.SetActive(false);
            
            count = count + 1;
            SetCountText();
        }
    }

    public void GameOver(bool didWin)
    {
        isGameOver = true;
        
        // Stop the player
        rb.linearVelocity = Vector3.zero; 
        rb.angularVelocity = Vector3.zero;

        if (didWin)
        {
            winText.text = "You Win!";
            if(AudioManager.instance) AudioManager.instance.PlaySound(AudioManager.instance.winSound);

            // High Score Logic
            if (currentTime < bestTime)
            {
                bestTime = currentTime;
                PlayerPrefs.SetFloat("BestTime", bestTime);
                PlayerPrefs.Save(); 
                
                winText.text = "New Best Time!\n" + bestTime.ToString("F2") + "s";
                UpdateBestTimeUI();
            }
            // Optional: Uncomment below if you want the game to restart after winning too
            // StartCoroutine(RestartGame());
        }
        else
        {
            // --- LOSE LOGIC ---
            if (loseText)
            {
                loseText.gameObject.SetActive(true);
                loseText.text = "You Lose!";
            }
            
            if(AudioManager.instance) AudioManager.instance.PlaySound(AudioManager.instance.loseSound);

            // --- RESTART TIMER ---
            StartCoroutine(RestartGame());
        }
    }

    // --- NEW FUNCTION: Reloads the level after a delay ---
    IEnumerator RestartGame()
    {
        yield return new WaitForSeconds(restartDelay);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void UpdateBestTimeUI()
    {
        if (bestTimeText)
        {
            if (bestTime == Mathf.Infinity) bestTimeText.text = "Best: --";
            else bestTimeText.text = "Best: " + bestTime.ToString("F2");
        }
    }

    void SetCountText()
    {
        countText.text = "Count: " + count.ToString();
        if (count >= 7) GameOver(true);
    }
}
