using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
namespace WiDiD.Reflection.Editors
{
    [CustomPropertyDrawer(typeof(ReflectedValue<>), true)]
    public class ReflectedValueDrawer : PropertyDrawer
    {
        private static readonly Dictionary<(int rootId, Type target), string[]> CachedPaths
            = new Dictionary<(int rootId, Type target), string[]>();

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty rootProp = property.FindPropertyRelative("rootObject");
            SerializedProperty pathProp = property.FindPropertyRelative("path");

            UnityEngine.Object rootObj = rootProp.objectReferenceValue;

            // Reflected type
            Type reflectedValueType = fieldInfo.FieldType;
            Type targetType = reflectedValueType.GetGenericArguments()[0];

            // Label text
            GUIContent finalLabel = new GUIContent($"{label.text} ({targetType.Name})");

            // Layout: divide inspector width
            float totalWidth = position.width;

            float sectionWidth = totalWidth / 3f;
            float valueWidth = sectionWidth / 2f;
            float labelWidth = sectionWidth / 2f;

            Rect labelRect = new Rect(position.x, position.y, labelWidth, position.height);
            Rect objectRect = new Rect(labelRect.xMax, position.y, sectionWidth, position.height);
            Rect dropdownRect = new Rect(objectRect.xMax, position.y, sectionWidth, position.height);
            Rect valueRect = new Rect(dropdownRect.xMax, position.y, valueWidth, position.height);

            // Draw label
            EditorGUI.LabelField(labelRect, finalLabel);

            // Draw object field
            EditorGUI.PropertyField(objectRect, rootProp, GUIContent.none);

            // Draw dropdown or placeholder
            if (rootObj == null)
            {
                EditorGUI.LabelField(dropdownRect, "Fill the object field", EditorStyles.centeredGreyMiniLabel);
                EditorGUI.LabelField(valueRect, "-", EditorStyles.centeredGreyMiniLabel);
                return;
            }

            // Get compatible paths
            Type rootType = rootObj.GetType();
            string[] paths = GetPaths(rootType, targetType, rootObj);

            int index = Array.IndexOf(paths, pathProp.stringValue);
            if (index < 0) index = 0;

            index = EditorGUI.Popup(dropdownRect, index, paths);
            pathProp.stringValue = paths.Length > 0 ? paths[index] : "";

            // Draw current value
            object currentVal = ReflectionPathUtility.GetValue(rootObj, pathProp.stringValue);
            string displayVal = currentVal != null ? currentVal.ToString() : "null";
            EditorGUI.LabelField(valueRect, displayVal);
        }
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var root = new VisualElement();
            root.style.flexDirection = FlexDirection.Row;
            root.style.alignItems = Align.Center;

            // Serialized properties
            var rootProp = property.FindPropertyRelative("rootObject");
            var pathProp = property.FindPropertyRelative("path");

            // Target type of ReflectedValue<T>
            Type reflectedValueType = fieldInfo.FieldType;
            Type targetType = reflectedValueType.GetGenericArguments()[0];

            // Label
            var label = new Label($"{property.displayName} ({targetType.Name})");
            label.style.minWidth = 40;
            label.style.unityTextAlign = TextAnchor.MiddleLeft;
            root.Add(label);

            // Declare popup and valueLabel first so they can be used in local functions
            PopupField<string> popup = null;
            Label valueLabel = null;

            // Object field
            var objectField = new ObjectField();
            objectField.objectType = typeof(UnityEngine.Object);
            objectField.value = rootProp.objectReferenceValue;
            objectField.style.flexGrow = 1;
            objectField.RegisterValueChangedCallback(evt =>
            {
                rootProp.objectReferenceValue = evt.newValue;
                rootProp.serializedObject.ApplyModifiedProperties();
                UpdatePathPopup();
            });
            root.Add(objectField);

            // Popup for reflected paths
            popup = new PopupField<string>(new List<string> { "Fill the object field" }, 0);
            popup.style.flexGrow = 1;
            popup.SetEnabled(false);
            popup.RegisterValueChangedCallback(evt =>
            {
                pathProp.stringValue = evt.newValue;
                pathProp.serializedObject.ApplyModifiedProperties();
                UpdateValueLabel();
            });
            root.Add(popup);

            // Value label
            valueLabel = new Label("-");
            valueLabel.style.minWidth = 20;
            valueLabel.style.unityTextAlign = TextAnchor.MiddleRight;
            root.Add(valueLabel);

            // Update popup content based on selected object
            void UpdatePathPopup()
            {
                var obj = rootProp.objectReferenceValue;
                if (obj == null)
                {
                    popup.choices = new List<string> { "Fill the object field" };
                    popup.index = 0;
                    popup.SetEnabled(false);
                    valueLabel.text = "-";
                    return;
                }

                Type rootType = obj.GetType();
                string[] paths = GetPaths(rootType, targetType, obj);

                popup.choices = paths.Length > 0 ? new List<string>(paths) : new List<string> { "(No matching members)" };
                int index = Mathf.Max(0, Array.IndexOf(paths, pathProp.stringValue));
                popup.index = index;
                popup.SetEnabled(paths.Length > 0);

                if (paths.Length > 0)
                    pathProp.stringValue = popup.value;
                else
                    pathProp.stringValue = "";

                pathProp.serializedObject.ApplyModifiedProperties();
                UpdateValueLabel();
            }

            // Update displayed value
            void UpdateValueLabel()
            {
                var obj = rootProp.objectReferenceValue;
                if (obj == null)
                {
                    valueLabel.text = "-";
                    return;
                }

                object currentVal = ReflectionPathUtility.GetValue(obj, pathProp.stringValue);
                valueLabel.text = currentVal != null ? currentVal.ToString() : "null";
            }

            // Initial setup
            UpdatePathPopup();

            return root;
        }
        private string[] GetPaths(Type rootType, Type targetType, UnityEngine.Object rootObj)
        {
            int rootId = rootObj.GetInstanceID();

            if (CachedPaths.TryGetValue((rootId, targetType), out var cached))
                return cached;

            List<string> paths = new List<string>();

            if (rootObj is GameObject go)
            {
                foreach (var comp in go.GetComponents<Component>())
                {
                    if (comp == null) continue;
                    string compName = comp.GetType().Name;
                    CollectPaths(comp.GetType(), compName, paths, 0, targetType, comp, fieldInfo);
                }
            }
            else
            {
                CollectPaths(rootType, "", paths, 0, targetType, rootObj, fieldInfo);
            }

            string[] result = paths.ToArray();
            CachedPaths[(rootId, targetType)] = result;
            return result;
        }
        private void CollectPaths(Type type, string prefix, List<string> paths, int depth, Type targetType, object rootObj, FieldInfo skipField)
        {
            if (depth > 4 || type == null) return;

            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

            foreach (var field in type.GetFields(flags))
            {
                if (field == skipField) continue;

                string path = string.IsNullOrEmpty(prefix) ? field.Name : $"{prefix}/{field.Name}";
                if (targetType.IsAssignableFrom(field.FieldType))
                    paths.Add(path);

                if (ShouldRecurse(field.FieldType))
                    CollectPaths(field.FieldType, path, paths, depth + 1, targetType, rootObj, skipField);
            }

            foreach (var prop in type.GetProperties(flags))
            {
                if (!prop.CanRead || prop.GetIndexParameters().Length > 0) continue;

                if (prop.Name == "Value" && prop.DeclaringType.IsGenericType &&
                    prop.DeclaringType.GetGenericTypeDefinition() == typeof(ReflectedValue<>))
                    continue;

                string path = string.IsNullOrEmpty(prefix) ? prop.Name : $"{prefix}/{prop.Name}";
                if (targetType.IsAssignableFrom(prop.PropertyType))
                    paths.Add(path);

                if (ShouldRecurse(prop.PropertyType))
                    CollectPaths(prop.PropertyType, path, paths, depth + 1, targetType, rootObj, skipField);
            }
        }

        private bool ShouldRecurse(Type type)
        {
            if (type == typeof(string)) return false;
            if (type.IsPrimitive) return false;
            if (type.IsEnum) return false;
            if (type.Namespace != null && type.Namespace.StartsWith("UnityEngine")) return false;
            return true;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }
}