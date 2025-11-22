using UnityEngine;

[System.Serializable]
public class EventData : IGridObject
{
    [field: Header("Displayed Informations")]
    [field: SerializeField, Tooltip("Event description"), TextArea(1, 4)] public string Description { get; private set; }

    [field: Header("Modifiers")]
    [field: SerializeField] public ModifierBank ModifierBank { get; private set; }

    [field: Header("Grid Position")]
    [field: SerializeField] public Vector2Int Coordinates { get; set; }

    [field: Header("Conditions")]
    [field: SerializeField] public DateCondition DateCondition { get; private set; }
    [field: SerializeField] public GaugeCondition GaugeCondition { get; private set; }

    [field: SerializeField, Tooltip("Optional: This event can only trigger if the parent event has been triggered before")]
    public Event ParentEvent { get; private set; }

}

