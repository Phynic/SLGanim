using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DodgeBuff : Buff {

    string _dodgeName;

    public DodgeBuff(int duration, string dodgeName)
    {
        if(duration == 0)
        {
            Duration = duration;
        }
        else
        {
            Duration = RoundManager.GetInstance().Players.Count * duration - 1;
        }
        _dodgeName = dodgeName;
    }

    public int Duration { get; set; }

    public void Apply(Transform character)
    {
        var CA = character.GetComponent<CharacterAction>();
        if (!CA.SetSkill(_dodgeName))
        {
            Debug.Log("Set Skill False");
        }
        else
        {
            Undo(character);
        }
    }

    public Buff Clone()
    {
        return new DodgeBuff(Duration, _dodgeName);
    }

    public void Undo(Transform character)
    {
        var currentMP = character.GetComponent<CharacterStatus>().attributes.Find(d => d.eName == "mp").value;
        var costMP = ((UnitSkill)SkillManager.GetInstance().skillList.Find(s => s.EName == _dodgeName)).costMP;
        var mp = currentMP - costMP;

        ChangeData.ChangeValue(character, "mp", mp);
        character.GetComponent<Unit>().Buffs.Remove(this);
    }
}
