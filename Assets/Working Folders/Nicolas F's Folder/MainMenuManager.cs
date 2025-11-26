using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    // Nom de la scène à charger
    public string loadingSceneName;

    // Appeler cette fonction pour charger la scène
    public void LoadScene()
    {
        if (!string.IsNullOrEmpty(loadingSceneName))
        {
            SceneManager.LoadScene(loadingSceneName);
        }
        else
        {
            Debug.LogWarning("Scene name is empty !");
        }
    }
}