using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    private GameManager gameManager;
    private PlayerController playerController;

    [Header("Oxygen Values")]
    public float currentOxygen;
    public float maxOxygen = 100f;
    public float oxygenDepletionRate;

    // Private variable to track the current oxygen bracket.
    private int currentOxygenBracket;

    private void Awake()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        playerController = FindFirstObjectByType<PlayerController>();
    }

    private void Start()
    {
        // Set the initial oxygen level and calculate the starting bracket.
        currentOxygen = maxOxygen;
        currentOxygenBracket = GetOxygenBracket();
    }

    private void Update()
    {
        // Deplete oxygen if the player is not in a replenishment zone.
        if (!playerController.isReplenishingOxygen)
        {
            currentOxygen -= oxygenDepletionRate * Time.deltaTime;
        }

        // Trigger a game over if oxygen runs out.
        if (currentOxygen <= 0)
        {
            currentOxygen = 0;
            gameManager.TriggerGameOver();
        }

        // Check if the oxygen level has crossed a threshold.
        CheckForStateChange();
    }

    public void ReplenishOxygen(float amount)
    {
        // Add oxygen over time and clamp it to the maximum value.
        currentOxygen += amount * Time.deltaTime;
        if (currentOxygen >= maxOxygen)
        {
            currentOxygen = maxOxygen;
        }
        CheckForStateChange();
    }

    public void AddOxygenBurst(float amount)
    {
        // Add a flat amount of oxygen and clamp it to the maximum value.
        currentOxygen += amount;
        if (currentOxygen >= maxOxygen)
        {
            currentOxygen = maxOxygen;
        }
        CheckForStateChange();
    }

    // Checks if the oxygen has moved into a new 20% bracket.
    private void CheckForStateChange()
    {
        int newBracket = GetOxygenBracket();

        // If the bracket is different from the last known one, notify the GameManager.
        if (newBracket != currentOxygenBracket)
        {
            gameManager.OnOxygenStateChanged(newBracket, currentOxygenBracket);
            currentOxygenBracket = newBracket;
        }
    }

    // Converts the current oxygen float value into an integer state (0-5).
    private int GetOxygenBracket()
    {
        if (currentOxygen >= maxOxygen) return 5; // 90-100%
        if (currentOxygen >= maxOxygen * 0.8f) return 4; // 80%
        if (currentOxygen >= maxOxygen * 0.6f) return 3; // 60%
        if (currentOxygen >= maxOxygen * 0.4f) return 2; // 40%
        if (currentOxygen > 0) return 1; // 20%
        return 0; // 0%
    }
}