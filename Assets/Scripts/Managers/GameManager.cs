using TMPro;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

// Manages the overall game state, UI elements, and key events.
public class GameManager : MonoBehaviour
{
    [Header("References")]
    public GameObject jormungandrObject;
    public PlayerStats playerStats;

    [Header("UI Elements")]
    [SerializeField] private TMP_Text itemCountText;
    [SerializeField] private TMP_Text memoryText;
    [SerializeField] private TMP_Text oxygenText;
    [SerializeField] private Image hidingTintImage;

    [Header("Timings")]
    [SerializeField] private float textFadeTime = 1f;
    [SerializeField] private float textDisplayTime = 2.5f;
    [SerializeField] private float tintFadeDuration = 0.5f;

    // Private variables for internal state.
    private int itemsCollected = 0;
    private Coroutine runningFadeCoroutine;

    private void Awake()
    {
        playerStats = FindFirstObjectByType<PlayerStats>();
    }

    private void Update()
    {
        if (playerStats != null)
        {
            oxygenText.text = "Oxygen: " + playerStats.currentOxygen.ToString("F0");
        }
    }

    // Called from PlayerHiding to control the screen tint visibility.
    public void SetHidingVignette(bool isHiding)
    {
        if (hidingTintImage == null) return;

        // Stop any previous fade to avoid visual conflicts.
        if (runningFadeCoroutine != null)
        {
            StopCoroutine(runningFadeCoroutine);
        }

        // Start a new fade to the appropriate transparency.
        float targetAlpha = isHiding ? 0.75f : 0f;
        runningFadeCoroutine = StartCoroutine(FadeTintRoutine(targetAlpha, tintFadeDuration));
    }

    // Called from PlayerStats when oxygen reaches zero.
    public void TriggerGameOver()
    {
        Debug.Log("You Died");
    }

    // Called from PlayerCollector when an item is picked up.
    public void OnItemCollected(int itemID, string newMemoryText)
    {
        itemsCollected++;
        itemCountText.text = "Items Collected: " + itemsCollected.ToString();

        // Display the collected memory text on the screen.
        memoryText.text = newMemoryText;
        StartCoroutine(ShowMemoryRoutine());

        // Trigger the monster to spawn after collecting a specific number of items.
        if (itemsCollected == 2)
        {
            StartJormungandrEvent();
        }
    }

    // Activates the monster GameObject in the scene.
    private void StartJormungandrEvent()
    {
        if (jormungandrObject != null)
        {
            jormungandrObject.SetActive(true);
        }
    }

    // Manages the sequence for showing a memory on screen (fade in, wait, fade out).
    private IEnumerator ShowMemoryRoutine()
    {
        yield return StartCoroutine(FadeTextToFullAlpha(textFadeTime, memoryText));
        yield return new WaitForSeconds(textDisplayTime);
        yield return StartCoroutine(FadeTextToZeroAlpha(textFadeTime, memoryText));
    }

    // A coroutine to smoothly fade the hiding tint image.
    private IEnumerator FadeTintRoutine(float targetAlpha, float duration)
    {
        float startAlpha = hidingTintImage.color.a;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, time / duration);
            hidingTintImage.color = new Color(hidingTintImage.color.r, hidingTintImage.color.g, hidingTintImage.color.b, newAlpha);
            yield return null;
        }

        // Ensure the final alpha is set exactly to the target.
        hidingTintImage.color = new Color(hidingTintImage.color.r, hidingTintImage.color.g, hidingTintImage.color.b, targetAlpha);
    }

    // A coroutine that fades a text element to be fully visible.
    public IEnumerator FadeTextToFullAlpha(float t, TMP_Text i)
    {
        i.color = new Color(i.color.r, i.color.g, i.color.b, 0);
        while (i.color.a < 1.0f)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a + (Time.deltaTime / t));
            yield return null;
        }
    }

    // A coroutine that fades a text element to be fully transparent.
    public IEnumerator FadeTextToZeroAlpha(float t, TMP_Text i)
    {
        i.color = new Color(i.color.r, i.color.g, i.color.b, 1);
        while (i.color.a > 0.0f)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a - (Time.deltaTime / t));
            yield return null;
        }
    }
}