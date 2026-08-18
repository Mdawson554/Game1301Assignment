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

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }
    public void UpdateAnimationState(
        Vector2 moveInput,
        bool isRunning,
        bool isInteracting,
        bool isAttacking,
        bool isJumping)
    {
        bool isMoving = moveInput.sqrMagnitude > 0.01f;
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
    public void SetJumping(bool isJumping)
    {
        if (anim == null)
        {
            return;
        }
        anim.SetBool(jumpingAnimatorHash, isJumping);
    }
    private void SetLocomotion(
        bool walking,
        bool running)
    {
        if (anim == null)
        {
            return;
        }
        anim.SetBool(walkingAnimatorHash, walking);
        anim.SetBool(runningAnimatorHash, running);
    }
    private void SetActions(
        bool interacting,
        bool attacking)
    {
        if (anim == null)
        {
            return;
        }
        anim.SetBool(interactingAnimatorHash, interacting);
        anim.SetBool(attackingAnimatorHash, attacking);
    }
}