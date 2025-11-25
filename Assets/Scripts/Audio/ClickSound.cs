using UnityEngine;
using UnityEngine.EventSystems;

public class ClickSound : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private AudioClip clickAudio;
    public void OnPointerClick(PointerEventData eventData)
    {
        AudioManager.Instance.PlaySound(clickAudio, 0.5f);
    }
}
