using UnityEngine;

/// <summary>
/// Represents a single message in a conversation history
/// </summary>
[System.Serializable]
public class ChatMessage
{
    public enum Alignment
    {
        Left,   // For prompts (Label + Description)
        Right   // For user choices ("You" + Choice)
    }

    public string SenderName;      // The label (e.g., "P.Wright") or "You"
    public string MessageText;     // The description or the choice text
    public Alignment MessageAlignment;

    public ChatMessage(string senderName, string messageText, Alignment alignment)
    {
        SenderName = senderName;
        MessageText = messageText;
        MessageAlignment = alignment;
    }
}

