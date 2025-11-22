using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIChoice : MonoBehaviour
{
    public Button button;
    public TextMeshProUGUI textLabel;
    public UIGaugeModifierManager uiGaugeModifierManager;
    public Choice displayedChoice;
    public UIPrompt parentUIPrompt;
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
        if (parentUIPrompt == null || displayedChoice == null) return;
        parentUIPrompt.SolveDisplayedPrompt(displayedChoice);
    }

    public void UpdateDisplay(Choice choice, UIPrompt parentUIPrompt)
    {
        textLabel.text = choice.Label;
        uiGaugeModifierManager.DisplayModifierBank(choice.ModifierBank);
        displayedChoice = choice;
        this.parentUIPrompt = parentUIPrompt;
    }

}
