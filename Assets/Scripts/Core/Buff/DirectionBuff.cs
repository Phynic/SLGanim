using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionBuff : IBuff
{
    public int Duration { get; set; }

    public DirectionBuff()
    {
        Duration = 0;
    }

    public void Apply(Transform character)
    {
        throw new NotImplementedException();
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
