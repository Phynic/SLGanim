namespace Sirenix.OdinInspector.Demos
{
    using System;
    using UnityEngine;

    [Serializable]
    public class CirclePlacementFunction : ObjectPlacementFunction
    {
        [Range(0, 1)]
        public float Arc = 1;

        public override Vector3 GetPosition(float t)
        {
            float theta = t * Mathf.PI * 2 * this.Arc;
            return new Vector3(Mathf.Cos(theta), 0, Mathf.Sin(theta));
        }
    }
}