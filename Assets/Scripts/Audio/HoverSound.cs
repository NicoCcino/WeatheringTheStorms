using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
public class HoverSound : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] private AudioClip audioClip;
    public void OnPointerEnter(PointerEventData eventData)
    {
        AudioManager.Instance.PlaySound(audioClip, 0.4f);
    }
}
