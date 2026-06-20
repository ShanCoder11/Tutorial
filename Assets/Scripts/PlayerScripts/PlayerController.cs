using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // ===== VARIABLES =====
    [Header("Movement Settings")]
    [SerializeField] float playerSpeed = 5f;
    [SerializeField] private float playerJumpHeight = 10f;

    [Header("Ground Check Settings")]
    [SerializeField] public Transform groundCheck;
    [SerializeField] public LayerMask groundLayer;
    [SerializeField] private float groundCheckRadius = 0.2f;

    [Header("Camera")]
    [SerializeField] public Transform orientation;

    PlayerInput playerInput;
    InputAction moveAction;
    Rigidbody rb;
    private bool isGrounded;
    Vector2 moveInput;

    // ===== STATE MACHINE =====
    private PlayerState currentState;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions.FindAction("Walking");

        // Initialize it to IDLE state
        ChangeState(new IdleState(this));
    }

    // Update is called once per frame
    void Update()
    {
        // Read input
        if (moveAction != null)
        {
            moveInput = moveAction.ReadValue<Vector2>();
        }

        // Ground check
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);

        // Current state handles its own logic
        currentState.Update(); 
    }

    private void FixedUpdate()
    {
        // Current state handles physics
        currentState.FixedUpdate();
    }

    // ===== STATE MACHINE CORE =====
    public void ChangeState(PlayerState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
    }

    // ===== PUBLIC GETTERS (states read from here) =====
    public Vector2 GetMoveInput() => moveInput;
    public bool IsGrounded() => isGrounded;
    public Rigidbody GetRigidbody() => rb;
    public Transform GetCameraTransform() => orientation;
    public Transform GetPlayerTransform() => transform;

    public float GetPlayerSpeed() => playerSpeed;
    public float GetJumpHeight() => playerJumpHeight;
    
    public bool IsRunning() => playerInput.actions.FindAction("Running").ReadValue<float>() == 1f;
    public bool GetJumpInput() => playerInput.actions.FindAction("Jumping").ReadValue<float>() == 1f;
}
