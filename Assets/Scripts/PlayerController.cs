using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float movementSpeed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float runningSpeedMultiplier;
    [SerializeField] private Transform cameraTransform;

    [Header("Jump")]
    [SerializeField] private float gravity = -9.8f;
    [SerializeField] private float jumpVelocity = 10f;

    [Header("Ground Check")]
    [SerializeField] private Vector3 groundCheckOffset;
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private float groundCheckRadius;
    [SerializeField] private LayerMask groundLayer;

    [Header("Input Actions")]
    [SerializeField] private InputActionReference moveInputAction;
    [SerializeField] private InputActionReference runInputAction;
    [SerializeField] private InputActionReference jumpInputAction;
    [SerializeField] private InputActionReference InteractInputAction;

    private Rigidbody rb;
    private Animator anim;

    private Vector2 moveInput;

    private bool _isGrounded;
    private bool _wasGrounded;
    private bool _isJumping;

    private float activeRunningSpeedMultiplier = 1f;
    private readonly int walkingAnimatorHash = Animator.StringToHash("Walking");
    private readonly int runningAnimatorHash = Animator.StringToHash("Running");
    private readonly int jumpingAnimatorHash = Animator.StringToHash("Jumping");
    private readonly int InteractingAnimatorHash = Animator.StringToHash("Interacting");

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        jumpInputAction.action.performed += Jump;
    }

    private void OnDestroy()
    {
        if (jumpInputAction != null)
        {
            jumpInputAction.action.performed -= Jump;
        }
    }

    private void Update()
    {
        moveInput = moveInputAction.action.ReadValue<Vector2>();
        
        bool isMoving = moveInput.sqrMagnitude > 0.01f;
        bool isRunning = runInputAction.action.IsPressed() && isMoving;
        bool isInteracting = InteractInputAction.action.IsPressed();
        
        UpdateMovementAnimation(isMoving, isRunning, isInteracting);
        anim.SetBool(jumpingAnimatorHash, _isJumping);
    }

    private void FixedUpdate()
    {
        CheckGrounded();
        HandleLanding();
        MovePlayer();
        RotatePlayer();
    }

    private void UpdateMovementAnimation(bool isMoving, bool isRunning, bool isInteracting)
    {
        if (isRunning)
        {
            anim.SetBool(walkingAnimatorHash, false);
            anim.SetBool(runningAnimatorHash, true);
            anim.SetBool(InteractingAnimatorHash, false);
            activeRunningSpeedMultiplier = runningSpeedMultiplier;
        }
        else if (isMoving)
        {
            anim.SetBool(walkingAnimatorHash, true);
            anim.SetBool(runningAnimatorHash, false);
            anim.SetBool(InteractingAnimatorHash, false);
            activeRunningSpeedMultiplier = 1f;
        }
        else if (isInteracting)
        {
            anim.SetBool(walkingAnimatorHash, false);
            anim.SetBool(runningAnimatorHash, true);
            anim.SetBool(InteractingAnimatorHash, true);
        }
        else
        {
            anim.SetBool(walkingAnimatorHash, false);
            anim.SetBool(runningAnimatorHash, false);
            anim.SetBool(InteractingAnimatorHash, false);
            activeRunningSpeedMultiplier = 1f;
        }
    }

    private void MovePlayer()
    {
        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;
        cameraForward.y = 0f;
        cameraRight.y = 0f;
        cameraForward.Normalize();
        cameraRight.Normalize();
        Vector3 moveDirection =
            cameraForward * moveInput.y +
            cameraRight * moveInput.x;
        Vector3 velocity =
            moveDirection *
            movementSpeed *
            activeRunningSpeedMultiplier;
        rb.linearVelocity = new Vector3(
            velocity.x,
            rb.linearVelocity.y,
            velocity.z
        );
    }

    private void RotatePlayer()
    {
        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;
        cameraForward.y = 0f;
        cameraRight.y = 0f;
        cameraForward.Normalize();
        cameraRight.Normalize();
        Vector3 moveDirection =
            cameraForward * moveInput.y +
            cameraRight * moveInput.x;
        if (moveDirection.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation =
                Quaternion.LookRotation(moveDirection);

            Quaternion finalRotation =
                Quaternion.Slerp(
                    rb.rotation,
                    targetRotation,
                    rotationSpeed
                );
            rb.MoveRotation(finalRotation);
        }
    }
    
    private void Jump(InputAction.CallbackContext ctx)
    {
        CheckGrounded();
        if (!_isGrounded || _isJumping)
        {
            return;
        }
        _isJumping = true;
        rb.linearVelocity = new Vector3(
            rb.linearVelocity.x,
            0f,
            rb.linearVelocity.z
        );
        rb.AddForce(
            Vector3.up * jumpVelocity,
            ForceMode.Impulse
        );
        _isGrounded = false;
        anim.SetBool(jumpingAnimatorHash, true);
    }

    private void HandleLanding()
    {
        if (!_wasGrounded && _isGrounded && _isJumping)
        {
            _isJumping = false;

            anim.SetBool(jumpingAnimatorHash, false);
        }
        _wasGrounded = _isGrounded;
    }

    public bool IsGrounded()
    {
        return _isGrounded;
    }

    private void CheckGrounded()
    {
        _isGrounded = Physics.SphereCast(
            transform.position + groundCheckOffset,
            groundCheckRadius,
            Vector3.down,
            out RaycastHit hit,
            groundCheckDistance,
            groundLayer
        );
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.purple;
        Gizmos.DrawSphere(
            transform.position + groundCheckOffset,
            groundCheckRadius
        );
        Gizmos.DrawSphere(
            transform.position +
            groundCheckOffset +
            Vector3.down * groundCheckDistance,
            groundCheckRadius
        );
        Gizmos.DrawCube(
            transform.position +
            groundCheckOffset +
            Vector3.down * groundCheckDistance / 2f,
            new Vector3(
                1.5f * groundCheckRadius,
                groundCheckDistance,
                1.5f * groundCheckRadius
            )
        );
    }
}