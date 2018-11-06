using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//兵粮丸
public class Hyourougan : AttackSkill, INinjaTool
{
    public int Level { get; set; }
    public int ID { get; set; }
    public string Equipped { get; set; }

    public void RemoveSelf(Transform character)
    {
        var items = character.GetComponent<CharacterStatus>().items;
        items.Remove(items.Find(i => i.ID == ID));
    }

    public void SetItem(ItemData itemData)
    {
        ID = itemData.ID;
        Level = itemData.itemLevel;
        Equipped = itemData.equipped;
        SetLevel(Level);
    }

    public override void SetLevel(int level)
    {
        base.SetLevel(level);
        damage = factor;
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
