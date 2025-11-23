using UnityEngine;
using UnityEngine.EventSystems;

public class UIHoverInformation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField, TextArea(1,4)] public string DisplayedText;

    public void OnPointerEnter(PointerEventData eventData)
    {
        UIHoverInformationsManager.Instance.DisplayInformations(this);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        UIHoverInformationsManager.Instance.HideInformations(this);
    }
}
