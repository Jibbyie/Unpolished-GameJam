using TMPro;
using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    private int itemsCollected = 0;

    [Header("References")]
    public GameObject jormungandrObject;
    public PlayerStats playerStats;

    [Header("UI Elements")]
    [SerializeField] private TMP_Text itemCountText;
    [SerializeField] private TMP_Text memoryText; // The UI element for displaying memories
    [SerializeField] private TMP_Text oxygenText;

    [Header("Timings")]
    [SerializeField] private float textFadeTime = 1f; // How long to fade in/out
    [SerializeField] private float textDisplayTime = 2.5f; // How long the memory stays on screen

    private void Awake()
    {
        playerStats = FindFirstObjectByType<PlayerStats>();
    }
    private void Update()
    {
        oxygenText.text = "Oxygen: " + playerStats.currentOxygen.ToString("F0");
    }

    public void TriggerGameOver()
    {
        Debug.Log("You Died");
    }

    public void OnItemCollected(int itemID, string newMemoryText)
    {
        itemsCollected++;
        itemCountText.text = "Items Collected: " + itemsCollected.ToString();

        // Set the text and start the routine to show it
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
        yield return FadeTextToFullAlpha(textFadeTime, memoryText); // Fade In
        yield return new WaitForSeconds(textDisplayTime);           // Wait
        yield return FadeTextToZeroAlpha(textFadeTime, memoryText); // Fade Out
    }

    // --- FADE COROUTINES --- // https://discussions.unity.com/t/fading-in-out-gui-text-with-c-solved/613416/2 
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