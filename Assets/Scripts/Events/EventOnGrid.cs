using UnityEngine;
using UnityEngine.EventSystems;

public class EventOnGrid : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private UIHoverInformation uIHoverInformation;
    public void Init(Event ev)
    {
        uIHoverInformation.DisplayedText = ev.EventData.Description;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
    }

    public void OnPointerExit(PointerEventData eventData)
    {
    }
}
