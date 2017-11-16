using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeatBulletTank : AttackSkill {
    public override void SetLevel(int level)
    {
        damageFactor = 35 + level * 5;
    }
    public override bool Filter(Skill sender)
    {
        if (sender.EName == "Intumescence")
        {
            return base.Filter(sender);
        }
        return false;
    }
}
