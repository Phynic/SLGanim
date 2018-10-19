namespace Sirenix.OdinInspector.Demos
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public class ValueDropdownExamples : MonoBehaviour
    {
        public GameObject[] AvailablePrefabs;

        [ValueDropdown("AvailablePrefabs")]
        public GameObject ActivePrefab;

        [ValueDropdown("GetLayers")]
        public string Layer;

        [ValueDropdown("textureSizes")]
        public int TextureSize;

        [ValueDropdown("friendlyTextureSizes")]
        public int FriendlyTextureSize;

#pragma warning disable 0414

        private static int[] textureSizes = new int[] { 256, 512, 1024 };

        private static ValueDropdownList<int> friendlyTextureSizes = new ValueDropdownList<int>()
        {
            { "Small", 256 },
            { "Medium", 512 },
            { "Large", 1024 },
        };

#pragma warning restore 0414

        private static List<string> GetLayers()
        {
            return Enumerable.Range(0, 32)
                .Select(i => LayerMask.LayerToName(i))
                .Where(s => s.Length > 0)
                .ToList();
        }
    }
}