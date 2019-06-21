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
            if (!(this is SmokeBomb))
                FXManager.GetInstance().Spawn("Stub", character.position, character.rotation, 4);
            //FXManager.GetInstance().StubSpawn(character.position, character.rotation, null);

            //这里延迟到下一帧，确保base部分执行完毕，render正常获取。
            Util_Coroutine.GetInstance().Invoke(() => { render.SetActive(false); }, 0.01f);
        }
        if (!base.Init(character))
            return false;
        return true;
    }

    public override void SetLevel(int level)
    {
        base.SetLevel(level);
        skillRange = factor;
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
        Util_Coroutine.GetInstance().Invoke(() => {
            character.position = focus;
            render.SetActive(true);
            var pair = SkillManager.GetInstance().skillQueue.Peek();
            ((UnitSkill)pair.Key).Complete();
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
