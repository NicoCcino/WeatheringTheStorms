using UnityEngine;


[DefaultExecutionOrder(-500)]
public class Singleton<T> : MonoBehaviour
where T : Component
{
    static private T instance;
    static public T Instance => instance;

    [SerializeField] private bool dontDestroyOnLoad = true;

    protected virtual void Awake()
    {
        if (instance == null)
        {
            instance = this as T;
            if (gameObject.transform.parent == null && dontDestroyOnLoad && Application.isPlaying)
                DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this);
        }
    }
}

