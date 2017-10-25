//using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NinjaCombo : AttackSkill {
    
    public override bool Init(Transform character)
    {
        if (!base.Init(character))
            return false;
        
        return true;
    }
    
    public override void SetLevel(int level)
    {
        damageFactor = 30 + level * 5;
    }

    protected override void InitSkill()
    {
        base.InitSkill();
    }
}
