using UnityEngine;

public class EventClickable : GridClickable<Event>
{
    protected override void OnClick()
    {
        EventManager.Instance.OpenEvent(LinkedGridObject);
    }
}
