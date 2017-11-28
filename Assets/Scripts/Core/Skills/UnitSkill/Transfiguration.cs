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

    public override List<string> LogSkillEffect()
    {
        string title = "";
        string info = "";
        string durationInfo = duration.ToString();
        List<string> s = new List<string>
        {
            title,
            info,
            durationInfo
        };
        return s;
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
            
            animator.speed = 0f;

            RoundManager.GetInstance().Invoke(() => {
                FXManager.GetInstance().SmokeSpawn(character.position, character.rotation, null);
            }, 0.6f);

            RoundManager.GetInstance().Invoke(() => {
                
                var buff = new TransfigurationBuff(duration, target);
                character.GetComponent<CharacterStatus>().Buffs.Add(buff);
                buff.Apply(character);
                animator.speed = 1f;
            }, 0.8f);
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
