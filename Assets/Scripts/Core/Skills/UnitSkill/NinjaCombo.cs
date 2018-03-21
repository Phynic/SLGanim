//using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NinjaCombo : AttackSkill {
    
    public override void SetLevel(int level)
    {
        damageFactor = damageFactor + (level - 1) * 5;
    }

    public override void Effect()
    {
        animator.speed = 0;
        
        RoundManager.GetInstance().Invoke(() => {
            animator.speed = 1;
            base.Effect();
        }, 0.1f);
    }
}
