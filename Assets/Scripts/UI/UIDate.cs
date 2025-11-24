using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIDate : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textDate;
    [SerializeField] private Slider slider;

    private void Update()
    {
        textDate.text = Timeline.Instance.currentDate.ToString("yyyy/MM");
        slider.value = Timeline.Instance.CurrentTick / (float)Timeline.Instance.TimeLineParameter.YearsWinCondition;
    }
}
