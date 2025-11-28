using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    [SerializeField] private AudioSource audioSource;
    public void PlaySound(AudioClip audioClip, float volume)
    {
        if (audioClip != null && audioSource != null)
        {
            audioSource.PlayOneShot(audioClip, volume);
        }
    }
}
