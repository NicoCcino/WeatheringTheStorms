using UnityEngine;
using System.Collections;

public class LoadingAudioManager : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource audioSource;      // Ton AudioSource à faire fade
    public AnimationCurve fadeCurve;     // Courbe du fade (y=1 à y=0)
    public float fadeDuration = 2f;      // Durée du fade en secondes

    [Header("Optional")]

    public bool autoPlay = true;
    public bool autoStartFade = true;    // Déclenche automatiquement au Start

    void Start()
    {
        if (autoPlay)
        {
            audioSource.Play();
        }
        if (autoStartFade && audioSource != null && fadeCurve != null)
        {
            StartFade();
        }
    }

    /// <summary>
    /// Lance le fade-out sur l'AudioSource
    /// </summary>
    public void StartFade()
    {
        if (audioSource != null && fadeCurve != null)
            StartCoroutine(FadeOutCoroutine());
    }

    private IEnumerator FadeOutCoroutine()
    {
        float startVolume = audioSource.volume;
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / fadeDuration);

            // Applique la courbe au volume
            audioSource.volume = startVolume * fadeCurve.Evaluate(t);

            yield return null;
        }

        // Stop le son à la fin
        audioSource.Stop();
        audioSource.volume = startVolume; // reset si tu veux relancer le son plus tard
    }
}
