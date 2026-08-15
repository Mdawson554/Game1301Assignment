using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float movementSpeed;
    [SerializeField] float rotationSpeed;
    [SerializeField] float runningSpeedMultiplier;
    [SerializeField] Transform cameraTransform;
    [SerializeField] private float gravity = -9.8f;
    [SerializeField] private float jumpVelocity = 10f;
    
    [Header("Ground Check")]
    [SerializeField] private Vector3 groundCheckOffset;
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private float groundCheckRadius;
    [SerializeField] private LayerMask groundLayer;
    
    [Header("Input Actions")]
    [SerializeField] InputActionReference moveInputAction;
    [SerializeField] InputActionReference runInputAction;
    [SerializeField] InputActionReference jumpInputAction;
    
    private bool _isGrounded;
    private bool _isJumping;
    Vector2 moveInput;
    Rigidbody rb;
    Animator anim;
    private Vector3 _velocity;
    float activeRunningSpeedMultiplier = 1f;
    
    private bool currentlyWalking = false;
    private bool currentlyRunning = false;
    private bool currentlyJumping = false;
 
    readonly int walkingAnimatorHash = Animator.StringToHash("Walking");
    readonly int runningAnimatorHash = Animator.StringToHash("Running");
    readonly int jumpingAnimatorHash = Animator.StringToHash("Jumping");
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        jumpInputAction.action.performed += Jump;
    }

    private void Jump(InputAction.CallbackContext ctx)
    {
        CheckGrounded();
        if (_isGrounded)
        {
            SetJumpingAnimation(true);
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z); 
            rb.AddForce(0, jumpVelocity, 0, ForceMode.Impulse);
            _isGrounded = false;
            _isJumping = true;
        }
    }

    void Update()
    {
        moveInput = moveInputAction.action.ReadValue<Vector2>();
        bool isShiftPressed = runInputAction.action.IsPressed();
        bool isMoving = moveInput.magnitude > 0;
        bool shouldWalk = isMoving && !isShiftPressed;
        bool shouldRun = isMoving && isShiftPressed;
        bool shouldIdle = !isMoving;

        if (shouldRun && !currentlyRunning)
        {
            currentlyWalking = false;
            anim.SetBool(walkingAnimatorHash, false);
            currentlyRunning = true;
            anim.SetBool(runningAnimatorHash, true);
            activeRunningSpeedMultiplier = runningSpeedMultiplier;
        }
        else if (shouldWalk && !currentlyWalking)
        {
            currentlyRunning = false;
            anim.SetBool(runningAnimatorHash, false);
            currentlyWalking = true;
            anim.SetBool(walkingAnimatorHash, true);
            activeRunningSpeedMultiplier = 1f;
        }
        else if (shouldIdle && (currentlyWalking || currentlyRunning))
        {
            SetWalkingAnimation(false);
            SetRunningAnimation(false);
            activeRunningSpeedMultiplier = 1f;
        }
    }

    private void FixedUpdate()
    {
        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;
        cameraForward.y = 0;
        cameraRight.y = 0;
        cameraForward.Normalize();
        cameraRight.Normalize();
        Vector3 moveDirection = cameraForward * moveInput.y + cameraRight * moveInput.x;
        Vector3 velocity = moveDirection * movementSpeed * activeRunningSpeedMultiplier;
        rb.linearVelocity = new Vector3(velocity.x, rb.linearVelocity.y, velocity.z);
        CheckGrounded();
        if (_isGrounded && currentlyJumping)
        {
            SetJumpingAnimation(false);
            _isJumping = false;
        }
        if (moveDirection.magnitude > 0)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            Quaternion finalRotation = Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed);
            rb.MoveRotation(finalRotation);
        }
    }
    private void SetWalkingAnimation(bool value)
    {
        if (currentlyWalking != value)
        {
            currentlyWalking = value;
            anim.SetBool(walkingAnimatorHash, value);
        }
    }
    private void SetRunningAnimation(bool value)
    {
        if (currentlyRunning != value)
        {
            currentlyRunning = value;
            anim.SetBool(runningAnimatorHash, value);
        }
    }
    private void SetJumpingAnimation(bool value)
    {
        if (currentlyJumping != value)
        {
            currentlyJumping = value;
            anim.SetBool(jumpingAnimatorHash, value);
        }
    }

    public bool IsGrounded()
    {
        return _isGrounded;
    }

    private void CheckGrounded()
    {
        bool wasGrounded = _isGrounded;
        _isGrounded = Physics.SphereCast(
            transform.position + groundCheckOffset,
            groundCheckRadius,
            Vector3.down,
            out RaycastHit hit,
            groundCheckDistance,
            groundLayer
        );
        if (wasGrounded != _isGrounded)
        {
            if (hit.collider != null)
            {
                Debug.Log($"[GROUND_CHECK] Hit: {hit.collider.gameObject.name}");
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.purple;
        Gizmos.DrawSphere(transform.position + groundCheckOffset, groundCheckRadius);
        Gizmos.DrawSphere(transform.position + groundCheckOffset + Vector3.down * groundCheckDistance, groundCheckRadius);
        Gizmos.DrawCube(transform.position + groundCheckOffset + Vector3.down * groundCheckDistance / 2, 
            new Vector3(1.5f * groundCheckRadius, groundCheckDistance, 1.5f * groundCheckRadius));
    }
}