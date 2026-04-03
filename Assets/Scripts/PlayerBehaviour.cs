using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBehaviour : MonoBehaviour
{
    // VARIABLES -- VARIABLES -- VARIABLES -- VARIABLES -- VARIABLES -- VARIABLES -- VARIABLES -- VARIABLES -- VARIABLES -- VARIABLES -- VARIABLES -- VARIABLES -- VARIABLES -- VARIABLES

    PlayerInput playerInput;    
    InputAction moveAction;
    Rigidbody rb;

    private bool isGrounded;
    private bool jumpInputReceived = false;
    private bool runInputReceived = false;

    Vector2 moveInput;

    [Header("Movement Settings")]
    [SerializeField] float playerSpeed = 5f;
    [SerializeField] private float playerJumpHeight = 10f;

    [Header("Ground Check Settings")]
    [SerializeField] public Transform groundCheck;
    [SerializeField] public LayerMask groundLayer;
    [SerializeField] private float groundCheckRadius = 0.2f;



    // BUILT-IN CLASSES -- BUILT-IN CLASSES -- BUILT-IN CLASSES -- BUILT-IN CLASSES -- BUILT-IN CLASSES -- BUILT-IN CLASSES -- BUILT-IN CLASSES -- BUILT-IN CLASSES -- BUILT-IN CLASSES

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions.FindAction("Walking");
    }

    // Update is called once per frame
    void Update()
    {
        if (moveAction != null) 
        {
            moveInput = moveAction.ReadValue<Vector2>();
        }
    }

    void FixedUpdate()
    {
        MovePlayer();
        PlayerJump();
    }



    // CUSTOM CLASSES -- CUSTOM CLASSES -- CUSTOM CLASSES -- CUSTOM CLASSES -- CUSTOM CLASSES -- CUSTOM CLASSES -- CUSTOM CLASSES -- CUSTOM CLASSES -- CUSTOM CLASSES -- CUSTOM CLASSES

    void MovePlayer()
    {
        Debug.Log($"Player Normalized: {moveInput.sqrMagnitude}");
        Vector3 moveDirection = new Vector3(moveInput.x, 0, moveInput.y);
        rb.linearVelocity = new Vector3(moveDirection.x * playerSpeed, rb.linearVelocity.y, moveDirection.z * playerSpeed);

        runInputReceived = playerInput.actions.FindAction("Running").ReadValue<float>() == 1f;
        if (runInputReceived)
        {
            playerSpeed = 10f;
        }
        else
        {
            playerSpeed = 5f;
        }

    }

    void PlayerJump()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);
        jumpInputReceived = playerInput.actions.FindAction("Jumping").ReadValue<float>() == 1f;
        if (jumpInputReceived && isGrounded)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, playerJumpHeight, rb.linearVelocity.z);
            jumpInputReceived = false;
        }
    }
}