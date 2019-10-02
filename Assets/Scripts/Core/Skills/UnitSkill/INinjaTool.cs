using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface INinjaTool {
    int Level { get; set; }
    int UniqueID { get; set; }
    void SetItem(ItemRecord itemData);
    void RemoveSelf(Transform character);
}
