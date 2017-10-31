using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
//瞬身术
public class Telesport : UnitSkill
{
    GameObject render;
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
        Debug.Log("aa");
        GameObject.Destroy(smoke, 1.6f);
        render = character.Find("Render").gameObject;
        RoundManager.GetInstance().Invoke(() => { render.SetActive(false); }, 0.2f);
        animator.speed = 0f;

        RoundManager.GetInstance().Invoke(this, "EndEffect", 0.8f);
        RoundManager.GetInstance().Invoke(() => { character.position = focus; render.SetActive(true); }, 1f);
        base.Effect();
        
    }

    public void EndEffect()
    {
        var go = Resources.Load("Prefabs/Particle/Smoke");
        Debug.Log("end");
        animator.speed = 1f;
        var smoke1 = GameObject.Instantiate(go, focus, character.rotation) as GameObject;
        GameObject.Destroy(smoke1, 1.6f);
        
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
