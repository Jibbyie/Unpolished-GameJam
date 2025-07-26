using UnityEngine;
using TMPro;
public class PlayerCollector : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Collectible"))
        {
            var collectible = collision.gameObject.GetComponent<Collectible>();

            int collectibleID = collectible.collectibleID;
            string memory = collectible.memoryText;

            bool isFinal = collectible.isFinalItem; 
            gameManager.OnItemCollected(collectibleID, memory, isFinal); 

            Destroy(collision.gameObject);
        }
    }
}
