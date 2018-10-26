using System;
using System.Collections;
using System.Collections.Generic;
using SLG;
using UnityEngine;

public class Shuriken : AttackSkill, INinjaTool
{
    public SLG.Material Material { get; set; }
    public int Level { get; set; }
    public int ID { get; set; }
    public string Equipped { get; set; }
    public override bool Init(Transform character)
    {
        SetLevel(Level);
        if (!base.Init(character))
            return false;

        return true;
    }

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
        switch (Material)
        {
            case SLG.Material.none:
                damageFactor = 6;
                _cName = "手里剑";
                break;
            case SLG.Material.steel:
                damageFactor = 6;
                hoverRange = 1;
                _cName = "八方手里剑";
                break;
            case SLG.Material.aluminum:
                damageFactor = 6;
                hit = 3 + level;
                _cName = "铝手里剑";
                break;
        }
    }
}
