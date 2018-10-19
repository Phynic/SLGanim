namespace Sirenix.OdinInspector.Demos
{
    using UnityEngine;
    using Sirenix.OdinInspector;

    public class CombineGroupAttributeExample1 : MonoBehaviour
    {
        [HorizontalGroup("Split", width: 0.4f)]
        [BoxGroup("Split/Left")]
        public int[] A;

        [BoxGroup("Split/Left")]
        public int[] C;

        [BoxGroup("Split/Center")]
        public int[] B;

        [BoxGroup("Split/Center")]
        public int[] D;

        [HorizontalGroup("Split", width: 0.4f)]
        [BoxGroup("Split/Right")]
        public int[] E;

        [BoxGroup("Split/Right")]
        public int[] F;
    }
}