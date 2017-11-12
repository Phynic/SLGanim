using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shuriken : AttackSkill, INinjaTool
{
    public SLG.Material material;
    public int level;
    public int position;

    public Shuriken(){

    }

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
        items.Remove(items.Find(i => i.itemPosition == position));
    }

    public void SetItem(PrivateItemData itemData)
    {
        material = itemData.itemMaterial;
        level = itemData.itemLevel;
        position = itemData.itemPosition;
    }

    public override void SetLevel(int level)
    {
        switch (material)
        {
            case SLG.Material.none:
                damageFactor = 10 + 5 * level;
                _cName = "手里剑";
                break;
            case SLG.Material.steel:
                damageFactor = 10 + 5 * level;
                hoverRange = 1;
                _cName = "八方手里剑";
                break;
            case SLG.Material.aluminum:
                damageFactor = 15;
                hit = 3 + level;
                _cName = "铝手里剑";
                break;
        }
    }

    protected override void InitSkill()
    {
        base.InitSkill();
    }

    protected override bool ApplyEffects()
    {
        RemoveSelf();
        return base.ApplyEffects();
    }
}
