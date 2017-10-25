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
        if (!base.Init(character))
            return false;
        return true;
    }

    public override void SetLevel(int level)
    {
        skillRange = 1 + level;
    }

    protected override bool ApplyEffects()
    {
        character.position = focus;
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
                return false;
            }
        }
        return true;
    }
}
