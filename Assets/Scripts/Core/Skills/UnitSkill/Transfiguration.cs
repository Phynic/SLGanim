using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//变化术
public class Transfiguration : UnitSkill
{
    public int duration;
    private Transform target;

    public override void SetLevel(int level)
    {
        duration = 2 + level;
    }

    protected override void InitSkill()
    {
        base.InitSkill();
    }

    protected override bool ApplyEffects()
    {
        if (animator.GetInteger("Skill") == 0 && animator.GetAnimatorTransitionInfo(0).IsName("DefaultSkill -> Exit"))
        {
            character.GetComponent<CharacterAction>().SetSkill("ChooseDirection");
            return true;
        }
        return false;
    }

    public override void Effect()
    {
        base.Effect();
        if (target.GetComponent<CharacterStatus>().characterIdentity == CharacterStatus.CharacterIdentity.noumenon)
        {
            var buff = new TransfigurationBuff(duration, target);
            character.GetComponent<CharacterStatus>().Buffs.Add(buff);
            buff.Apply(character);
        }
        else
        {
            DebugLogPanel.GetInstance().Log("Miss");
        }
    }

    public override bool Check()
    {
        var list = Detect.DetectObject(focus);

        foreach (var p in list)
        {
            if (p.GetComponent<Unit>() && character.GetComponent<Unit>() != p.GetComponent<Unit>())
            {
                target = p;
                return true;
            }
        }
        return false;
    }
}
