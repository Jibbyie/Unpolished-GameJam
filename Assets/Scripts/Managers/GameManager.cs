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
    [SerializeField] private Image hidingTintImage;

    [Header("Oxygen UI")]
    [SerializeField] private Image oxygenImage; // The UI Image to display the oxygen state.
    [SerializeField] private Sprite[] oxygenSprites; // Sprites from 0%, 20%, 40%, 60%, 80%, 100%.

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource; // The AudioSource component for playing sounds.
    [SerializeField] private AudioClip oxygenDecreaseSound;
    [SerializeField] private AudioClip oxygenIncreaseSound;
    [SerializeField] private AudioClip oxygenFullSound;
    [SerializeField] private AudioClip oxygenEmptySound;

    [Header("Timings")]
    [SerializeField] private float textFadeTime = 1f;
    [SerializeField] private float textDisplayTime = 2.5f;
    [SerializeField] private float tintFadeDuration = 0.5f;

    private int itemsCollected = 0;
    private Coroutine runningFadeCoroutine;

    // Called from PlayerStats to respond to oxygen level changes.
    public void OnOxygenStateChanged(int newBracket, int oldBracket)
    {
        // Update the UI Image to the correct sprite.
        if (oxygenImage != null && newBracket >= 0 && newBracket < oxygenSprites.Length)
        {
            oxygenImage.sprite = oxygenSprites[newBracket];
        }

        // Play the appropriate sound effect.
        if (audioSource != null)
        {
            if (newBracket == 0)
            {
                audioSource.PlayOneShot(oxygenEmptySound);
            }
            else if (newBracket == 5)
            {
                audioSource.PlayOneShot(oxygenFullSound);
            }
            else if (newBracket < oldBracket)
            {
                audioSource.PlayOneShot(oxygenDecreaseSound);
            }
            else if (newBracket > oldBracket)
            {
                audioSource.PlayOneShot(oxygenIncreaseSound);
            }
        }
    }

    public void SetHidingVignette(bool isHiding)
    {
        if (hidingTintImage == null) return;
        if (runningFadeCoroutine != null)
        {
            StopCoroutine(runningFadeCoroutine);
        }
        float targetAlpha = isHiding ? 0.75f : 0f;
        runningFadeCoroutine = StartCoroutine(FadeTintRoutine(targetAlpha, tintFadeDuration));
    }

    public void TriggerGameOver()
    {
        Debug.Log("You Died");
    }

    public void OnItemCollected(int itemID, string newMemoryText)
    {
        itemsCollected++;
        itemCountText.text = "Items Collected: " + itemsCollected.ToString();
        memoryText.text = newMemoryText;
        StartCoroutine(ShowMemoryRoutine());

        if (itemsCollected == 2)
        {
            StartJormungandrEvent();
        }
    }

    private void StartJormungandrEvent()
    {
        if (jormungandrObject != null)
        {
            jormungandrObject.SetActive(true);
        }
    }

    private IEnumerator ShowMemoryRoutine()
    {
        yield return StartCoroutine(FadeTextToFullAlpha(textFadeTime, memoryText));
        yield return new WaitForSeconds(textDisplayTime);
        yield return StartCoroutine(FadeTextToZeroAlpha(textFadeTime, memoryText));
    }

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
        hidingTintImage.color = new Color(hidingTintImage.color.r, hidingTintImage.color.g, hidingTintImage.color.b, targetAlpha);
    }

    public IEnumerator FadeTextToFullAlpha(float t, TMP_Text i)
    {
        i.color = new Color(i.color.r, i.color.g, i.color.b, 0);
        while (i.color.a < 1.0f)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a + (Time.deltaTime / t));
            yield return null;
        }
    }

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