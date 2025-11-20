using UnityEngine;
using TMPro;
using System;
using WiDiD.UI;

public class UIEvent : MonoBehaviour
{

    public CanvasGroupCustom canvasGroupCustom;
    public TextMeshProUGUI textHeader;
    public TextMeshProUGUI textDescription;
    public UIChoicesManager uiChoicesManager;


    [Header("Field for debug purposes only")]
    public Event DisplayedEvent = null;
    public void DisplayEvent(Event ev)
    {
        textDescription.text = ev.EventData.Description;
        textHeader.text = ev.EventData.Label;
        uiChoicesManager.SpawnChoices(ev.EventData.Choices, this);
        DisplayedEvent = ev;
    }
    public void HideDisplayedEvent()
    {
        canvasGroupCustom.Fade(false);

        if (DisplayedEvent == null) return;
        DisplayedEvent = null;
    }
    public void SolveDisplayedEvent(Choice choice)
    {
        if (DisplayedEvent == null) return;
        DisplayedEvent.Solve(choice);
    }
    private void OnEnable()
    {
        EventManager.Instance.OnEventTriggered += OnEventTriggeredCallback;
    }
    private void OnDisable()
    {
        EventManager.Instance.OnEventTriggered -= OnEventTriggeredCallback;
    }
    private void OnEventTriggeredCallback(Event ev)
    {
        DisplayEvent(ev);
        DisplayedEvent.OnSolved += OnDisplayedEventSolvedCallback;
    }
    private void OnDisplayedEventSolvedCallback(Choice choice)
    {
        DisplayedEvent.OnSolved -= OnDisplayedEventSolvedCallback;
        HideDisplayedEvent();
    }

    //DEBUG BLOCK
    private Event previousEvent = null;
    private void Update()
    {
        if (DisplayedEvent != previousEvent)
        {
            if (DisplayedEvent != null)
                DisplayEvent(DisplayedEvent);
            else
                HideDisplayedEvent();

            previousEvent = DisplayedEvent;
        }
    }
    //
}
