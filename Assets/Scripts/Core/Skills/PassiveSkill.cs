using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PassiveSkill : Skill
{
    public string description;
    public int level;
    public int factor;
    public PassiveSkill()
    {
        var passiveSkillData = (PassiveSkillData)skillData;
        description = passiveSkillData.description;
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
