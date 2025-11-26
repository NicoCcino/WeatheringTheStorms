using UnityEngine;

public class MusicController : MonoBehaviour
{
    public AudioSource gameMusic;         // musique actuelle (ex : calme)
    public AudioSource endMusic;         // musique suivante (ex : action)
    public AnimationCurve fadeCurve;   // courbe
    public float fadeDuration = 1.5f;  // durée du fade

    public void SwitchToEndMusic()
    {
        StartCoroutine(AudioFade.SwitchMusic(gameMusic, endMusic, fadeDuration, fadeCurve));
    }

    public void SwitchToGameMusic()
    {
        StartCoroutine(AudioFade.SwitchMusic(endMusic, gameMusic, fadeDuration, fadeCurve));
    }
}