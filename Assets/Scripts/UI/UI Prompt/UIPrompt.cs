using UnityEngine;
using TMPro;
using System;
using WiDiD.UI;

public class UIPrompt : MonoBehaviour
{

    public CanvasGroupCustom canvasGroupCustom;
    public TextMeshProUGUI textHeader;
    public TextMeshProUGUI textDescription;
    public UIChoicesManager uiChoicesManager;


    [Header("Field for debug purposes only")]
    public Prompt DisplayedPrompt = null;
    public void DisplayPrompt(Prompt prompt)
    {
        canvasGroupCustom.Fade(true);
        textDescription.text = prompt.PromptData.Description;
        textDescription.GetComponent<TypewriterEffect>().Play();
        textHeader.text = prompt.PromptData.Label;
        uiChoicesManager.SpawnChoices(prompt.PromptData.Choices, this);
        DisplayedPrompt = prompt;
    }
    public void HideDisplayedPrompt()
    {
        canvasGroupCustom.Fade(false);

        if (DisplayedPrompt == null) return;
        DisplayedPrompt = null;
    }
    public void SolveDisplayedPrompt(Choice choice)
    {
        if (DisplayedPrompt == null) return;
        DisplayedPrompt.Solve(choice);
    }
    private void OnEnable()
    {
        PromptManager.Instance.OnPromptOpened += OnPromptOpenedCallback;
    }
    private void OnDisable()
    {
        PromptManager.Instance.OnPromptOpened -= OnPromptOpenedCallback;
    }
    private void OnPromptOpenedCallback(Prompt prompt)
    {
        DisplayPrompt(prompt);
        DisplayedPrompt.OnSolved += OnDisplayedPromptSolvedCallback;
    }
    private void OnDisplayedPromptSolvedCallback(Choice choice)
    {
        DisplayedPrompt.OnSolved -= OnDisplayedPromptSolvedCallback;
        HideDisplayedPrompt();
    }

    //DEBUG BLOCK
    private Prompt previousPrompt = null;
    private void Update()
    {
        if (DisplayedPrompt != previousPrompt)
        {
            if (DisplayedPrompt != null)
                DisplayPrompt(DisplayedPrompt);
            else
                HideDisplayedPrompt();

            previousPrompt = DisplayedPrompt;
        }
    }
    //
}

