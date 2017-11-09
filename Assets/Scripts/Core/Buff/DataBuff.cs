using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataBuff : Buff
{
    string _dataName;
    int _factor;

    public DataBuff(int duration, string dataName, int factor)
    {
        if (duration <= 0)
        {
            Duration = duration;
        }
        else
        {
            Duration = RoundManager.GetInstance().Players.Count * duration - 1;
        }
        _dataName = dataName;
        _factor = factor;
    }

    public int Duration { get; set; }

    public void Apply(Transform character)
    {
        character.GetComponent<CharacterStatus>().attributes.Find(d => d.eName == _dataName).value += _factor;
    }

    public void Undo(Transform character)
    {
        character.GetComponent<CharacterStatus>().attributes.Find(d => d.eName == _dataName).value -= _factor;
        character.GetComponent<Unit>().Buffs.Remove(this);
        Debug.Log(_dataName);
    }

    public Buff Clone()
    {
        throw new NotImplementedException();
    }
}
