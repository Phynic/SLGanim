namespace Sirenix.OdinInspector.Demos
{
    using System;
    using UnityEngine;

#if UNITY_EDITOR

    using Sirenix.OdinInspector.Editor;
    using UnityEditor;
    using Sirenix.Utilities.Editor;
    using Sirenix.Utilities;

#endif

    // Example demonstrating how to create a custom drawer for an attribute.
    public class HealthBarExample : MonoBehaviour
    {
        [InfoBox("Here a visualization of a health bar being drawn with with a custom attribute drawer.")]
        [HealthBar(100)]
        public float Health;
    }

    // Attribute used by HealthBarAttributeDrawer.
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class HealthBarAttribute : Attribute
    {
        public float MaxHealth { get; private set; }

        public HealthBarAttribute(float maxHealth)
        {
            this.MaxHealth = maxHealth;
        }
    }

#if UNITY_EDITOR

    // Place the drawer script file in an Editor folder or wrap it in a #if UNITY_EDITOR condition.
    // Remember to add the OdinDrawer to your custom drawer classes, or they will not be found by Odin.
    [OdinDrawer]
    public class HealthBarAttributeDrawer : OdinAttributeDrawer<HealthBarAttribute, float>
    {
        protected override void DrawPropertyLayout(IPropertyValueEntry<float> entry, HealthBarAttribute attribute, GUIContent label)
        {
            // Call the next drawer, which will draw the float field.
            this.CallNextDrawer(entry.Property, label);

            // Get a rect to draw the health-bar on. You Could also use GUILayout instead, but using rects makes it simpler to draw the health bar.
            Rect rect = EditorGUILayout.GetControlRect();

            // Draw the health bar.
            SirenixEditorGUI.DrawSolidRect(rect, new Color(0f, 0f, 0f, 0.3f), false);
            SirenixEditorGUI.DrawSolidRect(rect.SetWidth(rect.width * Mathf.Clamp01(entry.SmartValue / attribute.MaxHealth)), Color.red, false);
            SirenixEditorGUI.DrawBorders(rect, 1);
        }
    }

#endif
}