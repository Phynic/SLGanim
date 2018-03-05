namespace Sirenix.OdinInspector.Demos
{
    using UnityEngine;

    public class DisableAndDisabledIfExamples : MonoBehaviour
    {
        [Indent]
        [Header("Enabled If")]
        public bool IsToggled;

        [Indent]
        [EnableIf("IsToggled")]
        public int EnableIfToggled;

        [Indent]
        [EnableIf("IsNotToggled")]
        public int EnableIfNotToggled;

        [Indent]
        [Header("Disable If")]
        [DisableIf("IsToggled")]
        public int DisableIfToggled;

        [Indent]
        [DisableIf("IsNotToggled")]
        public int DisableIfNotToggled;

        private bool IsNotToggled()
        {
            return !this.IsToggled;
        }
    }
}