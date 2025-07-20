using UnityEngine;
using TMPro;
public class PlayerCollector : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Collectible"))
        {
            // 1. Get the component and its data FIRST
            var collectible = collision.gameObject.GetComponent<Collectible>();

            int collectibleID = collectible.collectibleID;
            string memory = collectible.memoryText; 

            // 2. Pass the data to the GameManager
            gameManager.OnItemCollected(collectibleID, memory);

            // 3. Destroy the object LAST
            Destroy(collision.gameObject);
        }
    }
}
