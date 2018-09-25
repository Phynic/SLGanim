using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shuriken : AttackSkill, INinjaTool
{
    public SLG.Material material;
    public int level;
    public int ID;
    public override bool Init(Transform character)
    {
        SetLevel(level);
        if (!base.Init(character))
            return false;

        return true;
    }

    public void RemoveSelf()
    {
        var items = character.GetComponent<CharacterStatus>().items;
        items.Remove(items.Find(i => i.ID == ID));
    }

    //CharacterAction调用
    public void SetItem(ItemData itemData)
    {
        ID = itemData.ID;
        material = itemData.itemMaterial;
        level = itemData.itemLevel;
        SetLevel(level);
    }

    public override void SetLevel(int level)
    {
        switch (material)
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
    
    protected override bool ApplyEffects()
    {
        RemoveSelf();
        return base.ApplyEffects();
    }
}
