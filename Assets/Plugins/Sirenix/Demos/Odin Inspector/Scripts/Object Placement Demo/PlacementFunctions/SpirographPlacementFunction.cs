namespace Sirenix.OdinInspector.Demos
{
    using System;
    using UnityEngine;

    [Serializable]
    public class SpirographPlacementFunction : ObjectPlacementFunction
    {
        [Range(0, 1)]
        public float Arc = 1;

        [Range(0, 12)]
        public float Arms = 4;

        public override Vector3 GetPosition(float t)
        {
            float theta = t * Mathf.PI * 2 * this.Arc;
            return new Vector3(Mathf.Cos(theta), 0, Mathf.Sin(theta)) * (1 - (Mathf.Cos(theta * this.Arms) * 0.5f + 0.5f));
        }
    }
}