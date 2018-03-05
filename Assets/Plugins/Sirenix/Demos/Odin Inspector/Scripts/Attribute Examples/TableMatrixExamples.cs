namespace Sirenix.OdinInspector.Demos
{
    using UnityEngine;
    using Sirenix.OdinInspector;
    using Sirenix.Utilities;

    // Inheriting from SerializedMonoBehaviour is only needed if you want Odin to serialize the multi-dimensional arrays for you.
    // If you prefer doing that yourself, you can still make Odin show them in the inspector using the ShowInInspector attribute.
    public class TableMatrixExamples : SerializedMonoBehaviour
    {
        [InfoBox("Right-click and drag the column and row labels in order to modify the tables."), PropertyOrder(-10), OnInspectorGUI]
        private void MSG() { }

        [BoxGroup("Two Dimensional array without the TableMatrix attribute.")]
        public bool[,] BooleanTable = new bool[15, 6];

        [BoxGroup("ReadOnly table")]
        [TableMatrix(IsReadOnly = true)]
        public int[,] ReadOnlyTable = new int[5, 5];

        [BoxGroup("Labled table")]
        [TableMatrix(HorizontalTitle = "X axis", VerticalTitle = "Y axis")]
        public GameObject[,] LabledTable = new GameObject[15, 10];

        [BoxGroup("Enum table")]
        [TableMatrix(HorizontalTitle = "X axis")]
        public InfoMessageType[,] EnumTable = new InfoMessageType[4, 4];

        [BoxGroup("Custom table")]
        [TableMatrix(DrawElementMethod = "DrawColoredEnumElement", ResizableColumns = false, RowHeight = 16)]
        public bool[,] CustomCellDrawing = new bool[30, 30];

#if UNITY_EDITOR

        private static bool DrawColoredEnumElement(Rect rect, bool value)
        {
            if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition))
            {
                value = !value;
                GUI.changed = true;
                Event.current.Use();
            }

            UnityEditor.EditorGUI.DrawRect(rect.Padding(1), value ? new Color(0.1f, 0.8f, 0.2f) : new Color(0, 0, 0, 0.5f));

            return value;
        }

#endif
    }
}