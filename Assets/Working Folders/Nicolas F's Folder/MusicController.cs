using UnityEngine;

public class MusicController : MonoBehaviour
{
    public AudioSource gameMusic;         // musique actuelle
    public AudioSource winMusic;         // 
    public AudioSource lossMusic;         // 
    public AnimationCurve fadeCurve;   // courbe
    public float fadeDuration = 1.5f;  // durée du fade


    //public void SwitchToGameMusic()
    //{
    //        StartCoroutine(AudioFade.SwitchMusic(endMusic, gameMusic, fadeDuration, fadeCurve));
    //}

    public void SwitchToWinMusic()
    {
        StartCoroutine(AudioFade.SwitchMusic(gameMusic, winMusic, fadeDuration, fadeCurve));
    }
    public void SwitchToLossMusic()
    {
        StartCoroutine(AudioFade.SwitchMusic(gameMusic, lossMusic, fadeDuration, fadeCurve));
    }
}