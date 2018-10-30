using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface INinjaTool {
    int Level { get; set; }
    int ID { get; set; }
    string Equipped { get; set; }
    void SetItem(ItemData itemData);
    void RemoveSelf(Transform character);
}
