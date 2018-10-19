namespace Sirenix.OdinInspector.Demos
{
    using System;
    using UnityEngine;

    [Serializable]
    public class SineWavePlacementFunction : ObjectPlacementFunction
    {
        [Range(0, 6)]
        public float Frequency = 2;

        [Range(0, 1f)]
        public float Amplitude = 0.3f;

        [Wrap(0, 2)]
        public float Offset;

        [Wrap(0, 360)]
        public float Angle;

        public override Vector3 GetPosition(float t)
        {
            var angle = this.Angle * Mathf.Deg2Rad;
            var dir = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));
            var normal = new Vector3(-dir.z, 0, dir.x);
            return dir * (t * 2 - 1) + normal * Mathf.Sin((t + this.Offset) * Mathf.PI * 2 * this.Frequency) * this.Amplitude;
        }
    }
}