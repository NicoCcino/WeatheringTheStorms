using UnityEngine;
using UnityEngine.EventSystems;
public abstract class GridClickable<T> : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler where T : IGridObject
{
    public T LinkedGridObject;

    public void Init(T linkedGridObject)
    {
        this.LinkedGridObject = linkedGridObject;
    }

    protected abstract void OnClick();
    public void OnPointerClick(PointerEventData eventData)
    {
        GridManager.Instance.RemoveObjectAtCoordinates(LinkedGridObject.Coordinates);
        OnPointerExit(eventData);
        OnClick();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ClickableHelper.PointerEnterClickable(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ClickableHelper.PointerExitClickable(this);
    }
}
