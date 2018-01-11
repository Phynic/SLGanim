using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BanBuff : IBuff {
    
    public int Duration { get; set; }

    public BanBuff(int duration)
    {
        if (duration <= 0)
        {
            Duration = duration;
        }
        else
        {
            Duration = RoundManager.GetInstance().Players.Count * duration - 1;
        }
    }

    public void Apply(Transform character)
    {
        character.GetComponent<Unit>().UnitEnd = true;
        character.GetComponent<Unit>().Gray(true);
    }

    public IBuff Clone()
    {
        throw new NotImplementedException();
    }
    
    public void Undo(Transform character)
    {
        character.GetComponent<Unit>().Buffs.Remove(this);
    }
}
