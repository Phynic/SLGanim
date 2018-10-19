namespace Sirenix.OdinInspector.Demos
{
    using UnityEngine;

    public class WrapExamples : MonoBehaviour
    {
        [Header("Angle and radian")]
        [Indent]
        [Wrap(0f, 360)]
        public float AngleWrap;

        [Indent]
        [Wrap(0f, Mathf.PI * 2)]
        public float RadianWrap;

        [Header("Type tests")]
        [Indent]
        [Wrap(0f, 100f)]
        public short ShortWrapFrom0To100;

        [Indent]
        [Wrap(0f, 100f)]
        public int IntWrapFrom0To100;

        [Indent]
        [Wrap(0f, 100f)]
        public long LongWrapFrom0To100;

        [Indent]
        [Wrap(0f, 100f)]
        public float FloatWrapFrom0To100;

        [Indent]
        [Wrap(0f, 100f)]
        public double DoubleWrapFrom0To100;

        [Indent]
        [Wrap(0f, 100f)]
        public decimal DecimalWrapFrom0To100;

        [Header("Vectors")]
        [Indent]
        [Wrap(0f, 100f)]
        public Vector2 Vector2WrapFrom0To100;

        [Indent]
        [Wrap(0f, 100f)]
        public Vector3 Vector3WrapFrom0To100;

        [Indent]
        [Wrap(0f, 100f)]
        public Vector4 Vector4WrapFrom0To100;
    }
}