using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace WiDiD.Reflection
{
    public static class ReflectionPathUtility
    {
        public static object GetValue(object root, string path)
        {
            if (root == null || string.IsNullOrEmpty(path))
                return null;

            string[] parts = path.Split('/');
            object current = root;

            if (root is GameObject go)
            {
                string first = parts[0];

                if (first == "GameObject")
                {
                    current = go;
                    return parts.Length > 1 ? GetValueRecursive(current, parts, 1) : current;
                }
                else
                {
                    var comp = go.GetComponent(first);
                    if (comp == null) return null;
                    current = comp;
                    return parts.Length > 1 ? GetValueRecursive(current, parts, 1) : current;
                }
            }

            return GetValueRecursive(current, parts, 0);
        }

        private static object GetValueRecursive(object current, string[] parts, int index)
        {
            if (current == null || index >= parts.Length) return current;

            string part = parts[index];
            object next = GetMemberValue(current, part);
            return GetValueRecursive(next, parts, index + 1);
        }

        public static bool SetValue(object root, string path, object newValue)
        {
            if (root == null || string.IsNullOrEmpty(path))
                return false;

            string[] parts = path.Split('/');
            object current = root;

            if (root is GameObject go)
            {
                string first = parts[0];

                if (first == "GameObject")
                {
                    current = go;
                    return SetValueRecursive(current, parts, 1, newValue);
                }
                else
                {
                    var comp = go.GetComponent(first);
                    if (comp == null) return false;
                    current = comp;
                    return SetValueRecursive(current, parts, 1, newValue);
                }
            }

            return SetValueRecursive(current, parts, 0, newValue);
        }

        private static bool SetValueRecursive(object current, string[] parts, int index, object newValue)
        {
            if (current == null) return false;

            string part = parts[index];

            // last segment → assign
            if (index == parts.Length - 1)
            {
                return SetMemberValue(current, part, newValue);
            }

            // otherwise recurse
            object next = GetMemberValue(current, part);
            return SetValueRecursive(next, parts, index + 1, newValue);
        }

        // ------------------------
        // Helpers
        // ------------------------

        private static object GetMemberValue(object obj, string part)
        {
            if (obj == null) return null;

            string memberName = part;
            string indexer = null;

            // handle collection index
            int idxStart = part.IndexOf('[');
            if (idxStart >= 0 && part.EndsWith("]"))
            {
                memberName = part.Substring(0, idxStart);
                indexer = part.Substring(idxStart + 1, part.Length - idxStart - 2);
            }

            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            Type type = obj.GetType();

            object value = null;

            if (!string.IsNullOrEmpty(memberName))
            {
                FieldInfo field = type.GetField(memberName, flags);
                if (field != null)
                    value = field.GetValue(obj);

                PropertyInfo prop = type.GetProperty(memberName, flags);
                if (prop != null && prop.CanRead)
                    value = prop.GetValue(obj);
            }
            else
            {
                value = obj; // directly index root
            }

            if (value != null && indexer != null)
            {
                // array / list
                if (value is IList list && int.TryParse(indexer, out int i))
                {
                    if (i >= 0 && i < list.Count)
                        return list[i];
                }

                // dictionary
                if (value is IDictionary dict)
                {
                    return dict[indexer];
                }
            }

            return value;
        }

        private static bool SetMemberValue(object obj, string part, object newValue)
        {
            if (obj == null) return false;

            string memberName = part;
            string indexer = null;

            int idxStart = part.IndexOf('[');
            if (idxStart >= 0 && part.EndsWith("]"))
            {
                memberName = part.Substring(0, idxStart);
                indexer = part.Substring(idxStart + 1, part.Length - idxStart - 2);
            }

            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            Type type = obj.GetType();

            object container = null;

            if (!string.IsNullOrEmpty(memberName))
            {
                FieldInfo field = type.GetField(memberName, flags);
                if (field != null)
                {
                    container = field.GetValue(obj);
                    if (indexer == null)
                    {
                        field.SetValue(obj, newValue);
                        return true;
                    }
                }

                PropertyInfo prop = type.GetProperty(memberName, flags);
                if (prop != null)
                {
                    container = prop.GetValue(obj);
                    if (indexer == null && prop.CanWrite)
                    {
                        prop.SetValue(obj, newValue);
                        return true;
                    }
                }
            }
            else
            {
                container = obj; // directly index root
            }

            if (container != null && indexer != null)
            {
                // array / list
                if (container is IList list && int.TryParse(indexer, out int i))
                {
                    if (i >= 0 && i < list.Count)
                    {
                        list[i] = newValue;
                        return true;
                    }
                }

                // dictionary
                if (container is IDictionary dict)
                {
                    dict[indexer] = newValue;
                    return true;
                }
            }

            return false;
        }
    }
}
