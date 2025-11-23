using System.Collections;
using UnityEngine;
using TMPro;

public class TypewriterEffect : MonoBehaviour
{
    public TextMeshProUGUI textUI;
    [Header("Base Speed")]
    [Tooltip("Temps moyen entre chaque lettre")]
    public float baseDelay = 0.03f;

    [Header("Variations humaines")]
    [Tooltip("Variation aléatoire autour du délai (ex : 0.5 = ±50%)")]
    public float randomFactor = 0.5f;

    [Header("Pauses naturelles")]
    [Tooltip("Pause supplémentaire après un espace")]
    public float spacePause = 0.07f;
    [Tooltip("Pause supplémentaire après .,!?;:")]
    public float punctuationPause = 0.2f;

    private string originalText;
    private Coroutine typingCoroutine;

    void Awake()
    {
        // On récupère directement le texte déjà présent dans le component
        if (textUI == null)
            textUI = GetComponent<TextMeshProUGUI>();

        originalText = textUI.text;
    }

    void OnEnable()
    {
        Play();
    }

    public void Play()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeCoroutine());
    }
    public void Reset()
    {
        textUI.text = "";
    }

    IEnumerator TypeCoroutine()
    {
        textUI.text = "";

        foreach (char c in originalText)
        {
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
    }
}