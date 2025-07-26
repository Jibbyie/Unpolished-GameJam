using UnityEngine;

public class MonsterHitbox : MonoBehaviour
{
    public MonsterAI parentAI;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            parentAI.PlayerWasCaught();
        }
    }
}