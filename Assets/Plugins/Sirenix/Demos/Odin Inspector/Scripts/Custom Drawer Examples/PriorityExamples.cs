namespace Sirenix.OdinInspector.Demos
{
    using System;
    using UnityEngine;

#if UNITY_EDITOR

    using Sirenix.OdinInspector.Editor;
    using Sirenix.Utilities.Editor;

#endif

    // Example demonstrating drawer priorities.
    public class PriorityExamples : MonoBehaviour
    {
        [InfoBox(
            "In this example, we have three different drawers, with different priorities, all drawing the same value.\n\n" +
            "The purpose is to demonstrate the drawer chain, and the general purpose of each drawer priority.")]
        [ShowDrawerChain] // Displays all drawers involved with drawing the property.
        public MyClass MyClass;
    }

    [Serializable]
    public class MyClass
    {
        public string Name;
        public float Value;
    }

#if UNITY_EDITOR

    // This drawer is configured to have super priority. Of the three drawers here, this class will be called first.
    // In our example here, the super drawer instanciates the value, if it's null.
    [OdinDrawer]
    [DrawerPriority(1, 0, 0)]
    public class CUSTOM_SuperPriorityDrawer : OdinValueDrawer<MyClass>
    {
        protected override void DrawPropertyLayout(IPropertyValueEntry<MyClass> entry, GUIContent label)
        {
            // Create the value, if it's not created already.
            if (entry.SmartValue == null)
            {
                entry.SmartValue = new MyClass();
            }

            this.CallNextDrawer(entry, label);
        }
    }

    // This drawer is configured to have wrapper priority, and is therefore be called second.
    // In this example, the wrapper drawer draws a box around the property.
    [OdinDrawer]
    [DrawerPriority(0, 1, 0)]
    public class CUSTOM_WrapperPriorityDrawer : OdinValueDrawer<MyClass>
    {
        protected override void DrawPropertyLayout(IPropertyValueEntry<MyClass> entry, GUIContent label)
        {
            // Draw a box around the property.
            SirenixEditorGUI.BeginBox(label);
            this.CallNextDrawer(entry, null);
            SirenixEditorGUI.EndBox();
        }
    }

    // This drawer is configured to have value priority, and is therefore called last.'
    // In this example, the value drawer draws the fields of the PriorityClass object.
    [OdinDrawer]
    [DrawerPriority(0, 0, 1)]
    public class CUSTOM_ValuePriorityDrawer : OdinValueDrawer<MyClass>
    {
        protected override void DrawPropertyLayout(IPropertyValueEntry<MyClass> entry, GUIContent label)
        {
            // Draw the value fields.
            entry.Property.Children["Name"].Draw();
            entry.Property.Children["Value"].Draw();
        }
    }

#endif
}