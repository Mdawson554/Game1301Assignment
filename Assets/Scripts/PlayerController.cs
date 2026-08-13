using System;
using UnityEngine;
using UnityEngine.InputSystem;
 
public class PlayerController : MonoBehaviour
{
    [SerializeField] float movementSpeed;
    [SerializeField] float rotationSpeed;
    [SerializeField] float runningSpeedMulitplier;
    [SerializeField] Transform cameraTransform;
 
    //jump variables
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
    [SerializeField] InputActionReference jumpInputAction; //jump input
    private bool _isGrounded;
    private bool groundedLastFrame;
    private bool _isJumping;
    
    //game variables
    Vector2 moveInput;
    Rigidbody rb;
    Animator anim;
    private Vector3 _velocity;
    float activeRunningSpeedMultiplier = 1f;
 
    readonly int walkingAnimatorHash = Animator.StringToHash("Walking");
    readonly int runningAnimatorHash = Animator.StringToHash("Running");
    readonly int jumpingAnimatorHash = Animator.StringToHash("Jumping");    //jump anim
 
    
    public event Action OnJumpEvent;
    
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
            anim.SetBool(jumpingAnimatorHash, true);
            rb.AddForce(0,jumpVelocity,0, ForceMode.Impulse);
            _isGrounded = false;
        }
    }
 
    void Update()
    {
       moveInput = moveInputAction.action.ReadValue<Vector2>();
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
        Vector3 velocity = moveDirection * movementSpeed;
        rb.linearVelocity = new Vector3(velocity.x, rb.linearVelocity.y, velocity.z);
        if (moveDirection != Vector3.zero)
        {
            anim.SetBool(walkingAnimatorHash, true);
        }
       if (moveDirection == Vector3.zero)
        {
            anim.SetBool(walkingAnimatorHash, false);
            anim.SetBool(runningAnimatorHash, false);
        }
        if (runInputAction.action.IsPressed())
        {
            anim.SetBool(runningAnimatorHash, true);
            activeRunningSpeedMultiplier = runningSpeedMulitplier;
        }
        else
        {
           anim.SetBool(runningAnimatorHash, false);
           activeRunningSpeedMultiplier = 1;
        }
        CheckGrounded();
        groundedLastFrame = _isGrounded;
        if (groundedLastFrame)
        {
            anim.SetBool(jumpingAnimatorHash, false);
        }
       Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
       Quaternion finalRotation = Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed);
       rb.MoveRotation(finalRotation);
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
    
    void OnDrawGizmos()
    {
        Gizmos.color = Color.purple;
        Gizmos.DrawSphere(transform.position + groundCheckOffset, groundCheckRadius);
        Gizmos.DrawSphere(transform.position + groundCheckOffset + Vector3.down * groundCheckDistance, groundCheckRadius);
        Gizmos.DrawCube(transform.position + groundCheckOffset + Vector3.down * groundCheckDistance/2, 
            new Vector3(1.5f* groundCheckRadius, groundCheckDistance , 1.5f * groundCheckRadius) );
    }
}

