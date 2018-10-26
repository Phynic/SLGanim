using System.Collections;
using System.Collections.Generic;
using SLG;
using UnityEngine;

public class SmokeBomb : Substitute, INinjaTool
{
    public SLG.Material Material { get; set; }
    public int Level { get; set; }
    public int ID { get; set; }
    public string Equipped { get; set; }
    
    public void RemoveSelf(Transform character)
    {
        var items = character.GetComponent<CharacterStatus>().items;
        items.Remove(items.Find(i => i.ID == ID));
    }

    //CharacterAction调用
    public void SetItem(ItemData itemData)
    {
        ID = itemData.ID;
        Material = itemData.itemMaterial;
        Level = itemData.itemLevel;
        Equipped = itemData.equipped;
        SetLevel(Level);
    }

    public override void SetLevel(int level)
    {
        
    }
    
}
