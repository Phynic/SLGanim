namespace Sirenix.OdinInspector.Demos
{
    using Sirenix.Serialization;
    using UnityEngine;

    // Neather of these members will be serialized by Odin because we are not inhereting from SerializedMonoBehaviour.
    public class IncorrectUseOfOdinSerializeAttributeExamples1 : MonoBehaviour
    {
        [OdinSerialize, ShowInInspector]
        private MyCustomType1 UnitySerializedField1;

        [OdinSerialize, ShowInInspector]
        public MyCustomType2 UnitySerializedField2;

        [OdinSerialize, ShowInInspector]
        public MyCustomType1 UnitySerializedProperty1 { get; private set; }

        [OdinSerialize, ShowInInspector]
        public MyCustomType2 UnitySerializedProperty2 { get; private set; }

        [System.Serializable]
        public class MyCustomType1
        {
            public int Test;
        }

        public class MyCustomType2
        {
            public int Test;
        }
    }
}