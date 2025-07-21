using UnityEngine;

public class OxygenBubble : MonoBehaviour
{
    public float oxygenReplenishRate;
    private PlayerController playerController;
    private PlayerStats playerStats;

    private void Awake()
    {
        playerStats = FindFirstObjectByType<PlayerStats>();
        playerController = FindFirstObjectByType<PlayerController>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerStats.AddOxygenBurst(oxygenReplenishRate);
            Destroy(this.gameObject);
        }
    }
}
