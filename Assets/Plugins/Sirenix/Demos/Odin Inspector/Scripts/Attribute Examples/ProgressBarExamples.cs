namespace Sirenix.OdinInspector.Demos
{
	using UnityEngine;
	using Sirenix.OdinInspector;
	using Sirenix.Utilities;

	public sealed class ProgressBarExamples : MonoBehaviour
	{
		[InfoBox("The ProgressBar attribute draws a horizontal colored bar, which can also be clicked to change the value." +
			"\n\nIt can be used to show how full an inventory might be, or to make a visual indicator for a healthbar. " +
			"It can even be used to make fighting game style health bars, that stack multiple layers of health.")]
		[ProgressBar(0, 100)]
		public int ProgressBar = 50;

		[InfoBox("Using the ColorMember property, you can make a healthbar that changes color, the lower the value gets.")]
		[Space(15)]
		[ProgressBar(0, 100, ColorMember = "GetHealthBarColor")]
		public float HealthBar = 50;

		private Color GetHealthBarColor(float value)
		{
			// Blends between red, and yellow color for when the health is below 30,
			// and blends between yellow and green color for when the health is above 30.
			return Color.Lerp(Color.Lerp(
				Color.red, Color.yellow, MathUtilities.LinearStep(0f, 30f, value)),
				Color.green, MathUtilities.LinearStep(0f, 100f, value));
		}

		[InfoBox(
			"Using both ColorMember and BackgroundColorMember properties, and applying the ProgressBar attribute on a proprety, " +
			"you can make stacked health, that changes color, when the health is above 100%." +
			"\n\nSimilar to what you might see in a fighting game.")]
		[Range(0, 300), Space(15)]
		public float StackedHealth;

		[HideLabel, ShowInInspector]
		[ProgressBar(0, 100, ColorMember = "GetStackedHealthColor", BackgroundColorMember = "GetStackHealthBackgroundColor")]
		private float StackedHealthProgressBar
		{
			// Loops the stacked health value between 0, and 100.
			get { return this.StackedHealth - 100 * (int)((this.StackedHealth - 1) / 100); }
		}

		private Color GetStackedHealthColor()
		{
			return
				this.StackedHealth > 200 ? Color.cyan :
				this.StackedHealth > 100 ? Color.green :
				Color.red;
		}

		private Color GetStackHealthBackgroundColor()
		{
			return
				this.StackedHealth > 200 ? Color.green :
				this.StackedHealth > 100 ? Color.red :
				new Color(0.16f, 0.16f, 0.16f, 1f);
		}

		[InfoBox("It's also possible to change the size of a healthbar, using the Height property. Or you can specify a custom color, without refering to another color member.")]
		[PropertyOrder(10), HideLabel, Space(15)]
		[ProgressBar(-100, 100, r: 1, g: 1, b: 1, Height = 30)]
		public short BigProgressBar = 50;
	}
}