using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class ButtonTextHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private TextMeshProUGUI tmp;
    private string originalText;

    void Awake()
    {
        tmp = GetComponentInChildren<TextMeshProUGUI>();
        if (tmp != null)
            originalText = tmp.text;
    }

    // Quand la souris survole le bouton
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (tmp != null)
        {
            tmp.fontStyle |= FontStyles.Bold; // ajoute le gras
        }
    }

    // Quand la souris quitte le bouton
    public void OnPointerExit(PointerEventData eventData)
    {
        if (tmp != null)
        {
            tmp.fontStyle &= ~FontStyles.Bold; // enlève le gras
        }
    }
}
