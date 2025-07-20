using UnityEngine;
using TMPro;
public class PlayerCollector : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Collectible"))
        {
            Destroy(collision.gameObject);
            gameManager.OnItemCollected();
        }
    }
}
