using UnityEngine;

public class PlayerLocomotion : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private float runningSpeedMultiplier = 1.5f;
    [SerializeField] private Transform cameraTransform;

    [Header("Camera Rotation")]
    [SerializeField] private float cameraRotationResponse = 10f;
    [SerializeField] private float cameraTurnThreshold = -0.85f;

    private Vector3 moveDirection;
    private float activeSpeedMultiplier = 1f;

    public Vector3 MoveDirection => moveDirection;
    public Vector2 MoveInput { get; private set; }
    public bool IsMoving { get; private set; }
    public bool IsRunning { get; private set; }
    public float MovementSpeed => movementSpeed * activeSpeedMultiplier;
    public float CameraRotationResponse => cameraRotationResponse;

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
        moveDirection = cameraForward * MoveInput.y + cameraRight * MoveInput.x;
        if (moveDirection.sqrMagnitude > 1f)
        {
            moveDirection.Normalize();
        }
    }

    public bool ShouldTurnToCamera(Transform playerTransform)
    {
        if (cameraTransform == null || playerTransform == null)
        {
            return false;
        }
        Vector3 playerForward = playerTransform.forward;
        Vector3 cameraForward = cameraTransform.forward;
        playerForward.y = 0f;
        cameraForward.y = 0f;
        if (playerForward.sqrMagnitude <= 0.001f || cameraForward.sqrMagnitude <= 0.001f)
        {
            return false;
        }
        playerForward.Normalize();
        cameraForward.Normalize();
        float facingDot = Vector3.Dot(playerForward, cameraForward);
        return facingDot <= cameraTurnThreshold;
    }

    public Quaternion GetCameraRotation()
    {
        if (cameraTransform == null)
        {
            return Quaternion.identity;
        }
        Vector3 cameraForward = cameraTransform.forward;
        cameraForward.y = 0f;
        if (cameraForward.sqrMagnitude <= 0.001f)
        {
            return Quaternion.identity;
        }
        cameraForward.Normalize();
        return Quaternion.LookRotation(cameraForward);
    }
}