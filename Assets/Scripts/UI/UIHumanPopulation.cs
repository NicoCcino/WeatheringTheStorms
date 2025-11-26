using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class UIHumanPopulation : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textHumanPopulation;
    private Color startTextColor;
    public Color hitColor;
    public Color healColor;
    private long previousHumanCount;
    private void OnEnable()
    {
        startTextColor = textHumanPopulation.color;
        Human.Instance.OnHumanCountChanged += OnHumanCountChangedCallback;
    }
    private void OnDisable()
    {
        Human.Instance.OnHumanCountChanged -= OnHumanCountChangedCallback;
    }

    private void OnHumanCountChangedCallback(uint tick, float humanImpact)
    {
        PlayAnimation(Human.Instance.HumanCount);
        previousHumanCount = Human.Instance.HumanCount;
    }

    private void Update()
    {
        textHumanPopulation.text = Human.Instance.HumanCount.ToString("###\\.###\\.###\\.###");
    }
    private void PlayAnimation(long count)
    {

        Color targetColor = count < previousHumanCount ? hitColor : healColor;
        Color currentColor = startTextColor;

        Tween colorTween = DOTween.To(() => startTextColor, x => currentColor = x, targetColor, 0.12f).OnUpdate(() =>
        {
            textHumanPopulation.color = currentColor;
        });
        colorTween.SetLoops(2, LoopType.Yoyo);
    }
}
