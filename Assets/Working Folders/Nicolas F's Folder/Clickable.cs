using UnityEngine;
using UnityEngine.EventSystems;

public class ComputePowerClickable : MonoBehaviour, IPointerClickHandler
{
    public int ComputePowerGainOnClick = 1;

    public void OnPointerClick(PointerEventData eventData)
    {
        // Vérifie que c’est le clic gauche
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            Debug.Log("Clic gauche détecté sur " + gameObject.name);
            // Compute Power Update
            ComputePower.Instance.AddComputePower(ComputePowerGainOnClick);
            Debug.Log("Compute Power is now equal to " + ComputePower.Instance.value.ToString());
            // Unspawn bubble
            SimplePool.Despawn(gameObject);
        }
    }
}
