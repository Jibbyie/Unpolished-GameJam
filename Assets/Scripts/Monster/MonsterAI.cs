using Live2D.Cubism.Core;
using UnityEngine;

public class MonsterAI : MonoBehaviour
{
    [Header("References")]
    public PlayerController player; // A reference to the player's controller script.
    public Transform modelTransform; // The child object containing the Live2D model visuals.

    [Header("Patrol Settings")]
    public float speed = 2f; // How fast the monster moves towards the player.
    public float patrolDuration = 20f; // How long the monster stays active before disappearing.
    public float timeToCatch = 5f; // How long the player has to hide once spotted.

    [Header("Spawning Settings")]
    public float minSpawnDistance = 4f;
    public float maxSpawnDistance = 8f; 

    [Header("Behavior Settings")]
    public float swaySpeed = 1.5f; // How fast the head sways back and forth.
    public float swayMagnitude = 15f; // How far the head sways from the center.

    // Private variables for internal state management.
    private float patrolTimer;
    private float spottedTimer;
    private CubismModel cubismModel;
    private CubismParameter headParameter;

    private void Awake()
    {
        cubismModel = GetComponentInChildren<CubismModel>();
        if (cubismModel != null)
        {
            // Find the specific 'head' parameter by its ID to control it later.
            headParameter = cubismModel.Parameters.FindById("Param");
        }
    }

    private void OnEnable()
    {
        // Reset the monster's state each time it appears.
        patrolTimer = patrolDuration;
        spottedTimer = timeToCatch;

        // Calculate a random spawn position near the player.
        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        float randomDistance = Random.Range(minSpawnDistance, maxSpawnDistance);
        transform.position = (Vector2)player.transform.position + (randomDirection * randomDistance);
    }

    private void Update()
    {
        // Ensure the player exists before running any logic.
        if (player == null) return;

        // Tick down the monster's overall lifespan for this patrol.
        patrolTimer -= Time.deltaTime;
        if (patrolTimer <= 0f)
        {
            // Deactivate the monster if its patrol time is over.
            gameObject.SetActive(false);
            return;
        }

        // Rotate the visual model to always face the player.
        if (modelTransform != null)
        {
            Vector2 directionToPlayer = player.transform.position - transform.position;
            float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;
            modelTransform.rotation = Quaternion.Euler(0f, 0f, angle + 180f); // Add 180 degrees to offset the left-facing art.
        }

        // Animate the head parameter with a sine wave for a continuous sway.
        if (headParameter != null)
        {
            headParameter.Value = Mathf.Sin(Time.time * swaySpeed) * swayMagnitude;
        }

        // Check the player's hiding state to determine visibility.
        bool isPlayerVisible = !player.isHiding;
        if (isPlayerVisible)
        {
            // If the player is not hiding, move the monster towards them.
            transform.position = Vector2.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);

            // Decrease the catch timer while the player is visible.
            spottedTimer -= Time.deltaTime;
            if (spottedTimer <= 0f)
            {
                // Trigger a game over if the player is spotted for too long.
                Debug.Log("GAME OVER - Player is caught!");
            }
        }
        else
        {
            // If the player is hiding, reset the catch timer.
            spottedTimer = timeToCatch;
        }
    }
}