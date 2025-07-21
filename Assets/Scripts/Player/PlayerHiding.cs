using UnityEngine;

public class PlayerHiding : MonoBehaviour
{
    private PlayerController playerController;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the object we entered is tagged as a "HideSpot"
        if (other.gameObject.CompareTag("HideSpot"))
        {
            playerController.isHiding = true;
            Debug.Log("Player is now hiding.");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Check if the object we exited was a "HideSpot"
        if (other.gameObject.CompareTag("HideSpot"))
        {
            playerController.isHiding = false;
            Debug.Log("Player is no longer hiding.");
        }
    }
}