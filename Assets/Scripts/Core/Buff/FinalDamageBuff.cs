using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalDamageBuff : Buff
{
    public int Factor { get; private set; }

    public FinalDamageBuff(int duration, int factor)
    {
        if (duration <= 0)
        {
            Duration = duration;
        }
        else
        {
            Duration = RoundManager.GetInstance().Players.Count * duration - 1;
        }

        Factor = factor;
    }
    
    public int Duration { get; set; }

    public void Apply(Transform character)
    {
        throw new NotImplementedException();
    }

    public Buff Clone()
    {
        throw new NotImplementedException();
    }

    public void Undo(Transform character)
    {
        character.GetComponent<Unit>().Buffs.Remove(this);
    }
}
