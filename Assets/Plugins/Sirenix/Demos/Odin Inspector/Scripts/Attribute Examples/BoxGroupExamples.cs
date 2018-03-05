namespace Sirenix.OdinInspector.Demos
{
    using System;
    using UnityEngine;

    public class BoxGroupExamples : MonoBehaviour
    {
        // Box with a centered title.
        [BoxGroup("Centered Title", centerLabel: true)]
        public int A;

        [BoxGroup("Centered Title", centerLabel: true)]
        public int B;

        [BoxGroup("Centered Title", centerLabel: true)]
        public int C;

        // Box with a title.
        [BoxGroup("Left Oriented Title")]
        public int D;

        [BoxGroup("Left Oriented Title")]
        public int E;

        // Box with a title received from a field.
        [BoxGroup("$DynamicTitle1"), LabelText("Dynamic Title")]
        public string DynamicTitle1 = "Dynamic box title";

        [BoxGroup("$DynamicTitle1")]
        public int F;

        // Box with a title received from a property.
        [BoxGroup("$DynamicTitle2")]
        public int G;

        [BoxGroup("$DynamicTitle2")]
        public int H;

        // Box without a title.
        [InfoBox("You can also hide the label of a box group.")]
        [BoxGroup("NoTitle", false)]
        public int I;

        [BoxGroup("NoTitle")]
        public int J;

        [BoxGroup("NoTitle")]
        public int K;

#if UNITY_EDITOR
        public string DynamicTitle2
        {
            get { return UnityEditor.PlayerSettings.productName; }
        }
#endif

        [BoxGroup("Boxed Struct"), HideLabel]
        public SomeStruct BoxedStruct;

        public SomeStruct DefaultStruct;

        [Serializable]
        public struct SomeStruct
        {
            public int One;
            public int Two;
            public int Three;
        }
    }

}