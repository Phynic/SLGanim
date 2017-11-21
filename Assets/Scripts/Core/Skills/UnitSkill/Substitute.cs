using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//替身术
public class Substitute : UnitSkill
{

    public override bool Init(Transform character)
    {
        rotateToPathDirection = false;
        if (this.character == null)
        {
            FXManager.GetInstance().SmokeSpawn(character.position, character.rotation, null);
            RoundManager.GetInstance().Invoke(() => { render.SetActive(false); }, 0.2f);

        }


        if (!base.Init(character))
            return false;
        return true;
    }

    public override void SetLevel(int level)
    {
        skillRange = 1 + level;
    }

    public override List<string> LogSkillEffect()
    {
        string title = "";
        string info = "";
        string durationInfo = "1";
        List<string> s = new List<string>
        {
            title,
            info,
            durationInfo
        };
        return s;
    }

    protected override bool ApplyEffects()
    {
        
        FXManager.GetInstance().SmokeSpawn(focus, character.rotation, null);
        RoundManager.GetInstance().Invoke(() => {
            character.position = focus;
            render.SetActive(true);
        }, 0.2f);
        range.Delete();
        
        return true;
    }
    
    public override void Reset()
    {
        ResetSelf();
    }

    public override bool Check()
    {
        var list = Detect.DetectObject(focus);
        foreach(var p in list)
        {
            if (p.GetComponent<Unit>())
            {
                if (p.Find("Render").gameObject.activeInHierarchy)
                {
                    return false;
                }
            }
        }
        return true;
    }
}
