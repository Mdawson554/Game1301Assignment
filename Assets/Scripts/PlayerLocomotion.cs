using UnityEngine;

public class PlayerLocomotion : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private float runningSpeedMultiplier = 1.5f;

    [Header("Dependencies")]
    [SerializeField] private PlayerCamera playerCamera;

    private Vector3 moveDirection;
    private float activeSpeedMultiplier = 1f;

    public Vector3 MoveDirection => moveDirection;
    public Vector2 MoveInput { get; private set; }
    public bool IsMoving { get; private set; }
    public bool IsRunning { get; private set; }
    public float MovementSpeed => movementSpeed * activeSpeedMultiplier;

    private void Awake()
    {
        if (playerCamera == null)
        {
            playerCamera = GetComponent<PlayerCamera>();
        }
    }

    public void UpdateLocomotion(Vector2 moveInput, bool isRunning, bool isBlocked)
    {
        MoveInput = moveInput;
        IsMoving = moveInput.sqrMagnitude > 0.01f;
        IsRunning = isRunning && IsMoving && !isBlocked;
        activeSpeedMultiplier = IsRunning ? runningSpeedMultiplier : 1f;
        if (isBlocked)
        {
            moveDirection = Vector3.zero;
            return;
        }
        CalculateCameraRelativeMovement();
    }

    private void CalculateCameraRelativeMovement()
    {
        if (playerCamera == null)
        {
            moveDirection = Vector3.zero;
            return;
        }
        Vector3 cameraForward = playerCamera.GetCameraForward();
        Vector3 cameraRight = playerCamera.GetCameraRight();
        moveDirection = cameraForward * MoveInput.y + cameraRight * MoveInput.x;
        if (moveDirection.sqrMagnitude > 1f)
        {
            moveDirection.Normalize();
        }
    }
}