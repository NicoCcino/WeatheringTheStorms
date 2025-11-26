using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
public class UIGauge : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI modifierText;
    [SerializeField] private Image outlineImage;
    [SerializeField] private Color hitColor;
    [SerializeField] private Color healColor;
    [SerializeField] private Slider sliderDelta;
    [SerializeField] private Image sliderDeltaImage;


    private Vector3 startOutlineImageScale;
    private Color startOutlineImageColor;
    private void OnEnable()
    {
        startOutlineImageScale = outlineImage.transform.localScale;
        startOutlineImageColor = outlineImage.color;
    }
    public void UpdateDeltaGauge()
    {
        sliderDelta.value = slider.value;
    }
    public void UpdateModifier(float value)
    {
        string text = (value * 10).ToString("0");
        if (value > 0)
        {

            modifierText.color = Color.green;   // positif
            modifierText.text = "+" + text;
        }
        else if (value < 0)
        {
            modifierText.color = Color.red;     // négatif 
            modifierText.text = text;
        }

        else
        {
            modifierText.color = Color.white;   // neutre
            modifierText.text = "+" + text;
        }

    }

    public void UpdateGauge(float newValue)
    {
        float startValue = slider.value;
        float targetValue = newValue;
        float currentValue = startValue;
        Tween sliderValuetween = DOTween.To(() => startValue, x => currentValue = x, targetValue, 0.25f)
                                .OnUpdate(() =>
                                {
                                    slider.value = currentValue;
                                }
                                );

        Color targetColor = targetValue < startValue ? hitColor : healColor;
        Color currentColor = startOutlineImageColor;

        Tween colorTween = DOTween.To(() => startOutlineImageColor, x => currentColor = x, targetColor, 0.12f).OnUpdate(() =>
        {
            outlineImage.color = currentColor;
        });
        sliderDeltaImage.color = targetColor;

        SetDeltaTargetValue(newValue);



        colorTween.SetLoops(2, LoopType.Yoyo);

        Vector3 targetScale = new Vector3(startOutlineImageScale.x * 1.05f, startOutlineImageScale.y * 1.3f, startOutlineImageScale.z);
        Vector3 currentScale = startOutlineImageScale;

        Tween scaleTween = DOTween.To(() => startOutlineImageScale, x => currentScale = x, targetScale, 0.12f).OnUpdate(() =>
        {
            outlineImage.transform.localScale = currentScale;
        });
        scaleTween.SetLoops(2, LoopType.Yoyo);
    }


    private float targetVal;

    /// <summary>
    /// Sets the new target value and initiates the delayed, smooth animation.
    /// </summary>
    public void SetDeltaTargetValue(float newTargetValue)
    {
        // Only proceed if the value has actually changed.
        if (targetVal == newTargetValue) return;

        targetVal = newTargetValue;

        // 1. Kill any existing DOTween animation on this slider to reset the process.
        // This stops the previous transition immediately if a new value comes in.
        sliderDelta.DOKill(true);

        // 2. Start the new animation:
        sliderDelta.DOValue(targetVal, 0.25f) // Tween the slider.value to targetValue
              .SetDelay(0.5f)                  // Wait for the specified delay (0.4s)
              .SetEase(Ease.InOutSine);                       // Apply the smooth easing (e.g., OutQuad)
    }
}

