namespace Sirenix.OdinInspector.Demos
{
    using UnityEngine;

    public class QuaternionAndVectorExamples : MonoBehaviour
    {
        [InfoBox("Quaternions now have a more friendly drawer with a nifty context menu, that can be customized in the Odin Preferences window.")]
        public Quaternion Quaternion;

        [InfoBox(
            "Vectors have an extra right-click context menu option to quickly set the vector to common values such as Left, Right, Zero, etc.."
            + "\nYou can also scale the magnitude of a vector, by dragging on it's respective label.")]
        public Vector3 Vector3;

        [InfoBox("And all vector fields are responsive, meaning that labels are removed when the inspector is narrow. This can be disabled in the Odin Preferences.")]
        public Bounds Bounds;

        [InfoBox("Bounds and rects are drawn using vectors, so all of the above functionality works here as well.")]
        public Rect Rect;
    }
}