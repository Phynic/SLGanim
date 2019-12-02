using System;
using System.Collections;
using System.Collections.Generic;
using SLG;
using UnityEngine;

public class Shuriken : AttackSkill, INinjaTool
{
    public int Level { get; set; }
    public int UniqueID { get; set; }
    
    public void RemoveSelf(Transform character)
    {
        var items = character.GetComponent<Unit>().items;
        items.Remove(UniqueID);
    }

    //CharacterAction调用
    public void SetItem(ItemRecord itemRecord)
    {
        UniqueID = itemRecord.uniqueID;
        Level = itemRecord.level;
        SetLevel(Level);
    }
}

public class SteelShuriken : Shuriken
{
    public override void SetLevel(int level)
    {
        base.SetLevel(level);
        skillInfo.damage = skillInfo.factor;
        switch (level)
        {
            case 2:
                skillInfo.eName = "强力八方手里剑";
                break;
            case 3:
                skillInfo.eName = "超八方手里剑";
                break;
        }
    }
}

public class AluminumShuriken : Shuriken
{
    public override void SetLevel(int level)
    {
        base.SetLevel(level);
        skillInfo.hit = skillInfo.factor;
        switch (level)
        {
            case 2:
                skillInfo.eName = "强力铝手里剑";
                break;
            case 3:
                skillInfo.eName = "超铝手里剑";
                break;
        }
    }
}
