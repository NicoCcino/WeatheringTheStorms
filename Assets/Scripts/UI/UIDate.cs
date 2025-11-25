using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIDate : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textDate;
    [SerializeField] private Slider slider;

    private void OnEnable()
    {
        slider.value = 0;
        Timeline.Instance.OnTick += OnTickCallback;
    }
    private void OnDisable()
    {
        Timeline.Instance.OnTick -= OnTickCallback;
    }
    private void OnTickCallback(uint tick)
    {
        slider.value = Timeline.Instance.CurrentTick / (float)Timeline.Instance.TimeLineParameter.YearsWinCondition;
    }

    private void Update()
    {
        textDate.text = Timeline.Instance.currentDate.ToString("yyyy/MM");
        slider.value += (Time.deltaTime * Timeline.Instance.tickFreq) / (float)Timeline.Instance.TimeLineParameter.YearsWinCondition;
    }
}
