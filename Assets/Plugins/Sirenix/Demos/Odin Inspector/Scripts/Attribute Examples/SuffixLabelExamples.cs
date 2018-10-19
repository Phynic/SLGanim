namespace Sirenix.OdinInspector.Demos
{
	using UnityEngine;

	public class SuffixLabelExamples : MonoBehaviour
	{
		[InfoBox(
			"The SuffixLabel attribute draws a label at the end of a property. " +
			"It's useful for conveying intend about a property." +
			"\nFx, is the distance measured in meters, kilometers, or light years, or is the angle measured in degrees, or radians.")]
		[SuffixLabel("Prefab")]
		public GameObject GameObject;

		[Space(15)]
		[InfoBox(
			"Using the Overlay property, the suffix label will be drawn on top of the property instead of behind it." +
			"\nUse this for a neat inline look.")]
		[SuffixLabel("ms", Overlay = true)]
		public float Speed;

		[SuffixLabel("radians", Overlay = true)]
		public float Angle;

		[Space(15)]
		[InfoBox("The SuffixAttribute also supports referencing a member string field, property, or method by using $.")]
		[SuffixLabel("$Suffix", Overlay = true)]
		public string Suffix = "Dynamic suffix label";
	}
}