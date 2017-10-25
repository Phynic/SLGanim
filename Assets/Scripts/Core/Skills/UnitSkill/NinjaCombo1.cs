using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NinjaCombo1 : AttackSkill
{
    
    public override bool Init(Transform character)
    {
        SetLevel(character.GetComponent<CharacterStatus>().skills[_eName]);

        if (!base.Init(character))
            return false;

        return true;
    }

    public override void SetLevel(int level)
    {
        damageFactor = 10 + level * 5;
    }

    protected override void InitSkill()
    {
        base.InitSkill();
    }
}
