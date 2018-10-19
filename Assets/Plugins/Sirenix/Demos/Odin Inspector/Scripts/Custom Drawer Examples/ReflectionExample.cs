namespace Sirenix.OdinInspector.Demos
{
    using System;
    using UnityEngine;

#if UNITY_EDITOR

    using Sirenix.OdinInspector.Editor;
    using System.Reflection;
    using Sirenix.Utilities;
    using Sirenix.Utilities.Editor;

#endif

    // Example demonstrating how reflection can be used to enhance custom drawers.
    public class ReflectionExample : MonoBehaviour
    {
        [InfoBox(
            "This example demonstrates how reflection can be used to extend drawers from what otherwise would be possible.\n" +
            "In this case, a user can specify one of their own methods to receive a callback from the drawer chain.")]
        [OnClickMethod("OnClick")]
        public int InstanceMethod;

        [OnClickMethod("StaticOnClick")]
        public int StaticMethod;

        [OnClickMethod("InvalidOnClick")]
        public int InvalidMethod;

        private void OnClick()
        {
            Debug.Log("Hello?");
        }

        private static void StaticOnClick()
        {
            Debug.Log("Static Hello?");
        }
    }

    // Attribute with name of call back method.
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class OnClickMethodAttribute : Attribute
    {
        public string MethodName { get; private set; }

        public OnClickMethodAttribute(string methodName)
        {
            this.MethodName = methodName;
        }
    }

#if UNITY_EDITOR

    // Place the drawer script file in an Editor folder.
    // Remember to add the OdinDrawer to your custom drawer classes, or they will not be found by Odin.
    [OdinDrawer]
    public class OnClickMethodAttributeDrawer : OdinAttributeDrawer<OnClickMethodAttribute>
    {
        // Any non-static class or struct can be used as a context object,
        // and it's also possible to get multiple contexts for a single drawer.
        // In this case we've created a custom class, for use by this drawer only,
        // which holds all the data nessessary for the drawer to function.
        private class OnClickContext
        {
            // This field is used to display errors messages to the user, if something goes wrong.
            public string ErrorMessage;

            // Reference to the method specified by the user in the attribute.
            public MethodInfo Method;
        }

        protected override void DrawPropertyLayout(InspectorProperty property, OnClickMethodAttribute attribute, GUIContent label)
        {
            // Get context object for the current property.
            PropertyContext<OnClickContext> contextBuffer = property.Context.Get<OnClickContext>(this, "OnClickContext", (OnClickContext)null);
            OnClickContext context = contextBuffer.Value;

            // The context has been configured to null. So we initialize the context value the first time the property is drawn.
            if (context == null)
            {
                // Create the context and store the value in the buffer object.
                context = new OnClickContext();
                contextBuffer.Value = context;

                // Use MemberFinder to find the specified method, and store the method info in the context object.
                context.Method = MemberFinder.Start(property.ParentType)
                    .IsMethod()
                    .HasNoParameters()
                    .IsNamed(attribute.MethodName)
                    .GetMember<MethodInfo>(out contextBuffer.Value.ErrorMessage);
            }

            // Display any error that might have occured.
            if (context.ErrorMessage != null)
            {
                SirenixEditorGUI.ErrorMessageBox(context.ErrorMessage);

                // Continue drawing the rest of the property as normal.
                this.CallNextDrawer(property, label);
            }
            else
            {
                // Get the mouse down event.
                bool clicked = Event.current.rawType == EventType.MouseDown && Event.current.button == 0 && property.LastDrawnValueRect.Contains(Event.current.mousePosition);

                if (clicked)
                {
                    // Invoke the method stored in the context object.
                    if (context.Method.IsStatic)
                    {
                        context.Method.Invoke(null, null);
                    }
                    else
                    {
                        context.Method.Invoke(property.ParentValues[0], null);
                    }
                }

                // Draw the property.
                this.CallNextDrawer(property, label);

                if (clicked)
                {
                    // If the event havn't been used yet, then use it here.
                    Event.current.Use();
                }
            }
        }
    }

#endif
}