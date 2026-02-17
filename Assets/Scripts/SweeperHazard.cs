using UnityEngine;

public class SweeperHazard : MonoBehaviour
{
    [Header("Hazard Settings")]
    // The speed at which the hazard rotates in degrees per second
    public float rotationSpeed = 50f; // How fast it spins
    // The strength of the impact when the player touches this hazard
    public float knockbackForce = 15f; // How hard it hits you

    private Rigidbody rb;

    void Start()
    {
        // Cache the Rigidbody component for use in physics updates
        rb = GetComponent<Rigidbody>();
    }

    // FixedUpdate is called at fixed time intervals, essential for consistent physics behavior
    void FixedUpdate()
    {
        // 1. Calculate rotation
        // We use FixedUpdate and MoveRotation because this is a Physics interaction.
        // If we just used Transform.Rotate, the player might clip through it or get stuck inside.
        float angle = rotationSpeed * Time.fixedDeltaTime;
        
        // Create a rotation quaternion around the Y-axis (Up axis)
        Quaternion deltaRotation = Quaternion.Euler(new Vector3(0, angle, 0));

        // 2. Apply rotation to the Rigidbody
        // MoveRotation handles the physics engine better than directly setting transform.rotation
        rb.MoveRotation(rb.rotation * deltaRotation);
    }

    // 3. Handle Collision (Knockback)
    // This event fires automatically when another collider touches this object
    void OnCollisionEnter(Collision collision)
    {
        // Check if the object we hit is the Player
        if (collision.gameObject.CompareTag("Player"))
        {
            // Get the Player's Rigidbody so we can apply force to it
            Rigidbody playerRb = collision.gameObject.GetComponent<Rigidbody>();

            if (playerRb != null)
            {
                // Calculate direction: From Center of Sweeper -> To Player
                // This ensures the player is always pushed *away* from the hazard
                Vector3 pushDirection = (collision.transform.position - transform.position).normalized;
                
                // Add an "Impulse" force to fling the player away immediately
                // ForceMode.Impulse is ideal for instant impacts (like being hit by a bat)
                playerRb.AddForce(pushDirection * knockbackForce, ForceMode.Impulse);

                Debug.Log("Player hit by Sweeper!");
            }
        }
    }
}