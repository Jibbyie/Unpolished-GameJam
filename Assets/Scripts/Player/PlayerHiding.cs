using Live2D.Cubism.Rendering;
using UnityEngine;

public class PlayerHiding : MonoBehaviour
{
    private PlayerController playerController;
    private GameManager gameManager;
    private CubismRenderController cubismRenderController;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        gameManager = FindFirstObjectByType<GameManager>();
        cubismRenderController = GetComponentInChildren<CubismRenderController>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("HideSpot"))
        {
            playerController.isHiding = true;

            if (gameManager != null)
            {
                gameManager.SetHidingVignette(true);
            }

            if (cubismRenderController != null)
            {
                cubismRenderController.Opacity = 0.5f;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("HideSpot"))
        {
            playerController.isHiding = false;

            if (gameManager != null)
            {
                gameManager.SetHidingVignette(false);
            }

            if (cubismRenderController != null)
            {
                cubismRenderController.Opacity = 1.0f;
            }
        }
    }
}