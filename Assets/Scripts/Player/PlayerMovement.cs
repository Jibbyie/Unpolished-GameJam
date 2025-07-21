using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float playerSpeed = 5f;
    [SerializeField] private Rigidbody2D playerRB;

    //Add a reference for the child model
    [SerializeField] private Transform characterModel;

    private Animator animator;

    private void Awake()
    {
        if (playerRB == null)
        {
            playerRB = GetComponent<Rigidbody2D>();
        }
        // Get the Animator from the child object
        animator = GetComponentInChildren<Animator>();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        playerRB.linearVelocity = new Vector2(horizontalInput * playerSpeed, verticalInput * playerSpeed);

        bool isMoving = playerRB.linearVelocity.magnitude > 0.1f;
        animator.SetBool("isWalking", isMoving);

        // Flip the child model, not the parent 
        if (horizontalInput > 0.01f)
        {
            // Facing Right
            characterModel.localScale = new Vector3(Mathf.Abs(characterModel.localScale.x), characterModel.localScale.y, characterModel.localScale.z);
        }
        else if (horizontalInput < -0.01f)
        {
            // Facing Left
            characterModel.localScale = new Vector3(-Mathf.Abs(characterModel.localScale.x), characterModel.localScale.y, characterModel.localScale.z);
        }
    }
}