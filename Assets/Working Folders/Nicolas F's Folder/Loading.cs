using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using WiDiD.UI;

public class Loading : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI progressText;
    public float updateTextDelay = 0.3f;
    public float minimumLoadTime = 2f;
    public CanvasGroupCustom canvasGroupCustom;
    public TextMeshProUGUI textDescription;

    public string sceneToLoad;

    private string[] dots = new string[] { ".", "..", "..." };
    private int dotIndex = 0;
    // Async load variables
    private AsyncOperation asyncLoad;
    private Coroutine textUpdateCoroutine;
    private float timer = 0f;

    void Start()
    {
        textUpdateCoroutine = StartCoroutine(UpdateLoadingText());
        StartCoroutine(LoadAsync());

    }

    public void DisplayCreatorPrompt()
    {
        canvasGroupCustom.Fade(true);
        var typewriterEffect = textDescription.GetComponent<TypewriterEffect>();
        Debug.Log("Playing typewriter effet with this text: " + textDescription.text);
        typewriterEffect.Play(textDescription.text);
    }

    public void StartGame()
    {
        if (asyncLoad.progress >= 0.9f && timer >= minimumLoadTime)
        {
            Debug.Log("Game will now start.");
            asyncLoad.allowSceneActivation = true;
        }
    }
    IEnumerator LoadAsync()
    {
        timer = 0f;

        bool creatorPromptDisplayed = false;

        asyncLoad = SceneManager.LoadSceneAsync(sceneToLoad);
        asyncLoad.allowSceneActivation = false;

        while (!asyncLoad.isDone)
        {
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);

            // Optionnel : Afficher le pourcentage (à côté des points)
            // progressText.text = $"{Mathf.RoundToInt(progress * 100f)}% {dots[dotIndex]}";
            if (progressText != null)
                progressText.text = $"{dots[dotIndex]}";

            timer += Time.deltaTime;

            // Quand le délai minimum est passé
            if (timer >= minimumLoadTime && !creatorPromptDisplayed)
            {
                Debug.Log("Minimum time OK. Now displaying creator prompt and stopping text update coroutine.");
                creatorPromptDisplayed = true;
                DisplayCreatorPrompt();
                StopCoroutine(textUpdateCoroutine);
            }

            // Quand tout est prêt ET le délai minimum est passé
            if (asyncLoad.progress >= 0.9f && timer >= minimumLoadTime)
            {
                // Debug.Log("Game loaded and ready to start.");
                StopCoroutine(UpdateLoadingText());
                // operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }

    private IEnumerator UpdateLoadingText()
    {
        while (true)
        {
            if (!canvasGroupCustom.IsVisible)
            {
                dotIndex = (dotIndex + 1) % dots.Length;
            }
            yield return new WaitForSeconds(updateTextDelay);
        }
    }
}
