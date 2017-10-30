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
        var go = Resources.Load("Prefabs/Particle/Smoke");
        var smoke = GameObject.Instantiate(go, character.position, character.rotation) as GameObject;
        
        var smoke1 = GameObject.Instantiate(go, focus, character.rotation) as GameObject;

        GameObject.Destroy(smoke, 1.6f);
        GameObject.Destroy(smoke1, 1.6f);
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
