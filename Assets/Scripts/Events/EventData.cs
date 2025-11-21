using UnityEngine;

[System.Serializable]
public class EventData
{
    public enum EventCategory
    {
        Climate = 0,
        Societal = 1,
        Trust = 2
    }
    
    [field: Header("Displayed Informations")]
    [field: SerializeField, Tooltip("Will be displayed in the header of the event")] public string Label { get; private set; }
    [field: SerializeField, Tooltip("Will be displayed in main corpus of the event"), TextArea(1, 4)] public string Description { get; private set; }
    [field: SerializeField] public Choice[] Choices { get; private set; }


    [field: Header("Conditions")]
    [field: SerializeField] public DateCondition DateCondition { get; private set; }
    [field: SerializeField] public GaugeCondition GaugeCondition { get; private set; }

    [field: Header("Event Hierarchy")]
    [field: SerializeField, Tooltip("Optional: This event can only trigger if the parent event has been triggered before")] 
    public Event ParentEvent { get; private set; }


}
