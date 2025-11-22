using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using System.Runtime.CompilerServices;

public class UIConsole : MonoBehaviour
{
    public GameObject consoleContent;
    public GameObject eventLogPrefab;
    public ScrollRect scrollRect;


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

        string newLogText = Timeline.Instance.currentDate.ToString("yyyy/MM") + ": ";
        newLogText += triggeredEvent.EventData.Description;
        eventLog.GetComponent<TextMeshProUGUI>().text = newLogText;
        // Scroll down
        StartCoroutine(ScrollToBottom());
    }

    private IEnumerator ScrollToBottom()
    {
        yield return null; // attendre la fin du frame (Layout Group)
        scrollRect.verticalNormalizedPosition = 0f;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
