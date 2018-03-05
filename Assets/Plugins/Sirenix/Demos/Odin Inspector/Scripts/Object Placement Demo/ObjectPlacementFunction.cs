namespace Sirenix.OdinInspector.Demos
{
    using UnityEngine;

    public abstract class ObjectPlacementFunction
    {
        public abstract Vector3 GetPosition(float t);

        public virtual Vector3 GetTangent(float t)
        {
            float t1 = Mathf.Clamp01(t + 0.01f);
            float t2 = Mathf.Clamp01(t - 0.01f);

            if (t2 >= t1) t1 = t2 - 0.01f;
            if (t1 <= t2) t2 = t1 + 0.01f;

            var p1 = GetPosition(t1);
            var p2 = GetPosition(t2);

            return (p2 - p1).normalized;
        }

        public virtual Vector3 GetBinormal(float t)
        {
            var tangent = this.GetTangent(t);
            return new Vector3(-tangent.z, 0, tangent.x);
        }
    }
}