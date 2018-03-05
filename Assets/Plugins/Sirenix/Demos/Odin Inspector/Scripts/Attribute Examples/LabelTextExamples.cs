namespace Sirenix.OdinInspector.Demos
{
    using UnityEngine;

    public class LabelTextExamples : MonoBehaviour
    {
        [InfoBox("Specify a different label text for your properties.")]
        [LabelText("1")]
        public int MyInt1;

        [LabelText("2")]
        public int MyInt2;

        [LabelText("3")]
        public int MyInt3;

		[InfoBox("Use $ to refer to a member string.")]
		[LabelText("$LabelText")]
		public string LabelText = "Dynamic label text";
    }
}