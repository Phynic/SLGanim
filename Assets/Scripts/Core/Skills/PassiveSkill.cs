using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PassiveSkill : Skill
{
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
