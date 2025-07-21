using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    private GameManager gameManager;
    private PlayerController playerController;

    [Header("Oyxgen Values")]
    public float currentOxygen;
    public float maxOxygen = 100f;
    public float oxygenDepletionRate;

    private void Awake()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        playerController = FindFirstObjectByType<PlayerController>();

        currentOxygen = 100f;
    }

    private void Update()
    {
        if(!playerController.isReplenishingOxygen)
            currentOxygen -= oxygenDepletionRate * Time.deltaTime;

        if(currentOxygen <= 0 )
        {
            gameManager.TriggerGameOver();
        }
    }

    public void ReplenishOxygen(float amount)
    {
        currentOxygen += amount * Time.deltaTime;

        if(currentOxygen >= maxOxygen )
        {
            currentOxygen = maxOxygen;
        }
    }

    public void AddOxygenBurst(float amount)
    {
        currentOxygen += amount;

        if (currentOxygen >= maxOxygen)
        {
            currentOxygen = maxOxygen;
        }
    }
}
