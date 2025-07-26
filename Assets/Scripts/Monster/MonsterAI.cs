using Live2D.Cubism.Core;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MonsterAI : MonoBehaviour
{
    [Header("References")]
    public PlayerController player;
    public Transform modelTransform;
    [SerializeField] private GameObject screenFader;

    [Header("Behavior Settings")]
    public float speed = 2f;
    public float patrolDuration = 20f;
    public float swaySpeed = 1.5f;
    public float swayMagnitude = 15f;

    [Header("Spawning Settings")]
    public float minSpawnDistance = 4f;
    public float maxSpawnDistance = 8f;

    [Header("Audio")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource monsterAudioSource;
    [SerializeField] private AudioClip intenseTrack;
    [SerializeField] private AudioClip monsterGrowlSound;
    [SerializeField] private AudioClip monsterRoarSound;

    private float patrolTimer;
    private CubismModel cubismModel;
    private CubismParameter headParameter;
    private Coroutine caughtSequenceCoroutine;

    private void Awake()
    {
        cubismModel = GetComponentInChildren<CubismModel>();
        if (cubismModel != null)
        {
            headParameter = cubismModel.Parameters.FindById("Param");
        }
    }

    private void OnEnable()
    {
        patrolTimer = patrolDuration;
        caughtSequenceCoroutine = null;

        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        float randomDistance = Random.Range(minSpawnDistance, maxSpawnDistance);
        transform.position = (Vector2)player.transform.position + (randomDirection * randomDistance);

        if (musicSource != null && intenseTrack != null)
        {
            musicSource.clip = intenseTrack;
            musicSource.Play();
        }

        if (monsterAudioSource != null && monsterGrowlSound != null)
        {
            monsterAudioSource.PlayOneShot(monsterGrowlSound);
        }
    }

    private void OnDisable()
    {
        if (musicSource != null)
        {
            musicSource.Stop();
        }
    }

    private void Update()
    {
        if (player == null) return;

        patrolTimer -= Time.deltaTime;
        if (patrolTimer <= 0f)
        {
            gameObject.SetActive(false);
            return;
        }

        if (modelTransform != null)
        {
            Vector2 directionToPlayer = player.transform.position - transform.position;
            float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;
            modelTransform.rotation = Quaternion.Euler(0f, 0f, angle + 180f);
        }

        if (headParameter != null)
        {
            headParameter.Value = Mathf.Sin(Time.time * swaySpeed) * swayMagnitude;
        }

        if (!player.isHiding)
        {
            transform.position = Vector2.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);
        }
    }

    public void PlayerWasCaught()
    {
        // Check if the sequence is already running to avoid triggering it multiple times.
        if (caughtSequenceCoroutine == null)
        {
            caughtSequenceCoroutine = StartCoroutine(PlayerCaughtSequence());
        }
    }

    private IEnumerator PlayerCaughtSequence()
    {
        player.canMove = false;

        if (screenFader != null)
        {
            screenFader.SetActive(true);
        }

        AudioSource[] allAudioSources = FindObjectsOfType<AudioSource>();
        foreach (AudioSource source in allAudioSources)
        {
            source.Stop();
        }

        if (monsterAudioSource != null && monsterRoarSound != null)
        {
            monsterAudioSource.PlayOneShot(monsterRoarSound);
            yield return new WaitForSeconds(monsterRoarSound.length);
        }
        else
        {
            yield return new WaitForSeconds(2f);
        }

        SceneManager.LoadScene("MainMenu");
    }
}