using UnityEngine;

public class OxygenZone : MonoBehaviour
{
    public float oxygenReplenishRate;
    private PlayerController playerController;
    private PlayerStats playerStats;

    private void Awake()
    {
        playerStats = FindFirstObjectByType<PlayerStats>();
        playerController = FindFirstObjectByType<PlayerController>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            playerStats.ReplenishOxygen(oxygenReplenishRate);
            playerController.isReplenishingOxygen = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerController.isReplenishingOxygen = false;
        }
    }
}
