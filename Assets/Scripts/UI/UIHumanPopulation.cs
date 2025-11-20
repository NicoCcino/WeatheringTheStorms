using TMPro;
using UnityEngine;

public class UIHumanPopulation : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textHumanPopulation;
    private void Update()
    {
        textHumanPopulation.text = Human.Instance.HumanCount.ToString("###\\.###\\.###\\.###");
    }
}
