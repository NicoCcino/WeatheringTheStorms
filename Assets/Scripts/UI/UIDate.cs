using TMPro;
using UnityEngine;

public class UIDate : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textDate;

    private void Update()
    {
        textDate.text = Timeline.Instance.currentDate.ToString("yyyy/MM/dd");
    }
}
