using System.Collections;
using UnityEngine;
using TMPro;

public class TypewriterEffect : MonoBehaviour
{
    public TextMeshProUGUI textUI;
    [Header("Base Speed")]
    [Tooltip("Temps moyen entre chaque lettre")]
    public float baseDelay = 0.01f;

    [Header("Variations humaines")]
    [Tooltip("Variation aléatoire autour du délai (ex : 0.5 = ±50%)")]
    public float randomFactor = 0.2f;

    [Header("Pauses naturelles")]
    [Tooltip("Pause supplémentaire après un espace")]
    public float spacePause = 0.02f;
    [Tooltip("Pause supplémentaire après .,!?;:")]
    public float punctuationPause = 0.12f;

    // [Header("SFX")]

    private AudioSource audioSource;

    private string originalText;
    private Coroutine typingCoroutine;

    void Awake()
    {
        // On récupère directement le texte déjà présent dans le component
        if (textUI == null)
            textUI = GetComponent<TextMeshProUGUI>();

        originalText = textUI.text;
        audioSource = GetComponent<AudioSource>();

    }

    void OnEnable()
    {

    }

    public void Play(string text)
    {
        Play(text, 0);
    }

    public void Play(string text, int startFromIndex)
    {
        originalText = text;
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeCoroutine(startFromIndex));
        if (audioSource != null)
        {
            audioSource.loop = true;
            audioSource.Play();
        }

    }
    public void Stop()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);
        typingCoroutine = null;
        if (audioSource != null)
        {
            audioSource.Stop();
        }
    }

    public void Reset()
    {
        textUI.text = "";
    }

    IEnumerator TypeCoroutine(int startFromIndex = 0)
    {
        // Set the text up to startFromIndex immediately (no animation for this part)
        if (startFromIndex > 0 && startFromIndex < originalText.Length)
        {
            textUI.text = originalText.Substring(0, startFromIndex);
        }
        else
        {
            textUI.text = "";
        }

        // Animate from startFromIndex to the end
        for (int i = startFromIndex; i < originalText.Length; i++)
        {
            char c = originalText[i];
            textUI.text += c;

            float delay = baseDelay;

            // Variation humaine (ex : 30ms ± 50 %)
            float jitter = Random.Range(-baseDelay * randomFactor, baseDelay * randomFactor);
            delay += jitter;

            // Pause après les espaces
            if (c == ' ')
                delay += spacePause;

            // Pause après ponctuation
            if (".,!?:;".Contains(c))
                delay += punctuationPause;

            // On s'assure d'avoir un délai positif
            if (delay < 0.001f)
                delay = 0.001f;

            yield return new WaitForSeconds(delay);
        }
        if (audioSource != null)
        {
            audioSource.Stop();
        }
    }
}
