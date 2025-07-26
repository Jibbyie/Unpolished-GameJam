using TMPro;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("References")]
    public GameObject jormungandrObject;
    public PlayerStats playerStats;
    public PlayerController playerController;
    public PlayerMovement playerMovement;
    [SerializeField] private GameObject weddingRingObject;

    [Header("UI Elements")]
    [SerializeField] private TMP_Text itemCountText;
    [SerializeField] private TMP_Text memoryText;
    [SerializeField] private TMP_Text finalMemoryTextObject;
    [SerializeField] private Image hidingTintImage;
    [SerializeField] private Image screenFaderImage;

    [Header("Oxygen UI")]
    [SerializeField] private Image oxygenImage;
    [SerializeField] private Sprite[] oxygenSprites;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioClip somberAmbience;
    [SerializeField] private AudioClip memorySFX;
    [SerializeField] private AudioClip oxygenDecreaseSound;
    [SerializeField] private AudioClip oxygenIncreaseSound;
    [SerializeField] private AudioClip oxygenFullSound;
    [SerializeField] private AudioClip oxygenEmptySound;

    [Header("Timings")]
    [SerializeField] private float textFadeTime = 1f;
    [SerializeField] private float finalTextFadeTime = 4f;
    [SerializeField] private float textDisplayTime = 2.5f;
    [SerializeField] private float tintFadeDuration = 0.5f;

    private int itemsCollected = 0;
    private Coroutine runningFadeCoroutine;
    private bool isGameOver = false;

    public void OnOxygenStateChanged(int newBracket, int oldBracket)
    {
        if (isGameOver) return;
        if (oxygenImage != null && newBracket >= 0 && newBracket < oxygenSprites.Length)
        {
            oxygenImage.sprite = oxygenSprites[newBracket];
        }

        if (audioSource == null) return;

        if (newBracket == 0) audioSource.PlayOneShot(oxygenEmptySound);
        else if (newBracket == 5) audioSource.PlayOneShot(oxygenFullSound);
        else if (newBracket < oldBracket) audioSource.PlayOneShot(oxygenDecreaseSound);
        else if (newBracket > oldBracket) audioSource.PlayOneShot(oxygenIncreaseSound);
    }

    public void SetHidingVignette(bool isHiding)
    {
        if (hidingTintImage == null) return;
        if (runningFadeCoroutine != null) StopCoroutine(runningFadeCoroutine);

        float targetAlpha = isHiding ? 0.75f : 0f;
        runningFadeCoroutine = StartCoroutine(FadeImage(hidingTintImage, targetAlpha, tintFadeDuration));
    }

    public void TriggerGameOver()
    {
        if (isGameOver) return;
        isGameOver = true;

        DisablePlayer();
        StartCoroutine(GameOverSequence());
    }

    private void DisablePlayer()
    {
        if (playerMovement != null) playerMovement.enabled = false;
        if (playerStats != null) playerStats.enabled = false;
        if (playerController != null) playerController.canMove = false;
    }

    public void OnItemCollected(int itemID, string newMemoryText, bool isFinal)
    {
        if (isGameOver) return;
        if (isFinal)
        {
            StartCoroutine(FinalSequence());
            return;
        }

        itemsCollected++;
        itemCountText.text = "Items Collected: " + itemsCollected.ToString();
        audioSource.PlayOneShot(memorySFX);
        memoryText.text = newMemoryText;
        StartCoroutine(ShowMemoryRoutine());

        if (itemsCollected == 2 || itemsCollected == 3 || itemsCollected == 4)
        {
            StartJormungandrEvent();
        }

        if (itemsCollected >= 4)
        {
            if (weddingRingObject != null) weddingRingObject.SetActive(true);
        }
    }

    private void StartJormungandrEvent()
    {
        if (jormungandrObject != null && !jormungandrObject.activeInHierarchy)
        {
            jormungandrObject.SetActive(true);
        }
    }

    private IEnumerator GameOverSequence()
    {
        yield return StartCoroutine(FadeImage(screenFaderImage, 1f, 3f));
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("MainMenu");
    }

    private IEnumerator FinalSequence()
    {
        isGameOver = true;
        DisablePlayer();

        yield return StartCoroutine(FadeImage(screenFaderImage, 1f, 3f));

        AudioSource[] allAudioSources = FindObjectsOfType<AudioSource>();
        foreach (AudioSource source in allAudioSources)
        {
            source.Stop();
        }

        if (musicSource != null && somberAmbience != null)
        {
            musicSource.clip = somberAmbience;
            musicSource.loop = true;
            musicSource.Play();
        }

        if (finalMemoryTextObject != null)
        {
            finalMemoryTextObject.gameObject.SetActive(true);
            finalMemoryTextObject.color = new Color(finalMemoryTextObject.color.r, finalMemoryTextObject.color.g, finalMemoryTextObject.color.b, 0);
            yield return StartCoroutine(FadeText(finalMemoryTextObject, 1f, finalTextFadeTime));
        }

        yield return new WaitForSeconds(15f);

        if (finalMemoryTextObject != null)
        {
            yield return StartCoroutine(FadeText(finalMemoryTextObject, 0f, textFadeTime));
        }

        // Wait for 2 seconds on the black screen
        yield return new WaitForSeconds(5f);

        SceneManager.LoadScene("MainMenu");
    }

    private IEnumerator ShowMemoryRoutine()
    {
        yield return StartCoroutine(FadeText(memoryText, 1f, textFadeTime));
        yield return new WaitForSeconds(textDisplayTime);
        yield return StartCoroutine(FadeText(memoryText, 0f, textFadeTime));
    }

    private IEnumerator FadeImage(Image image, float targetAlpha, float duration)
    {
        if (image == null) yield break;

        float startAlpha = image.color.a;
        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, time / duration);
            image.color = new Color(image.color.r, image.color.g, image.color.b, newAlpha);
            yield return null;
        }
        image.color = new Color(image.color.r, image.color.g, image.color.b, targetAlpha);
    }

    private IEnumerator FadeText(TMP_Text textElement, float targetAlpha, float duration)
    {
        if (textElement == null) yield break;

        float startAlpha = textElement.color.a;
        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, time / duration);
            textElement.color = new Color(textElement.color.r, textElement.color.g, textElement.color.b, newAlpha);
            yield return null;
        }
        textElement.color = new Color(textElement.color.r, textElement.color.g, textElement.color.b, targetAlpha);
    }
}