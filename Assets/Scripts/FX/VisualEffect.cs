using System.Collections;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class VisualEffect : MonoBehaviour
{
    private ParticleSystem ps;
    public void PlayAndDespawn(Transform sourceTransform)
    {
        ps = GetComponent<ParticleSystem>();
        float duration = ps.main.duration;
        transform.parent = PoolableParent.Instance.transform;
        transform.position = sourceTransform.position;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;
        StartCoroutine(FXCoroutine(duration));
    }
    private IEnumerator FXCoroutine(float duration)
    {
        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        SimplePool.Despawn(gameObject);
    }
}
