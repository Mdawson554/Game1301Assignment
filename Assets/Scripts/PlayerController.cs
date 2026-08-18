using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Jump")]
    [SerializeField] private float jumpVelocity = 10f;

    [Header("Ground Check")]
    [SerializeField] private Vector3 groundCheckOffset;
    [SerializeField] private float groundCheckDistance = 0.2f;
    [SerializeField] private float groundCheckRadius = 0.3f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Dependencies")]
    [SerializeField] private PlayerInputManager playerInput;
    [SerializeField] private PlayerLocomotion playerLocomotion;
    [SerializeField] private PlayerAnimator playerAnimator;

    private Rigidbody rb;
    private bool _isGrounded;
    private bool _wasGrounded;
    private bool _isJumping;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (playerInput == null)
        {
            playerInput = GetComponent<PlayerInputManager>();
        }
        if (playerLocomotion == null)
        {
            playerLocomotion = GetComponent<PlayerLocomotion>();
        }
        if (playerAnimator == null)
        {
            playerAnimator = GetComponent<PlayerAnimator>();
        }
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
        UpdateLocomotion();
        CheckGrounded();
        HandleLanding();
        ApplyMovement();
        ApplyRotation();
        UpdatePlayerAnimator();
    }

    private void ShowMouse(bool value)
    {
        Cursor.visible = value;
        Cursor.lockState = value ? CursorLockMode.None : CursorLockMode.Locked;
    }

    private void UpdateLocomotion()
    {
        bool isActionLocked = playerInput.IsAttacking || playerInput.IsInteracting;
        playerLocomotion.UpdateLocomotion(playerInput.MoveInput, playerInput.IsRunning, isActionLocked);
    }

    private void ApplyMovement()
    {
        Vector3 velocity = playerLocomotion.MoveDirection * playerLocomotion.MovementSpeed;
        rb.linearVelocity = new Vector3(velocity.x, rb.linearVelocity.y, velocity.z);
    }

    private void ApplyRotation()
    {
        if (!playerLocomotion.ShouldTurnToCamera(transform))
        {
            return;
        }
        Quaternion targetRotation = playerLocomotion.GetCameraRotation();
        Quaternion finalRotation = Quaternion.Slerp(rb.rotation, targetRotation, playerLocomotion.CameraRotationResponse * Time.fixedDeltaTime);
        rb.MoveRotation(finalRotation);
    }

    private void UpdatePlayerAnimator()
    {
        playerAnimator.UpdateAnimationState(
            playerLocomotion.MoveInput,
            playerLocomotion.IsRunning,
            playerInput.IsInteracting,
            playerInput.IsAttacking,
            _isJumping
        );
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
        return playerInput != null && playerInput.IsInteracting;
    }

    public bool IsAttacking()
    {
        return playerInput != null && playerInput.IsAttacking;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.purple;
        Gizmos.DrawSphere(transform.position + groundCheckOffset, groundCheckRadius);
        Gizmos.DrawSphere(transform.position + groundCheckOffset + Vector3.down * groundCheckDistance, groundCheckRadius);
        Gizmos.DrawCube(transform.position + groundCheckOffset + Vector3.down * groundCheckDistance / 2f, new Vector3(1.5f * groundCheckRadius, groundCheckDistance, 1.5f * groundCheckRadius));
    }
}