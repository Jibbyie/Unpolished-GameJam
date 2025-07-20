using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float playerSpeed = 5f;
    [SerializeField] private Rigidbody2D playerRB;

    private void Awake()
    {
        if(playerRB == null)
        {
            playerRB = GetComponent<Rigidbody2D>();
        }
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        playerRB.linearVelocity = new Vector2(horizontalInput *  playerSpeed, verticalInput * playerSpeed);
    }
}
