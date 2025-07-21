using UnityEngine;

public class MonsterAI : MonoBehaviour
{
    // --- References & Settings ---
    public PlayerController player; 
    public float speed = 2f;
    public float patrolDuration = 20f; // How long it stays active
    public float timeToCatch = 5f;     // How long player can be seen before caught

    [Header("Spawning Settings")]
    public float minSpawnDistance = 4f; 
    public float maxSpawnDistance = 8f; 

    private float patrolTimer;
    private float spottedTimer;

    private void OnEnable()
    {
        // 1. Reset timers
        patrolTimer = patrolDuration;
        spottedTimer = timeToCatch;

        // 2. Find a random spawn position near the player
        Vector2 randomDirection = Random.insideUnitCircle.normalized; // Get a random direction
        float randomDistance = Random.Range(minSpawnDistance, maxSpawnDistance); // Get a random distance
        Vector2 spawnPosition = (Vector2)player.transform.position + (randomDirection * randomDistance);

        // 3. Set the monster's position
        transform.position = spawnPosition;
    }

    private void Update()
    {
        // --- Patrol Lifespan Timer ---
        patrolTimer -= Time.deltaTime;
        if (patrolTimer <= 0f)
        {
            gameObject.SetActive(false); // Despawn when time is up
            return; // Stop executing the rest of the code in this frame
        }

        // --- Movement and Detection ---
        bool isPlayerVisible = !player.isHiding;

        if (isPlayerVisible)
        {
            // If player is NOT hiding:
            // 1. Move towards the player
            transform.position = Vector2.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);

            // 2. Count down the "spotted" timer
            spottedTimer -= Time.deltaTime;
            if (spottedTimer <= 0f)
            {
                // Player is caught!
                Debug.Log("GAME OVER - Player is caught!");
            }
        }
        else
        {
            // If player IS hiding, reset the spotted timer.
            spottedTimer = timeToCatch;
        }
    }
}