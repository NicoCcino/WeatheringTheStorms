using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;
using System;
public class UIComputePower : MonoBehaviour
{

    public TextMeshProUGUI computePowerText;
    public Image imageTimer;

    private Vector3 startTextScale;
    private Vector3 startButtonScale;
    public Button button;
    public UIButtonFX uIButtonFX;

    private float timeUntilNextSpawn;
    float timer = 0.0f;
    void OnEnable()
    {
        startTextScale = computePowerText.transform.localScale;
        startButtonScale = button.transform.localScale;
        // S'abonner à l'événement
        if (ComputePower.Instance != null)
            ComputePower.Instance.OnCP += UpdateComputePowerText;
        // Mettre à jour le texte immédiatement avec la valeur actuelle
        UpdateComputePowerText(ComputePower.Instance.value);
        ComputePower.Instance.OnComputePowerLootSpawn += OnComputePowerLootSpawn;
        OnComputePowerLootSpawn(null);
    }

    private void OnComputePowerLootSpawn(ComputePowerLootData data)
    {
        timeUntilNextSpawn = (uint)(1 / ComputePower.Instance.spawnFrequency) + 1;
        timer = 0.0f;
    }

    void OnDisable()
    {
        // Se désabonner
        if (ComputePower.Instance != null)
            ComputePower.Instance.OnCP -= UpdateComputePowerText;
    }

    void UpdateComputePowerText(int value)
    {
        // Mettre à jour le texte
        computePowerText.text = value.ToString();
        PlayAnimation();
        // Si tu veux afficher un "+" pour les valeurs positives
        // computePowerText.text = (value >= 0 ? "+" : "") + value.ToString();
    }
    void PlayAnimation()
    {
        Vector3 textTargetScale = new Vector3(startTextScale.x * 1.5f, startTextScale.y * 1.5f, startTextScale.z);
        Vector3 textCurrentScale = startTextScale;

        Tween textScaleTween = DOTween.To(() => startTextScale, x => textCurrentScale = x, textTargetScale, 0.10f).OnUpdate(() =>
        {
            computePowerText.transform.localScale = textCurrentScale;
        });
        textScaleTween.SetLoops(2, LoopType.Yoyo);

        Vector3 buttonTargetScale = new Vector3(startButtonScale.x * 1.1f, startButtonScale.y * 1.1f, startButtonScale.z);
        Vector3 buttonCurrentScale = startButtonScale;

        Tween buttonScaleTween = DOTween.To(() => startButtonScale, x => buttonCurrentScale = x, buttonTargetScale, 0.10f).OnUpdate(() =>
        {
            button.transform.localScale = buttonCurrentScale;
        });
        buttonScaleTween.SetLoops(2, LoopType.Yoyo);


        uIButtonFX.PlayAnimation(0.2f);
    }

    private void Update()
    {
        //uint timeSinceLastSpawn = Timeline.Instance.CurrentTick - lastCpSpawnTick;

        timer += (Time.deltaTime * Timeline.Instance.tickFreq);
        imageTimer.fillAmount = timer / timeUntilNextSpawn;
    }
}
