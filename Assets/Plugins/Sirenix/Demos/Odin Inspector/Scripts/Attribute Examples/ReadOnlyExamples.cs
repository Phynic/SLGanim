namespace Sirenix.OdinInspector.Demos
{
    using UnityEngine;

    public class ReadOnlyExamples : MonoBehaviour
    {
        [InfoBox("ReadOnly disables properties in the inspector.")]
        [ReadOnly]
        public string MyString = "This is displayed as text";

        [ReadOnly]
        public int MyInt = 9001;

        [ReadOnly]
        public int[] MyIntList = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 };
    }
}