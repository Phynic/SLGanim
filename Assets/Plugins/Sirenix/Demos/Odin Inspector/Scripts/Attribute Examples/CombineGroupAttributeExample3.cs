namespace Sirenix.OdinInspector.Demos
{
    using UnityEngine;
    using Sirenix.OdinInspector;

    // A perhaps extreme example showing you that the same property can exist in multiple groups.
    public class CombineGroupAttributeExample3 : MonoBehaviour
    {
        [Range(0, 10)]
        [LabelWidth(20)]
        [HorizontalGroup("Split", 0.5f)]
        [FoldoutGroup("Split/Left")]
        [FoldoutGroup("Split/Right")]
        [BoxGroup("Box")]
        [BoxGroup("Split/Left/Box")]
        [BoxGroup("Split/Right/Box")]
        public float A;

        [FoldoutGroup("Split/Right")]
        [FoldoutGroup("Split/Left")]
        private void Button()
        {
        }
    }
}