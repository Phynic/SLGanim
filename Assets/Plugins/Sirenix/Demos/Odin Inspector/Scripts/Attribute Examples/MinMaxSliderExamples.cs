namespace Sirenix.OdinInspector.Demos
{
    using UnityEngine;

    public class MinMaxSliderExamples : MonoBehaviour
    {
        [InfoBox("Uses a Vector2 where x is the min knob and y is the max knob.")]
        [MinMaxSlider(-10, 10)]
        public Vector2 MinMaxValueSlider;

        [MinMaxSlider(-10, 10, true)]
		public Vector2 WithFields;
    }
}