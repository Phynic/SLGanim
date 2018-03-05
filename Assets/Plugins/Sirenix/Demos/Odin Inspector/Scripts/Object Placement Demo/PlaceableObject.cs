namespace Sirenix.OdinInspector.Demos
{
    using System;
    using UnityEngine;

    [Serializable]
    public class PlaceableObject : MonoBehaviour
    {
        public bool Enabled;

        [EnableIf("Enabled")]
        [Range(0, 1)]
        public float SpawnChance = 0.5f;

        [EnableIf("Enabled")]
        [LabelText("Scale")]
        [MinMaxSlider(0, 2)]
        public Vector2 MinMaxScaleSize = new Vector2(0.5f, 1.2f);
    }
}