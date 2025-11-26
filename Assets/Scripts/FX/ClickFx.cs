using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickFx : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private GameObject prefabFx;
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Spawn");
        GameObject go = SimplePool.Spawn(prefabFx);
        VisualEffect vfx = go.GetComponent<VisualEffect>();
        vfx.PlayAndDespawn(transform);
    }
}
