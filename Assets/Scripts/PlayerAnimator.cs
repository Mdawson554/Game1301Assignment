using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimator : MonoBehaviour
{
    private Animator anim;

    private readonly int walkingAnimatorHash = Animator.StringToHash("Walking");
    private readonly int runningAnimatorHash = Animator.StringToHash("Running");
    private readonly int jumpingAnimatorHash = Animator.StringToHash("Jumping");
    private readonly int interactingAnimatorHash = Animator.StringToHash("Interacting");
    private readonly int attackingAnimatorHash = Animator.StringToHash("Attacking");
    private readonly int velocityXAnimatorHash = Animator.StringToHash("VelocityX");
    private readonly int velocityYAnimatorHash = Animator.StringToHash("VelocityY");

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void UpdateAnimationState(Vector2 moveInput, bool isRunning, bool isInteracting, bool isAttacking, bool isJumping)
    {
        bool isMoving = moveInput.sqrMagnitude > 0.01f;
        SetLocomotionParameters(moveInput);
        if (isAttacking)
        {
            SetLocomotion(false, false);
            SetActions(false, true);
            SetJumping(isJumping);
            return;
        }
        if (isInteracting)
        {
            SetLocomotion(false, false);
            SetActions(true, false);
            SetJumping(isJumping);
            return;
        }
        if (isRunning && isMoving)
        {
            SetLocomotion(false, true);
            SetActions(false, false);
            SetJumping(isJumping);
            return;
        }
        if (isMoving)
        {
            SetLocomotion(true, false);
            SetActions(false, false);
            SetJumping(isJumping);
            return;
        }
        SetLocomotion(false, false);
        SetActions(false, false);
        SetJumping(isJumping);
    }

    private void SetLocomotionParameters(Vector2 moveInput)
    {
        anim.SetFloat(velocityXAnimatorHash, moveInput.x);
        anim.SetFloat(velocityYAnimatorHash, moveInput.y);
    }

    public void SetJumping(bool isJumping)
    {
        anim.SetBool(jumpingAnimatorHash, isJumping);
    }

    private void SetLocomotion(bool walking, bool running)
    {
        anim.SetBool(walkingAnimatorHash, walking);
        anim.SetBool(runningAnimatorHash, running);
    }

    private void SetActions(bool interacting, bool attacking)
    {
        anim.SetBool(interactingAnimatorHash, interacting);
        anim.SetBool(attackingAnimatorHash, attacking);
    }
}