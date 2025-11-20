using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIChoice : MonoBehaviour
{
    public Button button;
    public TextMeshProUGUI textLabel;
    public UIGaugeModifierManager uiGaugeModifierManager;
    public Choice displayedChoice;
    public UIEvent parentUIEvent;
    private void OnEnable()
    {
        button.onClick.AddListener(OnButtonClickCallback);
    }
    private void OnDisable()
    {
        button.onClick.RemoveListener(OnButtonClickCallback);
    }
    private void OnButtonClickCallback()
    {
        if (parentUIEvent == null || displayedChoice == null) return;
        parentUIEvent.SolveDisplayedEvent(displayedChoice);
    }

    public void UpdateDisplay(Choice choice, UIEvent parentUIEvent)
    {
        textLabel.text = choice.Label;
        uiGaugeModifierManager.DisplayModifierBank(choice.ModifierBank);
        displayedChoice = choice;
        this.parentUIEvent = parentUIEvent;
    }

}
