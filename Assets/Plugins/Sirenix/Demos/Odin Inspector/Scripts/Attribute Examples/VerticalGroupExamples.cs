namespace Sirenix.OdinInspector.Demos
{
	using UnityEngine;

	public class VerticalGroupExamples : MonoBehaviour
	{
#if UNITY_EDITOR
		[PropertyOrder(-1)]
		[OnInspectorGUI]
		public void DrawInfo()
		{
			Sirenix.Utilities.Editor.SirenixEditorGUI.InfoMessageBox(
				"VerticalGroup, similar to HorizontalGroup, groups properties together vertically in the inspector.\n" +
				"By itself it doesn't do much, but combined with other groups, like HorizontalGroup, it can be very useful.");
		}
#endif
		
		[HorizontalGroup("Split")]
		[VerticalGroup("Split/Left")]
		public InfoMessageType First;

		[VerticalGroup("Split/Left")]
		public InfoMessageType Second;

		[HideLabel]
		[VerticalGroup("Split/Right")]
		public int A;

		[HideLabel]
		[VerticalGroup("Split/Right")]
		public int B;
	}
}
