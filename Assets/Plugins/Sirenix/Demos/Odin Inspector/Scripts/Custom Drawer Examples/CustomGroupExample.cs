namespace Sirenix.OdinInspector.Demos
{
    using System;
    using UnityEngine;

#if UNITY_EDITOR

    using Sirenix.OdinInspector.Editor;
    using Sirenix.Utilities.Editor;
    using Sirenix.Utilities;
    using UnityEditor;

#endif

    // Example demonstrating how to create a custom group drawer.
    public class CustomGroupExample : SerializedMonoBehaviour
    {
#if UNITY_EDITOR

        [PropertyOrder(-1)]
        [OnInspectorGUI]
        public void Whoops()
        {
            Utilities.Editor.SirenixEditorGUI.InfoMessageBox("We may have gone overboard with this example");
        }

#endif

        [PartyGroup(3f, 20f)]
        public int MyInt;

        [PartyGroup]
        public float MyFloat { get; set; }

        [PartyGroup]
        public void StateTruth()
        {
            Debug.Log("Odin Inspector is awesome.");
        }

        [PartyGroup("Group Two", 10f, 8f)]
        public Vector3 AVector3;

        [PartyGroup("Group Two")]
        public int AnotherInt;

        [InfoBox("Of course, all the controls are still usable. If you can catch them at least.")]
        [PartyGroup("Group Three", 0.8f, 250f)]
        public Quaternion AllTheWayAroundAndBack;

        [PartyGroup("Group Four", 1f, 12f)]
        public Thingy ThingyField;

        public class Thingy
        {
            [PartyGroup(1f, 12f)]
            public Thingy ThingyField;
        }
    }

    // The custom group attribute. Must inherit PropertyGroupAttribute.
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class PartyGroupAttribute : PropertyGroupAttribute
    {
        public float Speed { get; private set; }
        public float Range { get; private set; }

        public PartyGroupAttribute(float speed = 0f, float range = 0f, int order = 0) : base("_DefaultGroup", order)
        {
            this.Speed = speed;
            this.Range = range;
        }

        public PartyGroupAttribute(string groupId, float speed = 0f, float range = 0f, int order = 0) : base(groupId, order)
        {
            this.Speed = speed;
            this.Range = range;
        }

        // This function is used to combine multiple group properties together.
        // With this it's possible to only specify some settings in just a single attribute, and
        // still have those settings affect the group as a whole.
        protected override void CombineValuesWith(PropertyGroupAttribute other)
        {
            var party = (PartyGroupAttribute)other;
            if (this.Speed == 0f)
            {
                this.Speed = party.Speed;
            }

            if (this.Range == 0f)
            {
                this.Range = party.Range;
            }
        }
    }

#if UNITY_EDITOR

    // Place the drawer script file in an Editor folder.
    // Remember to add the OdinDrawer to your custom drawer classes, or they will not be found by Odin.
    [OdinDrawer]
    public class PartyGroupAttributeDrawer : OdinGroupDrawer<PartyGroupAttribute>
    {
        // Custom context object. See ContextExampleAttributeDrawer.cs for more info.
        private class PartyContext
        {
            public Color Start;
            public Color Target;
        }

        // Remember to add the OdinDrawer to your custom drawer classes, or they will not be found by Odin.
        protected override void DrawPropertyGroupLayout(InspectorProperty property, PartyGroupAttribute attribute, GUIContent label)
        {
            GUILayout.Space(8f);

            // Changes the current GUI transform matrix, to make the inspector party.
            if (Event.current.rawType != EventType.Layout)
            {
                Vector3 offset = property.LastDrawnValueRect.position + new Vector2(property.LastDrawnValueRect.width, property.LastDrawnValueRect.height) * 0.5f;
                Matrix4x4 matrix =
                    Matrix4x4.TRS(offset, Quaternion.identity, Vector3.one) *
                    Matrix4x4.TRS(Vector3.zero, Quaternion.AngleAxis(Mathf.Sin((float)EditorApplication.timeSinceStartup * attribute.Speed) * attribute.Range, Vector3.forward), Vector3.one * (1f + MathUtilities.BounceEaseInFastOut(Mathf.Sin((float)UnityEditor.EditorApplication.timeSinceStartup * 2f)) * 0.1f)) *
                    Matrix4x4.TRS(-offset + new Vector3(Mathf.Sin((float)EditorApplication.timeSinceStartup * 2f), 0f, 0f) * 100f, Quaternion.identity, Vector3.one) *
                    GUI.matrix;
                GUIHelper.PushMatrix(matrix);
            }

            // Changes the party color.
            if (Event.current.rawType == EventType.Repaint)
            {
                var contextBuffer = property.Context.Get<PartyContext>(this, "Color", (PartyContext)null);
                var context = contextBuffer.Value;
                if (context == null)
                {
                    context = new PartyContext()
                    {
                        Start = UnityEngine.Random.ColorHSV(0f, 1f, 0.8f, 1f, 1f, 1f),
                        Target = UnityEngine.Random.ColorHSV(0f, 1f, 0.8f, 1f, 1f, 1f),
                    };
                    contextBuffer.Value = context;
                }

                float t = MathUtilities.Bounce(Mathf.Sin((float)EditorApplication.timeSinceStartup * 2f));
                if (t <= 0f)
                {
                    context.Start = context.Target;
                    context.Target = UnityEngine.Random.ColorHSV(0f, 1f, 0.8f, 1f, 1f, 1f);
                }

                GUIHelper.PushColor(Color.Lerp(context.Start, context.Target, t));
            }

            // Draws all the child properties of the group.
            SirenixEditorGUI.BeginBox();
            for (int i = 0; i < property.Children.Count; i++)
            {
                property.Children[i].Draw();
            }
            SirenixEditorGUI.EndBox();

            // Revert changes to GUI color and matrix.
            if (Event.current.rawType == EventType.Repaint)
            {
                GUIHelper.PopColor();
            }
            if (Event.current.rawType != EventType.Layout)
            {
                GUIHelper.PopMatrix();
            }

            // Request a repaint for fluid motion.
            GUIHelper.RequestRepaint();
            GUILayout.Space(8f);
        }
    }

#endif
}