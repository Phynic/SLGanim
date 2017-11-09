using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//倍化术
public class Intumescence : UnitSkill
{
    int factor = 30;
    FinalDamageBuff buff;
    public override bool Init(Transform character)
    {
        buff = new FinalDamageBuff(-1, factor);
        character.GetComponent<CharacterStatus>().Buffs.Add(buff);
        return base.Init(character);
    }

    public override void SetLevel(int level)
    {
        factor = 30 + level * 10;
    }

    protected override bool ApplyEffects()
    {
        throw new NotImplementedException();
    }

    public override void Effect()
    {
        base.Effect();
    }

    protected override void ResetSelf()
    {
        buff.Undo(character);
    }

    public override void Reset()
    {
        ResetSelf();
        base.Reset();
    }
}
