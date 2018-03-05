using UnityEngine;

namespace Sirenix.OdinInspector.Demos
{
    public class HideReferenceObjectPickerExamples : SerializedMonoBehaviour
    {
        [Title("Hidden Object Pickers")]
        [HideReferenceObjectPicker]
        public MyCustomReferenceType OdinSerializedProperty1;

        [HideReferenceObjectPicker]
        public MyCustomReferenceType OdinSerializedProperty2;

        [Title("Shown Object Pickers")]
        public MyCustomReferenceType OdinSerializedProperty3;

        public MyCustomReferenceType OdinSerializedProperty4;

#if UNITY_EDITOR

        [PropertyOrder(-1)]
        [OnInspectorGUI]
        private void OnInspectorGUI()
        {
            UnityEditor.EditorGUILayout.HelpBox("When the object picker is hidden, you can right click and set the instance to null, in order to set a new value.\n\n" +
                "If you don't want this behavior, you can use DisableContextMenu attribute to ensure people can't change the value.", UnityEditor.MessageType.Info);
        }

#endif

        public class MyCustomReferenceType
        {
            public int A;
            public int B;
            public int C;
        }
    }
}