using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class EnemyAI : MonoBehaviour
{
    // Reference to the player's Transform to track their position
    public Transform player;
    // Reference to the player's script to check game state (Win/Lose)
    public PlayerController playerScript;
    // Reference to the NavMeshAgent component for pathfinding
    private NavMeshAgent agent;

    void Start()
    {
        // Get the NavMeshAgent attached to this GameObject
        agent = GetComponent<NavMeshAgent>();
        
        // Disable automatic rotation so we can spin the enemy manually in Update
        agent.updateRotation = false; // We handle rotation manually
    }

    void Update()
    {
        // Check if the game has ended (Win or Lose)
        // If so, stop the enemy from moving or doing anything else
        if (playerScript.isGameOver)
        {
            agent.isStopped = true; // Stop pathfinding
            agent.velocity = Vector3.zero; // Stop current movement immediately
            return; // Exit the loop
        }

        // If the player exists, tell the agent to move toward the player's current position
        if (player != null)
        {
            agent.SetDestination(player.position);
        }

        // Apply a constant spinning effect for visual flair
        // Creates a chaotic rotation on all three axes
        transform.Rotate(new Vector3(15, 30, 45) * Time.deltaTime);
    }

    // Triggered when the enemy physically touches another collider
    void OnCollisionEnter(Collision collision)
    {
        // Do nothing if the game is already over
        if (playerScript.isGameOver) return;

        // Check if the object we collided with is the Player
        if (collision.gameObject.CompareTag("Player"))
        {
            // Call the GameOver method on the player script
            // Passing 'false' indicates a Loss
            playerScript.GameOver(false);
        }
    }
}