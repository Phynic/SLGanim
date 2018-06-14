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
    
    public override void Effect()
    {
        RoundManager.GetInstance().Invoke(() => {
            FXManager.GetInstance().SmokeSpawn(character.position, character.rotation, null);
            render.SetActive(false);
        }, 0.6f);
        animator.speed = 0f;

        RoundManager.GetInstance().Invoke(() => {
            FXManager.GetInstance().SmokeSpawn(focus, character.rotation, null);
            animator.speed = 1f;
        }, 1.4f);
        RoundManager.GetInstance().Invoke(() => {
            character.position = focus;
            render.SetActive(true);
        }, 1.6f);
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
