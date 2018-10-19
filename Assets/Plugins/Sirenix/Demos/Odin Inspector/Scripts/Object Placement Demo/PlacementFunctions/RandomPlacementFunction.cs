namespace Sirenix.OdinInspector.Demos
{
    using UnityEngine;

    [System.Serializable]
    public class RandomPlacementFunction : ObjectPlacementFunction
    {
#pragma warning disable 1635, 0414

        private static ValueDropdownList<bool> areaTypes = new ValueDropdownList<bool>()
    {
        { "Circle", true },
        { "Square", false }
    };

#pragma warning restore 1635, 0414

        [LabelText("Area")]
        [ValueDropdown("areaTypes")]
        public bool isCircle = true;

        public override Vector3 GetPosition(float t)
        {
            if (this.isCircle)
            {
                Vector2 p = Random.insideUnitCircle;
                return new Vector3(p.x, 0, p.y);
            }
            else
            {
                return new Vector3(Random.value * 2 - 1, 0, Random.value * 2 - 1);
            }
        }
    }
}