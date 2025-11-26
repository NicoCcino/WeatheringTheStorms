using UnityEngine;
using WiDiD.UI;
using TMPro;

public class GameManager : MonoBehaviour
{

    public float winConditionClimate;
    public float winConditionSocietal;
    public GameObject panelPromptEnd;
    public TextMeshProUGUI endText;
    public GameObject[] gameObjectsToHide;
    public MusicController musicController;

    private void OnGaugeChanged(uint currentTick)
    {
        // Check if player won
        if (GaugeManager.Instance.ClimateGauge.value >= winConditionClimate && GaugeManager.Instance.SocietalGauge.value >= winConditionSocietal)
        {
            Debug.Log("You won!");
            DisplayEndPanel();
        }
    }

    private void DisplayEndPanel()
    {
        // Pause the game
        Timeline.Instance.SetPauseSpeed();
        // Display panel and trigger typewriter effect.
        panelPromptEnd.GetComponent<CanvasGroupCustom>().Fade(true);
        var typewriterEffect = endText.GetComponent<TypewriterEffect>();
        typewriterEffect.Play(endText.text);
        // Hide all other panels
        foreach (GameObject go in gameObjectsToHide)
        {
            go.SetActive(false);
        }
        // Switch music
        musicController.SwitchToEndMusic();

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Subscribe to gauge change events
        if (Timeline.Instance != null)
        {
            GaugeManager.Instance.OnGaugeChanged += OnGaugeChanged;
        }
        else
        {
            Debug.LogError("LogFileManager: Timeline instance not found!");
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
