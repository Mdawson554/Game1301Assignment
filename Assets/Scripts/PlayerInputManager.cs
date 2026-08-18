using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour
{
    [Header("Input Actions")]
    [SerializeField] private InputActionReference moveInputAction;
    [SerializeField] private InputActionReference runInputAction;
    [SerializeField] private InputActionReference jumpInputAction;
    [SerializeField] private InputActionReference interactInputAction;
    [SerializeField] private InputActionReference attackInputAction;

    public Vector2 MoveInput { get; private set; }
    public bool IsRunning { get; private set; }
    public bool IsInteracting { get; private set; }
    public bool IsAttacking { get; private set; }
    public event Action JumpPressed;

    private void OnEnable()
    {
        if (jumpInputAction != null)
        {
            jumpInputAction.action.performed += OnJumpPerformed;
        }
    }

    private void OnDisable()
    {
        if (jumpInputAction != null)
        {
            jumpInputAction.action.performed -= OnJumpPerformed;
        }
    }

    private void Update()
    {
        ReadInput();
    }

    private void ReadInput()
    {
        ReadMovementInput();
        ReadActionInput();
    }

    private void ReadMovementInput()
    {
        if (moveInputAction != null)
        {
            MoveInput = moveInputAction.action.ReadValue<Vector2>();
        }
        else
        {
            MoveInput = Vector2.zero;
        }
    }

    private void ReadActionInput()
    {
        IsRunning = runInputAction != null && runInputAction.action.IsPressed();
        IsInteracting = interactInputAction != null && interactInputAction.action.IsPressed();
        IsAttacking = attackInputAction != null && attackInputAction.action.IsPressed();
    }

    private void OnJumpPerformed(InputAction.CallbackContext context)
    {
        JumpPressed?.Invoke();
    }
}