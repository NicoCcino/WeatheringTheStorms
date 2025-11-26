using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class EventOnGrid : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private UIHoverInformation uIHoverInformation;
    [SerializeField] private Renderer highlightedRenderer;
    private Event ev;
    private Color startColor;
    public void Init(Event ev)
    {
        sprite.sprite = ev.EventData.Icon;
        uIHoverInformation.DisplayedText = ev.EventData.Description;
        this.ev = ev;
        ev.OnEventHoveredEnter += OnEventHoveredEnter;
        ev.OnEventHoveredEnter += OnEventHoveredExit;
        startColor = highlightedRenderer.material.GetColor("_Color");
    }

    private void OnEventHoveredExit(Event ev)
    {

        highlightedRenderer.material.SetColor("_Color", startColor);
    }

    private void OnEventHoveredEnter(Event ev)
    {
        highlightedRenderer.material.SetColor("_Color", startColor * 2);
    }

    private void OnDisable()
    {
        ev.OnEventHoveredEnter -= OnEventHoveredEnter;
        ev.OnEventHoveredEnter -= OnEventHoveredExit;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (ev == null)
            return;

        ev.OnEventHoveredEnter?.Invoke(ev);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ev.OnEventHoveredExit?.Invoke(ev);
    }
}
