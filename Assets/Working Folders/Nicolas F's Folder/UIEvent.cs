using UnityEngine;
using TMPro;

public class UIEvent : MonoBehaviour
{

    public TextMeshProUGUI textDescription;
    public TextMeshProUGUI textHeader;

    public void DisplayEvent(Event ev)
    {
        textDescription.text = ev.EventData.Description;
        textHeader.text = ev.EventData.Label;
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
