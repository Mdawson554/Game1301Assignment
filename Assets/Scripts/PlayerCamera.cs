using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float cameraRotationResponse = 10f;
    [SerializeField] private float cameraTurnThreshold = -0.85f;

    public float CameraRotationResponse => cameraRotationResponse;

    private void OnEnable()
    {
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main?.transform;
        }
    }
    public Vector3 GetCameraForward()
    {
        if (cameraTransform == null)
        {
            return Vector3.forward;
        }

        Vector3 cameraForward = cameraTransform.forward;
        cameraForward.y = 0f;

        if (cameraForward.sqrMagnitude <= 0.001f)
        {
            return Vector3.forward;
        }

        return cameraForward.normalized;
    }
    public Vector3 GetCameraRight()
    {
        if (cameraTransform == null)
        {
            return Vector3.right;
        }
        Vector3 cameraRight = cameraTransform.right;
        cameraRight.y = 0f;
        if (cameraRight.sqrMagnitude <= 0.001f)
        {
            return Vector3.right;
        }
        return cameraRight.normalized;
    }

    public bool ShouldTurnToCamera(Vector2 moveInput)
    {
        return moveInput.y > 0.1f;
    }
    public Quaternion GetTargetRotation(Vector3 moveDirection)
    {
        if (moveDirection.sqrMagnitude <= 0.001f)
        {
            return Quaternion.identity;
        }
        return Quaternion.LookRotation(moveDirection.normalized);
    }
}