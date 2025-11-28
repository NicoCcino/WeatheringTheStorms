using UnityEngine;
using TMPro;
using WiDiD.UI;

public class UIPromptEnd : MonoBehaviour
{
    public TextMeshProUGUI winText;
    public TextMeshProUGUI lossText;
    public CanvasGroupCustom[] canvasGroupCustomsToHide;
    public SpriteRenderer worldMapSpriteRenderer;

    public GameObject clickableSpawner;

    public void DisplayEndPanel(TextMeshProUGUI text)
    {
        // Pause the game
        Timeline.Instance.SetPauseSpeed(false);
        // Display panel and trigger typewriter effect.
        GetComponent<CanvasGroupCustom>().Fade(true);
        var typewriterEffect = text.GetComponent<TypewriterEffect>();
        typewriterEffect.Play(text.text);
        // Hide Unecessary UI
        foreach (var canvasGroupCustom in canvasGroupCustomsToHide)
        {
            canvasGroupCustom.Fade(false);
        }
        // Hide World Map
        worldMapSpriteRenderer.gameObject.SetActive(false);
        // Deactivate Spawner
        clickableSpawner.SetActive(false);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
