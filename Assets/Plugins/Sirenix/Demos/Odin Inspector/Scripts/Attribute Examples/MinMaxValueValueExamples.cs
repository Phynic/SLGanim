namespace Sirenix.OdinInspector.Demos
{
    using UnityEngine;

    public class MinMaxValueValueExamples : MonoBehaviour
    {
        // Bytes
        [Header("Bytes")]
        [Indent]
        [MinValue(100)]
        public byte ByteMinValue100;

        [Indent]
        [MaxValue(100)]
        public byte ByteMaxValue100;

        [Indent]
        [MinValue(0)]
        public sbyte SbyteMinValue0;

        [Indent]
        [MaxValue(0)]
        public sbyte SbyteMaxValue0;

        // Shorts
        [Header("Int 16")]
        [Indent]
        [MinValue(0)]
        public short ShortMinValue0;

        [Indent]
        [MaxValue(0)]
        public short ShortMaxValue0;

        [Indent]
        [MinValue(100)]
        public ushort UshortMinValue100;

        [Indent]
        [MaxValue(100)]
        public ushort UshortMaxValue100;

        // Ints
        [Header("Int 32")]
        [Indent]
        [MinValue(0)]
        public int IntMinValue0;

        [Indent]
        [MaxValue(0)]
        public int IntMaxValue0;

        [Indent]
        [MinValue(100)]
        public uint UintMinValue100;

        [Indent]
        [MaxValue(100)]
        public uint UintMaxValue100;

        // Longs
        [Header("Int 64")]
        [Indent]
        [MinValue(0)]
        public long LongMinValue0;

        [Indent]
        [MaxValue(0)]
        public long LongMaxValue0;

        [Indent]
        [MinValue(100)]
        public ulong UlongMinValue100;

        [Indent]
        [MaxValue(100)]
        public ulong UlongMaxValue100;

        // Floats
        [Header("Float")]
        [Indent]
        [MinValue(0)]
        public float FloatMinValue0;

        [Indent]
        [MaxValue(0)]
        public float FloatMaxValue0;

        [Indent]
        [MinValue(0)]
        public double DoubleMinValue0;

        [Indent]
        [MaxValue(0)]
        public double DoubleMaxValue0;

        // Decimal
        [Header("Decimal")]
        [Indent]
        [MinValue(0)]
        public decimal DecimalMinValue0;

        [Indent]
        [MaxValue(0)]
        public decimal DecimalMaxValue0;

        // Vectors
        [Header("Vectors")]
        [Indent]
        [MinValue(0)]
        public Vector2 Vector2MinValue0;

        [Indent]
        [MaxValue(0)]
        public Vector2 Vector2MaxValue0;

        [Indent]
        [MinValue(0)]
        public Vector3 Vector3MinValue0;

        [Indent]
        [MaxValue(0)]
        public Vector3 Vector3MaxValue0;

        [Indent]
        [MinValue(0)]
        public Vector4 Vector4MinValue0;

        [Indent]
        [MaxValue(0)]
        public Vector4 Vector4MaxValue0;
    }
}