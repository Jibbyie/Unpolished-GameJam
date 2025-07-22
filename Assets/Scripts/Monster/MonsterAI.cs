using Live2D.Cubism.Core;
using UnityEngine;

public class MonsterAI : MonoBehaviour
{
    [Header("References")]
    public PlayerController player;

    [Header("Patrol Settings")]
    public float speed = 2f;
    public float patrolDuration = 20f;
    public float timeToCatch = 5f;

    [Header("Spawning Settings")]
    public float minSpawnDistance = 4f;
    public float maxSpawnDistance = 8f;

    [Header("Behavior Settings")]
    public float headTurnMagnitude = 30f;
    public float flipDeadZone = 1.5f;

    private float patrolTimer;
    private float spottedTimer;
    private CubismModel cubismModel;
    private CubismParameter headParameter;

    private void Awake()
    {
        cubismModel = GetComponentInChildren<CubismModel>();
    }

    private void OnEnable()
    {
        patrolTimer = patrolDuration;
        spottedTimer = timeToCatch;

        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        float randomDistance = Random.Range(minSpawnDistance, maxSpawnDistance);
        Vector2 spawnPosition = (Vector2)player.transform.position + (randomDirection * randomDistance);

        transform.position = spawnPosition;

        if (cubismModel != null)
        {
            headParameter = cubismModel.Parameters.FindById("Param");
        }
    }

    private void Update()
    {
        patrolTimer -= Time.deltaTime;
        if (patrolTimer <= 0f)
        {
            gameObject.SetActive(false);
            return;
        }

        FlipAndLook();

        bool isPlayerVisible = !player.isHiding;

        if (isPlayerVisible)
        {
            transform.position = Vector2.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);

            spottedTimer -= Time.deltaTime;
            if (spottedTimer <= 0f)
            {
                Debug.Log("GAME OVER - Player is caught!");
            }
        }
        else
        {
            spottedTimer = timeToCatch;
        }
    }

    private void FlipAndLook()
    {
        if (player == null || headParameter == null) return;

        float directionToPlayerX = player.transform.position.x - transform.position.x;

        if (directionToPlayerX > flipDeadZone && transform.localScale.x > 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (directionToPlayerX < -flipDeadZone && transform.localScale.x < 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }

        Vector2 forwardVector = (transform.localScale.x > 0) ? Vector2.left : Vector2.right;
        Vector2 directionToPlayer = player.transform.position - transform.position;
        float angle = Vector2.SignedAngle(forwardVector, directionToPlayer);

        // Invert the final angle to match the model's parameter setup
        headParameter.Value = Mathf.Clamp(-angle, -headTurnMagnitude, headTurnMagnitude);
    }
}