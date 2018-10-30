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
        duration = 3 + (level - 1) * (int)growFactor;
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
    
    public override void Effect()
    {
        base.Effect();
        
        if (target.GetComponent<CharacterStatus>().characterIdentity == CharacterStatus.CharacterIdentity.noumenon)
        {
            
            animator.speed = 0f;

            GameController.GetInstance().Invoke(() => {
                FXManager.GetInstance().SmokeSpawn(character.position, character.rotation, null);
            }, 0.6f);

            GameController.GetInstance().Invoke(() => {
                
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
