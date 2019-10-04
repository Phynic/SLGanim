using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//兵粮丸
public class Hyourougan : AttackSkill, INinjaTool
{
    public int Level { get; set; }
    public int UniqueID { get; set; }

    public void RemoveSelf(Transform character)
    {
        var items = character.GetComponent<CharacterStatus>().items;
        items.Remove(UniqueID);
    }

    public void SetItem(ItemRecord itemData)
    {
        UniqueID = itemData.uniqueID;
        Level = itemData.level;
        SetLevel(Level);
    }

    public override void SetLevel(int level)
    {
        base.SetLevel(level);
        skillInfo.damage = skillInfo.factor;
    }

    public override bool Check()
    {
        var list = Detect.DetectObject(focus);

        foreach (var p in list)
        {
            if (p.GetComponent<CharacterStatus>())
            {
                if (!character.GetComponent<CharacterStatus>().IsEnemy(p.GetComponent<CharacterStatus>()))
                {
                    other.Add(p);
                }
            }
        }

        if (other.Count > 0)
        {
            return true;
        }
        return false;
    }
}
