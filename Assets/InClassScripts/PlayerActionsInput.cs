/*using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-2)]
public class PlayerActionsInput : MonoBehaviour, PlayerControls.IPlayerActionsMapActions
{
    public bool AttackPressed { get; private set; }
    public bool GatherPressed { get; private set; }

    private void OnEnable()
    {
        if (PlayerInputManager.Instance?.PlayerControls == null)
        {
            Debug.LogError("Player controls is not initialized - cannot enable");
            return;
        }

        PlayerInputManager.Instance.PlayerControls.PlayerActionsMap.Enable();
        PlayerInputManager.Instance.PlayerControls.PlayerActionsMap.SetCallbacks(this);
    }

    private void OnDisable()
    {
        if (PlayerInputManager.Instance?.PlayerControls == null)
        {
            Debug.LogError("Player controls is not initialized - cannot disable");
            return;
        }

        PlayerInputManager.Instance.PlayerControls.PlayerActionsMap.Disable();
        PlayerInputManager.Instance.PlayerControls.PlayerActionsMap.RemoveCallbacks(this);
    }

    private void LateUpdate()
    {
        AttackPressed = false;
        GatherPressed = false;
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        AttackPressed = true;
    }

    public void OnGather(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        GatherPressed = true;
    }
}*/