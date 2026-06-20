using Unity.VisualScripting;
using UnityEngine;

// ===== BASE STATE CLASS =====
public abstract class PlayerState
{
    protected PlayerController controller;

    public PlayerState(PlayerController controller)
    {
        this.controller = controller;
    }

    public virtual void Enter() { }
    public virtual void Update() { }
    public virtual void FixedUpdate() { }
    public virtual void Exit() { }

    // ===== UTILITY CLASSES (classes used by multiple states) =====
    protected void MovePlayer(float speed)
    {
        Transform cameraTransform = controller.GetCameraTransform();
        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;

        cameraForward.y = 0;
        cameraRight.y = 0;
        cameraForward.Normalize();
        cameraRight.Normalize();

        Vector2 moveInput = controller.GetMoveInput();
        Vector3 moveDirection = (cameraForward * moveInput.y + cameraRight * moveInput.x).normalized;

        controller.GetRigidbody().linearVelocity = new Vector3(moveDirection.x * speed, controller.GetRigidbody().linearVelocity.y, moveDirection.z * speed);
    }
}

// ===== IDLE STATE =====
public class IdleState : PlayerState
{
    public IdleState(PlayerController controller) : base(controller) { }

    public override void Enter()
    {
        // Stop movement when entering idle
        controller.GetRigidbody().linearVelocity = Vector3.zero;
    }

    public override void Update()
    {
        // Check if we should transition
        if (controller.GetMoveInput().magnitude > 0)
        {
            if (controller.IsRunning())
            {
                controller.ChangeState(new RunningState(controller));
            }
            else
            {
                controller.ChangeState(new WalkingState(controller));
            }
        }

        if (controller.GetJumpInput() && controller.IsGrounded())
        {
            controller.ChangeState(new JumpingState(controller));
        }
    }

    public override void FixedUpdate()
    {
        // Idle does nothing in physics
    }
}

// ===== MOVING STATE =====
public class WalkingState : PlayerState
{
    public WalkingState(PlayerController controller) : base(controller) { }

    public override void Update()
    {
        // Transition to running if sprint input
        if (controller.IsRunning())
        {
            controller.ChangeState(new RunningState(controller));
            return;
        }

        // Transition to idle if no input
        if (controller.GetMoveInput().magnitude == 0)
        {
            controller.ChangeState(new IdleState(controller));
            return;
        }

        // Jump from moving
        if (controller.GetJumpInput() && controller.IsGrounded())
        {
            controller.ChangeState(new JumpingState(controller));
        }
    }

    public override void FixedUpdate()
    {
        MovePlayer(controller.GetPlayerSpeed());
    }
}

// ===== RUNNING STATE =====
public class RunningState : PlayerState
{
    private float runSpeed = 10f;

    public RunningState(PlayerController controller) : base(controller) { }

    public override void Update()
    {
        // Stop running if sprint input released
        if (!controller.IsRunning())
        {
            if (controller.GetMoveInput().magnitude > 0)
            {
                controller.ChangeState(new WalkingState(controller));
            }
            else
            {
                controller.ChangeState(new IdleState(controller));
            }
            return;
        }

        // Stop running if no movement input
        if (controller.GetMoveInput().magnitude == 0)
        {
            controller.ChangeState(new IdleState(controller));
            return;
        }

        // Jump from running
        if (controller.GetJumpInput() && controller.IsGrounded())
        {
            controller.ChangeState(new JumpingState(controller));
        }
    }

    public override void FixedUpdate()
    {
        MovePlayer(runSpeed);
    }
}

// ===== JUMPING STATE =====
public class JumpingState : PlayerState
{
    public JumpingState(PlayerController controller) : base(controller) { }

    public override void Enter()
    {
        // Apply jump force
        controller.GetRigidbody().linearVelocity = new Vector3(controller.GetRigidbody().linearVelocity.x, controller.GetJumpHeight(), controller.GetRigidbody().linearVelocity.z);
    }

    public override void Update()
    {
        // Stay in air until we land
        if (controller.IsGrounded())
        {
            // Return to idle, moving or running based on input
            if (controller.GetMoveInput().magnitude == 0)
            {
                controller.ChangeState(new IdleState(controller));
            }
            else if (controller.IsRunning())
            {
                controller.ChangeState(new RunningState(controller));
            }
            else
            {
                controller.ChangeState(new WalkingState(controller));
            }
        }
    }

    public override void FixedUpdate()
    {
        // Allow air movement while jumping
        Transform cameraTransform = controller.GetCameraTransform();
        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;

        cameraForward.y = 0;
        cameraRight.y = 0;
        cameraForward.Normalize();
        cameraRight.Normalize();

        Vector2 moveInput = controller.GetMoveInput();
        Vector3 moveDirection = (cameraForward * moveInput.y + cameraRight * moveInput.x).normalized;

        // Reduced air control (60% of ground speed)
        float airSpeed = controller.IsRunning() ? 10f * 0.6f : controller.GetPlayerSpeed() * 0.6f;
        
        controller.GetRigidbody().linearVelocity = new Vector3(moveDirection.x * airSpeed, controller.GetRigidbody().linearVelocity.y, moveDirection.z * airSpeed);
    }
}