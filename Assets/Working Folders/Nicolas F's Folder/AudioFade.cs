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
            audioSource.volume = startVolume * curve.Evaluate(t);
            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = startVolume; // reset pour réutiliser
    }
}