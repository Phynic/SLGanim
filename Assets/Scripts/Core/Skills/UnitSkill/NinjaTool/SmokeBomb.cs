using System.Collections;
using System.Collections.Generic;
using SLG;
using UnityEngine;

//烟弹
public class SmokeBomb : Substitute, INinjaTool
{
    public int Level { get; set; }
    public int UniqueID { get; set; }
    
    public void RemoveSelf(Transform character)
    {
        var items = character.GetComponent<CharacterStatus>().items;
        items.Remove(items.Find(i => i.uniqueID == UniqueID));
    }

    //CharacterAction调用
    public void SetItem(ItemRecord itemRecord)
    {
        UniqueID = itemRecord.uniqueID;
        Level = itemRecord.level;
        SetLevel(Level);
    }
}
