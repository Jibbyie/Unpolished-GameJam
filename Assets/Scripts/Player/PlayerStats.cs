using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    private GameManager gameManager;
    private PlayerController playerController;

    [Header("Oxygen Values")]
    public float currentOxygen;
    public float maxOxygen = 100f;
    public float oxygenDepletionRate;

    private int currentOxygenBracket;

    // Awake is called once when the script instance is being loaded, before the game starts.
    private void Awake()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        playerController = FindFirstObjectByType<PlayerController>();
    }

    private void Start()
    {
        // Set the player's oxygen to full at the beginning of the game.
        currentOxygen = maxOxygen;

        // Calculate which UI bracket the oxygen level starts in.
        currentOxygenBracket = GetOxygenBracket();

        // Tell the GameManager to display the correct starting UI image.
        gameManager.OnOxygenStateChanged(currentOxygenBracket, currentOxygenBracket);
    }

    private void Update()
    {
        // If the player is not in an oxygen zone, deplete their oxygen over time.
        if (!playerController.isReplenishingOxygen)
        {
            currentOxygen -= oxygenDepletionRate * Time.deltaTime;
        }

        // If oxygen runs out, clamp it at zero and trigger a game over.
        if (currentOxygen <= 0)
        {
            currentOxygen = 0;
            gameManager.TriggerGameOver();
        }

        // Check every frame if the oxygen level has crossed into a new bracket.
        CheckForStateChange();
    }

    // Public function called by OxygenZone to add oxygen gradually over time.
    public void ReplenishOxygen(float amount)
    {
        // Increase the current oxygen by the amount per second.
        currentOxygen += amount * Time.deltaTime;
        // Ensure oxygen doesn't go above the maximum.
        if (currentOxygen >= maxOxygen)
        {
            currentOxygen = maxOxygen;
        }
        // Check if this change caused the UI to enter a new state.
        CheckForStateChange();
    }

    // Public function called by OxygenBubble to add a flat amount of oxygen instantly.
    public void AddOxygenBurst(float amount)
    {
        // Increase the current oxygen by a flat value.
        currentOxygen += amount;
        // Ensure oxygen doesn't go above the maximum.
        if (currentOxygen >= maxOxygen)
        {
            currentOxygen = maxOxygen;
        }
        // Check if this change caused the UI to enter a new state.
        CheckForStateChange();
    }

    // Compares the current oxygen bracket with the last known one to detect a change.
    private void CheckForStateChange()
    {
        // Determine the current oxygen bracket.
        int newBracket = GetOxygenBracket();

        // If the bracket has changed since the last frame...
        if (newBracket != currentOxygenBracket)
        {
            // ...tell the GameManager about the change to update the UI and play sounds.
            gameManager.OnOxygenStateChanged(newBracket, currentOxygenBracket);
            // ...and update the stored bracket to the new one.
            currentOxygenBracket = newBracket;
        }
    }

    // Converts the current oxygen float value into an integer state (0-5).
    private int GetOxygenBracket()
    {
        if (currentOxygen >= maxOxygen * 0.8f) return 5; // Return state 5 if oxygen is 80% or more.
        if (currentOxygen >= maxOxygen * 0.6f) return 4; // Return state 4 if oxygen is 60% or more.
        if (currentOxygen >= maxOxygen * 0.4f) return 3; // Return state 3 if oxygen is 40% or more.
        if (currentOxygen >= maxOxygen * 0.2f) return 2; // Return state 2 if oxygen is 20% or more.
        if (currentOxygen > 0) return 1;                  // Return state 1 if oxygen is above 0.
        return 0;                                         // Return state 0 if oxygen is 0 or less.
    }
}