namespace Sirenix.OdinInspector.Demos
{
    using System;
    using UnityEngine;

#if UNITY_EDITOR

    using Sirenix.OdinInspector.Editor;
    using Sirenix.Utilities.Editor;

#endif

    // Example demonstrating how use context objects in custom drawers.
    public class ContextExample : MonoBehaviour
    {
        [InfoBox(
            "This examples show how context objects can be used to keep track of individual states, for individual properties.\n" +
            "Context objects are used throughout all of these drawer examples, for different purposes. We recommend having a look at those examples as well.")]
        [ContextExample]
        public int Field;
    }

    // The attribute used by the ContextExampleAttributeDrawer.
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class ContextExampleAttribute : Attribute
    { }

#if UNITY_EDITOR

    // Place the drawer script file in an Editor folder.
    // Remember to add the OdinDrawer to your custom drawer classes, or it will not be found by Odin.
    [OdinDrawer]
    public class ContextExampleAttributeDrawer : OdinAttributeDrawer<ContextExampleAttribute>
    {
        protected override void DrawPropertyLayout(InspectorProperty property, ContextExampleAttribute attribute, GUIContent label)
        {
			// Get two context objects. One for a frame counter, and one for an on/off button.
            PropertyContext<int> frameCount = property.Context.Get(this, "counter", 0);
            PropertyContext<bool> toggle = property.Context.Get(this, "toggler", false);

			// Count the frames.
            if (Event.current.type == EventType.Layout && toggle.Value)
            {
                frameCount.Value++;
                GUIHelper.RequestRepaint();
            }

			// Draw the current frame count, and a start stop button.
            SirenixEditorGUI.BeginBox();
            {
                GUILayout.Label("Frame Count: " + frameCount.Value.ToString());

                if (GUILayout.Button(toggle.Value ? "Stop" : "Start"))
                {
                    toggle.Value = !toggle.Value;
                }
            }
            SirenixEditorGUI.EndBox();

			// Continue the drawer chain.
            this.CallNextDrawer(property, label);
        }
    }

#endif
}