namespace Sirenix.OdinInspector.Demos
{
    using UnityEngine;
    using System;

#if UNITY_EDITOR

    using Sirenix.OdinInspector.Editor;
    using UnityEditor;
    using Sirenix.Utilities;

#endif

    // Example demonstating how to make a custom drawer for a custom type.
    public class CustomDrawerExample : MonoBehaviour
    {
        [InfoBox("This example demonstrates how a custom drawer can be implemented for a custom struct or class.")]
        public MyStruct MyStruct;
    }

    // Custom data struct, for demonstration.
    [Serializable]
    public struct MyStruct
    {
        public float X;
        public float Y;
    }

#if UNITY_EDITOR

    [OdinDrawer]
    public class CustomStructDrawer : OdinValueDrawer<MyStruct>
    {
        protected override void DrawPropertyLayout(IPropertyValueEntry<MyStruct> entry, GUIContent label)
        {
            MyStruct value = entry.SmartValue;

            var rect = EditorGUILayout.GetControlRect();

            // In Odin, labels are optional and can be null, so we have to account for that.
            if (label != null)
            {
                rect = EditorGUI.PrefixLabel(rect, label);
            }

            var prev = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 20;

            value.X = EditorGUI.Slider(rect.AlignLeft(rect.width * 0.5f), "X", value.X, 0, 1);
            value.Y = EditorGUI.Slider(rect.AlignRight(rect.width * 0.5f), "Y", value.Y, 0, 1);

            EditorGUIUtility.labelWidth = prev;

            entry.SmartValue = value;
        }
    }

#endif
}