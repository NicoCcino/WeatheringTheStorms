using UnityEngine;
using WiDiD.UI;
using TMPro;

public class GameManager : MonoBehaviour
{

    public float winConditionClimate;
    public float winConditionSocietal;
    public GameObject panelPromptEnd;
    public TextMeshProUGUI winText;
    public TextMeshProUGUI lossText;
    public GameObject[] gameObjectsToHide;
    public MusicController musicController;

    private void OnTimelineTick(uint currentTick)
    {
        // Check if player lost
        if (Human.Instance.HumanCount <= 0)
        {
            winText.gameObject.SetActive(false);
            lossText.gameObject.SetActive(true);
            Debug.Log("You lost!");
            DisplayEndPanel(lossText);
        }
    }

    private void OnGaugeChanged(uint currentTick)
    {
        // Check if player won
        if (GaugeManager.Instance.ClimateGauge.value >= winConditionClimate && GaugeManager.Instance.SocietalGauge.value >= winConditionSocietal)
        {
            winText.gameObject.SetActive(true);
            lossText.gameObject.SetActive(false);
            Debug.Log("You won!");
            DisplayEndPanel(winText);
        }
    }

    private void DisplayEndPanel(TextMeshProUGUI text)
    {
        // Pause the game
        Timeline.Instance.SetPauseSpeed();
        // Display panel and trigger typewriter effect.
        panelPromptEnd.GetComponent<CanvasGroupCustom>().Fade(true);
        var typewriterEffect = text.GetComponent<TypewriterEffect>();
        typewriterEffect.Play(text.text);
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
            Timeline.Instance.OnTick += OnTimelineTick;
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
