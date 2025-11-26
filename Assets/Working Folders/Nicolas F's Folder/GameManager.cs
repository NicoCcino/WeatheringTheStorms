using UnityEngine;
using WiDiD.UI;
using TMPro;

public class GameManager : MonoBehaviour
{

    public float winConditionClimate;
    public float winConditionSocietal;
    public TextMeshProUGUI winText;
    public TextMeshProUGUI lossText;
    public MusicController musicController;
    public UIPromptEnd uiPromptEnd;

    private void OnTimelineTick(uint currentTick)
    {
        // Check if player lost
        if (Human.Instance.HumanCount <= 0)
        {
            winText.gameObject.SetActive(false);
            lossText.gameObject.SetActive(true);
            Debug.Log("You lost!");
            uiPromptEnd.DisplayEndPanel(lossText);
            musicController.SwitchToLossMusic(); // Switch music
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
            uiPromptEnd.DisplayEndPanel(winText);
            musicController.SwitchToWinMusic(); // Switch music
        }
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
