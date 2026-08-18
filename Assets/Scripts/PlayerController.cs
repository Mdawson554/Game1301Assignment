using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private float runningSpeedMultiplier = 1.5f;
    [SerializeField] private Transform cameraTransform;
    [Header("Camera Movement")]
    [SerializeField] private float cameraRotationResponse = 10f;
    [Header("Jump")]
    [SerializeField] private float jumpVelocity = 10f;
    [Header("Ground Check")]
    [SerializeField] private Vector3 groundCheckOffset;
    [SerializeField] private float groundCheckDistance = 0.2f;
    [SerializeField] private float groundCheckRadius = 0.3f;
    [SerializeField] private LayerMask groundLayer;
    [Header("Dependencies")]
    [SerializeField] private PlayerInputManager playerInput;
    [SerializeField] private PlayerAnimator playerAnimator;

    private Rigidbody rb;
    private Vector3 moveDirection;
    private bool _isGrounded;
    private bool _wasGrounded;
    private bool _isJumping;
    private float activeRunningSpeedMultiplier = 1f;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void Start()
    {
        ShowMouse(false);
    }

    private void OnEnable()
    {
        if (playerInput != null)
        {
            playerInput.JumpPressed += Jump;
        }
    }

    private void OnDisable()
    {
        if (playerInput != null)
        {
            playerInput.JumpPressed -= Jump;
        }
    }
    private void FixedUpdate()
    {
        CalculateCameraRelativeMovement();
        CheckGrounded();
        HandleLanding();
        CalculateMovementSpeed();
        ApplyMovement();
        ApplyRotation();
        UpdatePlayerAnimator();
    }
    private void ShowMouse(bool value)
    {
        Cursor.visible = value;
        Cursor.lockState = value ? CursorLockMode.None : CursorLockMode.Locked;
    }

    private void CalculateCameraRelativeMovement()
    {
        if (cameraTransform == null)
        {
            moveDirection = Vector3.zero;
            return;
        }
        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;
        cameraForward.y = 0f;
        cameraRight.y = 0f;
        cameraForward.Normalize();
        cameraRight.Normalize();
        moveDirection = cameraForward * playerInput.MoveInput.y + cameraRight * playerInput.MoveInput.x;
        if (moveDirection.sqrMagnitude > 1f)
        {
            moveDirection.Normalize();
        }
    }
    private void CalculateMovementSpeed()
    {
        if (playerInput.IsAttacking || playerInput.IsInteracting)
        {
            activeRunningSpeedMultiplier = 1f;
        }
        else if (playerInput.IsRunning)
        {
            activeRunningSpeedMultiplier = runningSpeedMultiplier;
        }
        else
        {
            activeRunningSpeedMultiplier = 1f;
        }
    }

    private void ApplyMovement()
    {
        Vector3 velocity = moveDirection * movementSpeed * activeRunningSpeedMultiplier;
        rb.linearVelocity = new Vector3(velocity.x, rb.linearVelocity.y, velocity.z);
    }

    private void ApplyRotation()
    {
        if (playerInput.MoveInput.sqrMagnitude <= 0.001f)
        {
            return;
        }
        Vector3 cameraForward = cameraTransform.forward;
        cameraForward.y = 0f;
        if (cameraForward.sqrMagnitude <= 0.001f)
        {
            return;
        }
        cameraForward.Normalize();
        Quaternion targetRotation = Quaternion.LookRotation(cameraForward);
        Quaternion finalRotation = Quaternion.Slerp(rb.rotation, targetRotation, cameraRotationResponse * Time.fixedDeltaTime);
        rb.MoveRotation(finalRotation);
    }
    private void UpdatePlayerAnimator()
    {
        playerAnimator.UpdateAnimationState(playerInput.MoveInput, playerInput.IsRunning, playerInput.IsInteracting, playerInput.IsAttacking, _isJumping);
    }

    private void Jump()
    {
        CheckGrounded();
        if (!_isGrounded || _isJumping)
        {
            return;
        }
        _isJumping = true;
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(Vector3.up * jumpVelocity, ForceMode.Impulse);
        _isGrounded = false;
    }
    private void CheckGrounded()
    {
        _isGrounded = Physics.SphereCast(transform.position + groundCheckOffset, groundCheckRadius, Vector3.down, out RaycastHit hit, groundCheckDistance, groundLayer);
    }

    private void HandleLanding()
    {
        if (!_wasGrounded && _isGrounded && _isJumping)
        {
            _isJumping = false;
        }
        _wasGrounded = _isGrounded;
    }
    public bool IsGrounded()
    {
        return _isGrounded;
    }

    public bool IsInteracting()
    {
        return playerInput != null &&
               playerInput.IsInteracting;
    }

    public bool IsAttacking()
    {
        return playerInput != null &&
               playerInput.IsAttacking;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.purple;
        Gizmos.DrawSphere(transform.position + groundCheckOffset, groundCheckRadius);
        Gizmos.DrawSphere(transform.position + groundCheckOffset + Vector3.down * groundCheckDistance, groundCheckRadius);
        Gizmos.DrawCube(transform.position + groundCheckOffset + Vector3.down * groundCheckDistance / 2f, new Vector3(1.5f * groundCheckRadius, groundCheckDistance, 1.5f * groundCheckRadius));
    }
}