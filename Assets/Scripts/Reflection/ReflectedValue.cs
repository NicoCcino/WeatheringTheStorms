using System;
using UnityEngine;

namespace WiDiD.Reflection
{
    [Serializable]
    public class ReflectedValue<T>
    {
        [SerializeField] private UnityEngine.Object rootObject; // the object to start from
        [SerializeField] private string path; // reflected path

        public UnityEngine.Object Root => rootObject;
        public string Path => path;

        public bool IsValid => rootObject != null && path != string.Empty;
        public T Value
        {
            get
            {
                if (rootObject == null || string.IsNullOrEmpty(path))
                    return default;

                object val = ReflectionPathUtility.GetValue(rootObject, path);
                if (val is T cast)
                    return cast;

                return default;
            }
            set
            {
                if (rootObject == null || string.IsNullOrEmpty(path))
                    return;

                ReflectionPathUtility.SetValue(rootObject, path, value);
            }
        }
    }
}