using System;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(10)]
public class UITrustGauge : MonoBehaviour
{
    [SerializeField] private RectTransform cursor;
    [SerializeField] private RectTransform[] levels;
    [SerializeField] private float SliderValue;
    [SerializeField] private float CursorYOffset;
    public Image imageTimer;
    private float timeUntilNextSpawn;
    float timer = 0.0f;
    private void OnEnable()
    {
        GaugeManager.Instance.TrustGauge.OnGaugeModified += OnGaugeModifierCallback;
        ComputePower.Instance.OnComputePowerLootSpawn += OnComputePowerLootSpawn;
        OnComputePowerLootSpawn(null);
    }
    private void OnDisable()
    {
        GaugeManager.Instance.TrustGauge.OnGaugeModified -= OnGaugeModifierCallback;
        ComputePower.Instance.OnComputePowerLootSpawn -= OnComputePowerLootSpawn;
    }
    private void OnComputePowerLootSpawn(ComputePowerLootData data)
    {
        timeUntilNextSpawn = (uint)(1 / ComputePower.Instance.spawnFrequency) + 1;
        timer = 0.0f;
    }
    private void SetGaugeValue(float value)
    {
        float max = GaugeManager.Instance.TrustGauge.gaugeParameter.Max;
        float min = GaugeManager.Instance.TrustGauge.gaugeParameter.Min;
        float range = max - min;
        float proportionalValue = (value - min) / range;
        float index = proportionalValue * levels.Length;
        RectTransform targetTransform = levels[Mathf.FloorToInt(index)];

        cursor.position = targetTransform.position + new Vector3(0, CursorYOffset, 0);
    }
    private void OnGaugeModifierCallback(float value)
    {
        SliderValue = value;
    }
    private void Update()
    {
        SetGaugeValue(GaugeManager.Instance.TrustGauge.value);

        timer += (Time.deltaTime * Timeline.Instance.tickFreq);
        imageTimer.fillAmount = timer / timeUntilNextSpawn;
    }
}
