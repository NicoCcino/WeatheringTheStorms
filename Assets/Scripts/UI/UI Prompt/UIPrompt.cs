using UnityEngine;
using TMPro;
using System;
using System.Collections.Generic;
using WiDiD.UI;
using System.Collections;
using UnityEngine.UI;

public class UIPrompt : MonoBehaviour
{

    public CanvasGroupCustom canvasGroupCustom;
    public TextMeshProUGUI textHeader;
    public TextMeshProUGUI textDescription;
    public UIChoicesManager uiChoicesManager;
    public ScrollRect scrollRect;


    [Header("Field for debug purposes only")]
    public Prompt DisplayedPrompt = null;
    public void DisplayPrompt(Prompt prompt)
    {
        canvasGroupCustom.Fade(true);

        // Get conversation history
        List<ChatMessage> conversationHistory = PromptManager.Instance.GetConversationHistory(prompt);

        // Build the history text (if any) and current description separately
        string historyText = BuildHistoryText(conversationHistory);
        historyText = historyText + "<align=left><b>" + prompt.PromptData.Label + ":</b>\n";
        string currentDescription = prompt.PromptData.Description + "</align>";
        string fullText = historyText + currentDescription;

        var typewriterEffect = textDescription.GetComponent<TypewriterEffect>();

        if (string.IsNullOrEmpty(historyText))
        {
            // No history - just play typewriter on current description
            typewriterEffect.Play(fullText);
        }
        else
        {
            // Play typewriter starting from after the history (history appears instantly, only current animates)
            typewriterEffect.Play(fullText, historyText.Length);
        }

        textHeader.text = prompt.PromptData.Label + " chat :";
        uiChoicesManager.SpawnChoices(prompt.PromptData.Choices, this);
        DisplayedPrompt = prompt;
        StartCoroutine(ScrollToBottom());
    }
    private IEnumerator ScrollToBottom()
    {
        yield return null; // attendre la fin du frame (Layout Group)
        scrollRect.verticalNormalizedPosition = 0f;
    }
    private string BuildHistoryText(List<ChatMessage> history)
    {
        if (history.Count == 0)
        {
            return string.Empty;
        }

        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        // Add history messages
        foreach (ChatMessage message in history)
        {
            if (message.MessageAlignment == ChatMessage.Alignment.Left)
            {
                // Prompt message (left aligned)
                sb.AppendLine($"<align=left><b>{message.SenderName}:</b>\n {message.MessageText}</align>");
            }
            else
            {
                // User choice (right aligned)
                sb.AppendLine($"<align=right><b>{message.SenderName}:</b>\n {message.MessageText}</align>");
            }
            sb.AppendLine(); // Empty line for spacing
        }


        return sb.ToString();
    }
    public void HideDisplayedPrompt()
    {
        canvasGroupCustom.Fade(false);

        if (DisplayedPrompt == null) return;
        DisplayedPrompt = null;

        // Stop typewriter effect
        var typewriterEffect = textDescription.GetComponent<TypewriterEffect>();
        typewriterEffect.Stop();
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
            {
                DisplayPrompt(DisplayedPrompt);
            }
            else
            {
                previousPrompt.OnSolved -= OnDisplayedPromptSolvedCallback;
                HideDisplayedPrompt();
            }
            previousPrompt = DisplayedPrompt;
        }
    }
    //
}

