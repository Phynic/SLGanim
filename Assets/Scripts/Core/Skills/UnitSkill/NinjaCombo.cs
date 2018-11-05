//using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NinjaCombo : AttackSkill {
    
    public override void SetLevel(int level)
    {
        factor = factor + (level - 1) * (int)growFactor;
    }

    public override void Effect()
    {
        animator.speed = 0;
        
        GameController.GetInstance().Invoke(() => {
            animator.speed = 1;
            base.Effect();
        }, 0.1f);
    }
}
