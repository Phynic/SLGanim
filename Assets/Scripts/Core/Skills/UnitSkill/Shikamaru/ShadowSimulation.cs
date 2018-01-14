using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowSimulation : UnitSkill {
    public int duration;

    public List<Transform> other = new List<Transform>();
    public override void SetLevel(int level)
    {
        duration = level + 2;
    }
    
    public override void Effect()
    {
        foreach(var o in other)
        {
            var banBuff = new BanBuff(duration);
            o.GetComponent<Unit>().Buffs.Add(banBuff);
            banBuff.Apply(o);
        }
        var buff = new BanBuff(duration);
        character.GetComponent<Unit>().Buffs.Add(buff);
        buff.Apply(character);

        base.Effect();
    }
    
    public override List<string> LogSkillEffect()
    {
        string title = "";
        string info = "";
        string durationInfo = duration.ToString();
        List<string> s = new List<string>
        {
            title,
            info,
            durationInfo
        };
        return s;
    }

    public override bool Check()
    {
        other.Clear();

        var list = Detect.DetectObjects(hoverRange, focus);

        foreach (var l in list)
        {
            foreach (var u in l)
            {
                if (u.GetComponent<CharacterStatus>())
                {
                    if (character.GetComponent<CharacterStatus>().IsEnemy(u.GetComponent<CharacterStatus>()))
                    {
                        other.Add(u);
                    }
                }
            }
        }
        //other = other.Distinct().ToList();
        if (other.Count > 0)
        {
            return true;
        }
        return false;
    }
}
