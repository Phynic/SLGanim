namespace Sirenix.OdinInspector.Demos
{
    using UnityEngine;
    using System.Collections.Generic;

#if UNITY_EDITOR

    using Sirenix.Utilities.Editor;

#endif

    public class ListExamples : MonoBehaviour
    {
        public List<float> FloatList;

        [Range(0, 1)]
        public float[] FloatRangeArray;

        [ReadOnly]
        public int[] ReadOnlyArray1 = new int[] { 1, 2, 3 };

        [ListDrawerSettings(IsReadOnly = true)]
        public int[] ReadOnlyArray2 = new int[] { 1, 2, 3 };

        [ListDrawerSettings(NumberOfItemsPerPage = 5)]
        public int[] FiveItemsPerPage;

        [ListDrawerSettings(ShowIndexLabels = true, ListElementLabelName = "SomeString")]
        public SomeStruct[] IndexLabels;

        [ListDrawerSettings(DraggableItems = false, Expanded = false, ShowIndexLabels = true, ShowPaging = false, ShowItemCount = false)]
        public int[] MoreListSettings = new int[] { 1, 2, 3 };

        [ListDrawerSettings(OnBeginListElementGUI = "BeginDrawListElement", OnEndListElementGUI = "EndDrawListElement")]
        public SomeStruct[] InjectListElementGUI;
        
        [ListDrawerSettings(HideAddButton = true, OnTitleBarGUI = "DrawAddButton")]
        public List<int> CustomButtons;

#if UNITY_EDITOR

        private void BeginDrawListElement(int index)
        {
            SirenixEditorGUI.BeginBox(this.InjectListElementGUI[index].SomeString);
        }
        private void EndDrawListElement(int index)
        {
            SirenixEditorGUI.EndBox();
        }

        private void DrawAddButton()
        {
            if (SirenixEditorGUI.ToolbarButton(EditorIcons.Plus))
            {
                this.CustomButtons.Add(Random.Range(0, 100));
            }

            GUIHelper.PushGUIEnabled(GUI.enabled && this.CustomButtons.Count > 0);
            if (SirenixEditorGUI.ToolbarButton(EditorIcons.Minus))
            {
                this.CustomButtons.RemoveAt(this.CustomButtons.Count - 1);
            }
            GUIHelper.PopGUIEnabled();
        }

#endif

        [System.Serializable]
        public struct SomeStruct
        {
            public string SomeString;
            public int One;
            public int Two;
            public int Three;
        }
    }
}