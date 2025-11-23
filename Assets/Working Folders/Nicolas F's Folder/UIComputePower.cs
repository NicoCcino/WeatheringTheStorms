using UnityEngine;
using TMPro;

public class UIComputePower : MonoBehaviour
{

    public TextMeshProUGUI computePowerText;

    void OnEnable()
    {
        // S'abonner à l'événement
        if (ComputePower.Instance != null)
            ComputePower.Instance.OnCP += UpdateComputePowerText;
        // Mettre à jour le texte immédiatement avec la valeur actuelle
        UpdateComputePowerText(ComputePower.Instance.value);
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

        // Si tu veux afficher un "+" pour les valeurs positives
        // computePowerText.text = (value >= 0 ? "+" : "") + value.ToString();
    }
}
