using UnityEngine;

[System.Serializable]
public class PromptData : IGridObject
{
    public enum PromptCategory
    {
        Climate = 0,
        Societal = 1
    }

    [field: Header("Displayed Informations")]
    [field: SerializeField, Tooltip("Will be displayed in the header of the prompt")] public string Label { get; private set; }
    [field: SerializeField, Tooltip("Will be displayed in main corpus of the prompt"), TextArea(1, 4)] public string Description { get; private set; }
    [field: SerializeField] public Choice[] Choices { get; private set; }

    [field: Header("Grid Position")]
    [field: SerializeField] public Vector2Int Coordinates { get; set; }

    [field: Header("Conditions")]
    [field: SerializeField] public DateCondition DateCondition { get; private set; }
    [field: SerializeField] public GaugeCondition GaugeCondition { get; private set; }

    [field: SerializeField, Tooltip("Optional: This prompt can only trigger if the parent prompt has been triggered before")]
    public Prompt ParentPrompt { get; private set; }

}

