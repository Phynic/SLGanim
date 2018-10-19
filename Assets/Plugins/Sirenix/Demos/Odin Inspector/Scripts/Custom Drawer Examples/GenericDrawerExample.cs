namespace Sirenix.OdinInspector.Demos
{
    using UnityEngine;
    using Sirenix.OdinInspector;
    using Sirenix.Serialization;
    using System.Collections.Generic;

#if UNITY_EDITOR

    using Sirenix.OdinInspector.Editor;
    using Sirenix.Utilities.Editor;
    using UnityEditor;

#endif

    // Example demonstrating how drawers can be implemented with generic constraints.
    public class GenericDrawerExample : SerializedMonoBehaviour
    {
        [InfoBox(
            "This examples demonstates how a custom drawer can defined to be generic." +
            "\nThis allows a single drawer implementation, to deal with a wide array of values.")]
        [OdinSerialize]
        public MyGenericClass<int, int> A; // Drawn with struct drawer

        [OdinSerialize]
        public MyGenericClass<Vector3, Quaternion> B; // Drawn with struct drawer

        [OdinSerialize]
        public MyGenericClass<int, GameObject> C; // Drawn with generic parameter extraction drawer

        [OdinSerialize]
        public MyGenericClass<string, List<string>> D; // Drawn with strong list drawer

        [OdinSerialize]
        public MyGenericClass<string, string> E; // Drawn with default drawers, as none of the generic drawers beneath apply

        public List<MyClass> F; // Drawn with the custom list drawer
    }

    // Generic class with any two generic types.
    public class MyGenericClass<T1, T2>
    {
        public T1 First;
        public T2 Second;
    }

#if UNITY_EDITOR

    // Remember to add the OdinDrawer to your custom drawer classes, or it will not be found by Odin.
    [OdinDrawer]
    public class MyGenericClassDrawer_Struct<T1, T2> : OdinValueDrawer<MyGenericClass<T1, T2>>
        where T1 : struct
        where T2 : struct
    {
        protected override void DrawPropertyLayout(IPropertyValueEntry<MyGenericClass<T1, T2>> entry, GUIContent label)
        {
            SirenixEditorGUI.DrawSolidRect(EditorGUILayout.GetControlRect(), Color.red);
            this.CallNextDrawer(entry, label);
        }
    }

    [OdinDrawer]
    public class MyGenericClassDrawer_StrongList<TElement, TList> : OdinValueDrawer<MyGenericClass<TElement, TList>>
        where TList : IList<TElement>
        where TElement : class
    {
        protected override void DrawPropertyLayout(IPropertyValueEntry<MyGenericClass<TElement, TList>> entry, GUIContent label)
        {
            SirenixEditorGUI.DrawSolidRect(EditorGUILayout.GetControlRect(), Color.blue);
            this.CallNextDrawer(entry, label);
        }
    }

    // Note how it is possible to give a generic parameter as the drawn type; Odin will look at the constraints on the parameter to determine where it applies
    [OdinDrawer]
    public class MyGenericClassDrawer_GenericParameterExtraction<TValue, TUnityObject> : OdinValueDrawer<TValue>
        where TValue : MyGenericClass<int, TUnityObject>
        where TUnityObject : UnityEngine.Object
    {
        protected override void DrawPropertyLayout(IPropertyValueEntry<TValue> entry, GUIContent label)
        {
            SirenixEditorGUI.DrawSolidRect(EditorGUILayout.GetControlRect(), Color.green);
            this.CallNextDrawer(entry, label);
        }
    }

    [OdinDrawer]
    [DrawerPriority(0, 0, 2)]
    public class MyClassListDrawer<TList, TElement> : OdinValueDrawer<TList>
        where TList : IList<TElement>
        where TElement : MyClass
    {
        protected override void DrawPropertyLayout(IPropertyValueEntry<TList> entry, GUIContent label)
        {
            SirenixEditorGUI.DrawSolidRect(EditorGUILayout.GetControlRect(), new Color(1, 0.5f, 0));
            this.CallNextDrawer(entry, label);
        }
    }

#endif
}