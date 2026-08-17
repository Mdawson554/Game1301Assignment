using UnityEngine;

public class NPC : MonoBehaviour
{
    private Animator anim;
    private readonly int DyingAnimatorHash = Animator.StringToHash("Dying");
    
    public void OnInteract()
    {
        Debug.Log("NPC dead");
        anim.SetBool(DyingAnimatorHash, true);
    }
}
