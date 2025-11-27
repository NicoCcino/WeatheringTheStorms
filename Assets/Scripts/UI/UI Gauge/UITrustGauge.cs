using System;
using UnityEngine;

public class UITrustGauge : MonoBehaviour
{
    [SerializeField] private RectTransform cursor;
    [SerializeField] private RectTransform[] levels;
    [SerializeField] private float SliderValue;
    [SerializeField] private float CursorYOffset;

    private void OnEnable()
    {
        GaugeManager.Instance.TrustGauge.OnGaugeModified += OnGaugeModifierCallback;
    }
    private void OnDisable()
    {
        GaugeManager.Instance.TrustGauge.OnGaugeModified -= OnGaugeModifierCallback;
    }
    private void SetGaugeValue(float value)
    {
        float max = GaugeManager.Instance.TrustGauge.gaugeParameter.Max;
        float min = GaugeManager.Instance.TrustGauge.gaugeParameter.Min;
        float range = max - min;
        float proportionalValue = (value - min) / range;
        float index = proportionalValue * levels.Length;
        Debug.Log(index);
        RectTransform targetTransform = levels[Mathf.FloorToInt(index)];

        cursor.position = targetTransform.position + new Vector3(0, CursorYOffset, 0);
    }
    private void OnGaugeModifierCallback(float value)
    {
        Debug.Log("Trust gauge value " + value);
        SliderValue = value;
    }
    private void Update()
    {
        SetGaugeValue(SliderValue);
    }
}
