using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class UIConsole : MonoBehaviour
{
    public GameObject consoleContent;
    public GameObject eventLogPrefab;
    public ScrollRect scrollRect;

    public Color positiveColor;
    public Color negativeColor;
    private void OnEnable()
    {
        EventManager.Instance.OnEventTriggered += HandleEvent;
    }

    private void OnDisable()
    {
        EventManager.Instance.OnEventTriggered -= HandleEvent;
    }

    // Cette fonction est appelée automatiquement quand l’événement est déclenché
    private void HandleEvent(Event triggeredEvent)
    {
        Debug.Log(triggeredEvent.ToString() + " a été observé !");
        // Spawn new evenLog
        GameObject eventLog = SimplePool.Spawn(eventLogPrefab);
        // Set parent
        eventLog.transform.SetParent(consoleContent.transform);
        // Update text
        eventLog.transform.localScale = Vector3.one;
        string newLogText = Timeline.Instance.currentDate.ToString("yyyy/MM") + ": ";
        newLogText += triggeredEvent.EventData.Description;
        eventLog.GetComponentInChildren<TextMeshProUGUI>().text = newLogText;
        eventLog.GetComponentInChildren<Image>().color = triggeredEvent.IsEventPositive() ? positiveColor : negativeColor;
        // Scroll down
        StartCoroutine(ScrollToTop());
    }

    private IEnumerator ScrollToTop()
    {
        yield return null; // attendre la fin du frame (Layout Group)
        scrollRect.verticalNormalizedPosition = 1f;
    }

}
