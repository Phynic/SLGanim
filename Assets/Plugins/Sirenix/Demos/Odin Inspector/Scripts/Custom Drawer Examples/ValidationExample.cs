namespace Sirenix.OdinInspector.Demos
{
    using System;
    using UnityEngine;

#if UNITY_EDITOR

    using Sirenix.OdinInspector.Editor;
    using Sirenix.Utilities.Editor;

#endif

    // This example demonstates how to implement drawers that can validate properties,
    // and how to add warnings and errors that will be picked up by Odin Scene Validator.
    public class ValidationExample : MonoBehaviour
    {
        [InfoBox(
            "This is example demonstrates how to implement a custom drawer, that validates the property's value, " +
            "and how to get Odin Scene Validator to pick up that validation warning or error.")]
        [NotOne]
        public int NotOne;
    }

    // Attribute used by the NotOneAttributeDrawer
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class NotOneAttribute : Attribute
    {
    }

#if UNITY_EDITOR

    // Place the drawer script file in an Editor folder.
    // Remember to add the OdinDrawer to your custom drawer classes, or it will not be found by Odin.
    [OdinDrawer]
    public class NotOneAttributeDrawer : OdinAttributeDrawer<NotOneAttribute, int>
    {
        protected override void DrawPropertyLayout(IPropertyValueEntry<int> entry, NotOneAttribute attribute, GUIContent label)
        {
            SirenixEditorGUI.BeginBox();
            {
                if (entry.SmartValue == 1)
                {
                    // Odin Scene Validator will listen for calls to SirenixEditorGUI.WarningMessageBox and SirenixEditorGUI.ErrorMessageBox,
                    // so as long as you're just calling one of those two functions, the Scene Validator will catch it.
                    SirenixEditorGUI.ErrorMessageBox("1 is not a valid value.");
                }
                else
                {
                    SirenixEditorGUI.InfoMessageBox("Value is not 1 and therefore valid.");
                }

                // Continue the drawer chain.
                this.CallNextDrawer(entry, label);

                // Button for opening the Odin Scene Validator window.
                if (GUILayout.Button("Open Odin Scene Validator"))
                {
                    OdinSceneValidatorWindow.OpenWindow();
                }
            }
            SirenixEditorGUI.EndBox();
        }
    }

#endif
}