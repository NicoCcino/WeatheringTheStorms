using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class Loading : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI progressText;
    public float updateTextDelay = 0.3f;
    public float minimumLoadTime = 2f;

    public string sceneToLoad;

    private string[] dots = new string[] { ".", "..", "..." };
    private int dotIndex = 0;

    void Start()
    {
        StartCoroutine(LoadAsync());
        StartCoroutine(UpdateLoadingText());
    }

    IEnumerator LoadAsync()
    {
        float timer = 0f;

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneToLoad);
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);

            // Optionnel : Afficher le pourcentage (à côté des points)
            // progressText.text = $"{Mathf.RoundToInt(progress * 100f)}% {dots[dotIndex]}";
            if (progressText != null)
                progressText.text = $"{dots[dotIndex]}";

            timer += Time.deltaTime;

            // Quand tout est prêt ET le délai minimum est passé
            if (operation.progress >= 0.9f && timer >= minimumLoadTime)
            {
                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }

    private IEnumerator UpdateLoadingText()
    {
        while (true)
        {
            dotIndex = (dotIndex + 1) % dots.Length;
            yield return new WaitForSeconds(updateTextDelay);
        }
    }
}
