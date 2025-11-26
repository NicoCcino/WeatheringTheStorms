using System.Collections;
using UnityEngine;

public class UIButtonFX : MonoBehaviour
{
    [SerializeField] private RectTransform fxTransform;

    // Use anchoredPosition values. For a stretched rect, these are offsets from the center.
    // E.g., -100 on X moves the center 100 units left from the parent's center.
    [SerializeField] private Vector2 startPos = new Vector2(-100f, 0f);
    [SerializeField] private Vector2 endPos = new Vector2(100f, 0f);

    public void PlayAnimation(float duration)
    {
        StartCoroutine(FxCoroutine(duration));
    }

    private IEnumerator FxCoroutine(float duration)
    {
        float timer = 0.0f;
        float t = 0f;

        // Ensure it starts at the beginning position immediately
        fxTransform.anchoredPosition = startPos;

        while (timer < duration)
        {
            t = timer / duration;

            // Use Vector2.Lerp since anchoredPosition is a Vector2
            fxTransform.anchoredPosition = Vector2.Lerp(startPos, endPos, t);

            timer += Time.deltaTime;
            yield return null;
        }

        // Ensure it always ends exactly at the end position
        fxTransform.anchoredPosition = endPos;
    }
}