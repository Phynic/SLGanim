using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//瞬身术
public class Telesport : UnitSkill
{
    
    public override void SetLevel(int level)
    {
        skillRange = 2 + level;
    }

    protected override void InitSkill()
    {
        base.InitSkill();
    }

    protected override bool ApplyEffects()
    {
        if (animator.GetInteger("Skill") == 0 && animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            character.GetComponent<CharacterAction>().SetSkill("ChooseDirection");
            return true;
        }
        return false;
    }

    public override void Effect()
    {
        base.Effect();
        character.position = focus;
    }

    public override bool Check()
    {
        var list = Detect.DetectObject(focus);
        foreach (var p in list)
        {
            if (p.GetComponent<Unit>())
            {
                return false;
            }
        }
        return true;
    }
}
