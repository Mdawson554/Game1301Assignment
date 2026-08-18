using UnityEngine;
 
public class SFX_AnimationEvent : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip clip;
 
    public void PlaySFX()
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
 
}