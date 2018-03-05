namespace Sirenix.OdinInspector.Demos
{
    using UnityEngine;

    public class ButtonAndButtonGroupExamples : MonoBehaviour
    {
        public string DynamicLabel = "Dynamic label";

        public bool HideButton;

        public bool DisableButton;

        // Inline Buttons:
        [InlineButton("OnClick")]
        public int InlineButton;

        [InlineButton("OnClick")]
        [InlineButton("OnClick")]
        public int InlineButtons;

        // Button sizes:
        [Button(ButtonSizes.Small)]
        private void DefaultSizedButton() { OnClick(); }

        [Button(ButtonSizes.Medium)]
        private void MediumSizedButton() { OnClick(); }

        [Button(ButtonSizes.Large)]
        private void LargeSizedButton() { OnClick(); }

        [Button(ButtonSizes.Gigantic)]
        private void GiganticSizedButton() { OnClick(); }

        // Button Groups:
        [ButtonGroup("My Button Group")] //[ButtonGroup("My Button Group 1")] Will also work.
        private void A() { OnClick(); }

        [ButtonGroup("My Button Group")]
        private void B() { OnClick(); }

        [ButtonGroup("My Button Group")]
        private void C() { OnClick(); }

        // Dynamic buttons:
        [Button, GUIColor(0, 1, 0, 1)]
        private void ColoredButton() { OnClick(); }

        [Button("Custom Button Name")]
        private void NamedButton() { OnClick(); }

        [Button("$DynamicLabel")]
        private void DynamiclyNamedButton() { OnClick(); }

        [HideIf("HideButton")] // Or ShowIf
        [DisableIf("DisableButton")] // Or EnableIf
        [Button(ButtonSizes.Gigantic), GUIColor(0, 1, 0, 1)]
        private void ConditionalButton() { OnClick(); }

        // OnClick
        private void OnClick()
        {
            Debug.Log("Click");
        }
    }
}