using UnityEngine;
using UnityEngine.EventSystems;

public static class ClickableHelper
{
    public static void PointerEnterClickable(IPointerEnterHandler pointerEnterHandler)
    {
        if (pointerEnterHandler is MonoBehaviour mono)
        {
            mono.transform.localScale *= 1.1f;
        }
    }
    public static void PointerExitClickable(IPointerEnterHandler pointerExitHandler)
    {
        if (pointerExitHandler is MonoBehaviour mono)
        {
            mono.transform.localScale /= 1.1f;
        }
    }
}
