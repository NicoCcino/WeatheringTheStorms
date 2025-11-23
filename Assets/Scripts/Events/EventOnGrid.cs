using UnityEngine;
using UnityEngine.EventSystems;

public class EventOnGrid : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private UIHoverInformation uIHoverInformation;
    public void Init(Event ev)
    {
        sprite.sprite = ev.EventData.Icon;
        uIHoverInformation.DisplayedText = ev.EventData.Description;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
    }

    public void OnPointerExit(PointerEventData eventData)
    {
    }
}
