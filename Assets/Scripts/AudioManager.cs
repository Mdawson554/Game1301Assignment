using System.Collections;
using UnityEngine;

namespace _Project.Scripts.Core
{
    public class AudioManager : Singleton<AudioManager>
    {
        public AudioSource sfxAudioSource;

        public void PlaySound(AudioClip clip)
        {
            if (clip == null) return;
            sfxAudioSource.PlayOneShot(clip);
        }
    }
}