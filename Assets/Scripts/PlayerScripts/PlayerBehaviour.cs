using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBehaviour : MonoBehaviour
{
    // VARIABLES -- VARIABLES -- VARIABLES -- VARIABLES -- VARIABLES -- VARIABLES -- VARIABLES -- VARIABLES -- VARIABLES -- VARIABLES -- VARIABLES -- VARIABLES -- VARIABLES -- VARIABLES

    PlayerInput playerInput;    
    InputAction moveAction;
    Rigidbody rb;
    public Transform orientation;

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
        // if we have to make a dashing movement or rolling one, etc., it would be very hard to make a billion if else statements

        Transform cameraTransform = Camera.main.transform;

        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;

        cameraForward.y = 0;
        cameraRight.y = 0;
        cameraForward.Normalize();
        cameraRight.Normalize();

        Vector3 moveDirection = (cameraForward * moveInput.y + cameraRight * moveInput.x).normalized;
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
        // that's why i'm trying to change that

        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);
        jumpInputReceived = playerInput.actions.FindAction("Jumping").ReadValue<float>() == 1f;
        if (jumpInputReceived && isGrounded)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, playerJumpHeight, rb.linearVelocity.z);
            jumpInputReceived = false;
        }
    }
}