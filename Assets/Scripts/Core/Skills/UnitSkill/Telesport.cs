using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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

        FXManager.GetInstance().SmokeSpawn(character.position,character.rotation,null);
        
        RoundManager.GetInstance().Invoke(() => { render.SetActive(false); }, 0.2f);
        animator.speed = 0f;

        RoundManager.GetInstance().Invoke(() => {
            FXManager.GetInstance().SmokeSpawn(focus, character.rotation, null);
            animator.speed = 1f;
        }, 0.8f);
        RoundManager.GetInstance().Invoke(() => {
            character.position = focus;
            render.SetActive(true);
        }, 1f);
        base.Effect();
        
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
