using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class EventOnGrid : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private UIHoverInformation uIHoverInformation;
    [SerializeField] private Renderer highlightedRenderer;

    [SerializeField] private Color positiveColor;
    [SerializeField] private Color negativeColor;
    private Event ev;
    private Color startColor;


    //Duration purposes
    uint startTick = 0;
    uint remainingDuration = 0;
    public void Init(Event ev)
    {
        sprite.sprite = ev.EventData.Icon;
        uIHoverInformation.DisplayedText = ev.EventData.Description;
        this.ev = ev;
        ev.OnEventHoveredEnter += OnEventHoveredEnter;
        ev.OnEventHoveredEnter += OnEventHoveredExit;
        startTick = Timeline.Instance.CurrentTick;
        highlightedRenderer.material.SetColor("_Color", ev.IsEventPositive() ? positiveColor : negativeColor);
        startColor = highlightedRenderer.material.GetColor("_Color");
        if (ev.EventData.Duration > 0)
        {
            Timeline.Instance.OnTick += OnTimelineTickCallback;
        }
    }

    private void OnTimelineTickCallback(uint currentTick)
    {
        if (ev == null) return;

        remainingDuration = (uint)ev.EventData.Duration - (currentTick - startTick);
        if (remainingDuration <= 0)
        {
            SimplePool.Despawn(gameObject);
        }
    }

    private void OnEventHoveredExit(Event ev)
    {

        highlightedRenderer.material.SetColor("_Color", startColor);
    }

    private void OnEventHoveredEnter(Event ev)
    {
        highlightedRenderer.material.SetColor("_Color", startColor * 10);
    }

    private void OnDisable()
    {
        ev.OnEventHoveredEnter -= OnEventHoveredEnter;
        ev.OnEventHoveredEnter -= OnEventHoveredExit;
        Timeline.Instance.OnTick -= OnTimelineTickCallback;
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
