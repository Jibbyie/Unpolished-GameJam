using TMPro;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private int itemsCollected = 0;

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

    void StartJormungandrEvent()
    {
        if (jormungandrObject != null)
        {
            jormungandrObject.SetActive(true);
        }
    }

    private IEnumerator ShowMemoryRoutine()
    {
        yield return FadeTextToFullAlpha(textFadeTime, memoryText);
        yield return new WaitForSeconds(textDisplayTime);
        yield return FadeTextToZeroAlpha(textFadeTime, memoryText);
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