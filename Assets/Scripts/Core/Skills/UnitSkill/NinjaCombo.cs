//using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NinjaCombo : AttackSkill {

    public override void SetLevel(int level)
    {
        base.SetLevel(level);
        damage = factor;
    }

    public override void Effect()
    {
        animator.speed = 0;
        
        Util_Coroutine.GetInstance().Invoke(() => {
            animator.speed = 1;
            base.Effect();
        }, 0.1f);
    }
}
