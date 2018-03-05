namespace Sirenix.OdinInspector.Demos
{
    using UnityEngine;
    using Sirenix.OdinInspector;

    public class ShowAndHideIfExamples : MonoBehaviour
    {
        public bool IsToggled;

        [ShowIf("IsToggled")]
        public Vector2 VisibleWhenToggled;

        [ShowIf("IsNotToggled")]
        public Vector3 VisibleWhenNotToggled;

        [ShowIf("IsInEditMode")]
        public Vector3 VisibleOnlyInEditorMode;

        [HideIf("IsToggled")]
        public Vector2 HiddenWhenToggled;

        [HideIf("IsNotToggled")]
        public Vector3 HiddenWhenNotToggled;

        [HideIf("IsInEditMode")]
        public Vector3 HiddenOnlyInEditorMode;

        private bool IsNotToggled()
        {
            return !this.IsToggled;
        }

        private bool IsInEditMode()
        {
            return !Application.isPlaying;
        }
    }
}