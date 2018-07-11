using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PassiveSkill : Skill
{
    public int level;
    public int factor;
    public PassiveSkill()
    {

    }

    public void SetLevel(int level)
    {
        this.level = level;
    }

    public override bool Check()
    {
        throw new NotImplementedException();
    }

    public override bool Init(Transform character)
    {
        throw new NotImplementedException();
    }

    public override bool OnUpdate(Transform character)
    {
        throw new NotImplementedException();
    }
}
