using UnityEngine;
using System.Collections;

public static class AudioFade
{
    public static IEnumerator FadeOut(AudioSource audioSource, float duration, AnimationCurve curve)
    {
        float startVolume = audioSource.volume;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / duration);
            audioSource.volume = startVolume * (1f - curve.Evaluate(t));
            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = startVolume; // reset pour réutiliser
    }

    public static IEnumerator FadeIn(AudioSource audioSource, float duration, AnimationCurve curve)
    {
        float targetVolume = audioSource.volume;

        audioSource.volume = 0f;
        audioSource.Play();

        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / duration);

            audioSource.volume = targetVolume * curve.Evaluate(t);

            yield return null;
        }

        audioSource.volume = targetVolume; // reset propre
    }

    public static IEnumerator SwitchMusic(AudioSource currentSource, AudioSource nextSource, float duration, AnimationCurve curve)
    {
        // Lancer fade-out et fade-in en séquence
        if (currentSource != null)
        {
            yield return FadeOut(currentSource, duration, curve);
        }

        if (nextSource != null)
        {
            yield return FadeIn(nextSource, duration, curve);
        }
    }

}