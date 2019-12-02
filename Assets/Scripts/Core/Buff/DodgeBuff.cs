using UnityEngine;

public class DodgeBuff : IBuff {

    string _dodgeName;
    public ItemRecord itemRecord = null;
    public bool done = false;
    public DodgeBuff(int duration, string dodgeName)
    {
        if(duration <= 0)
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
        if(itemRecord != null)
        {
            if(!CA.SetItem(_dodgeName, itemRecord))
            {
                Debug.Log("Set item False");
            }
            done = true;
        }
        else
        {
            if (!CA.SetSkill(_dodgeName))
            {
                Debug.Log("Set Skill False");
            }
            else
            {
                var costMP = ((UnitSkill)SkillManager.GetInstance().skillList.Find(s => s.EName == _dodgeName)).skillInfo.costMP;
                character.GetComponent<Unit>().attributes.Find(d => d.eName == "mp").PlusValue(-costMP);
                done = true;
            }
        }
    }

    public IBuff Clone()
    {
        return new DodgeBuff(Duration, _dodgeName);
    }
    
    public void Undo(Transform character)
    {
        //buff统一管理部分会调用undo，之后会自动remove。
        if (!done)
        {
            var costMP = ((UnitSkill)SkillManager.GetInstance().skillList.Find(s => s.EName == _dodgeName)).skillInfo.costMP;
            character.GetComponent<Unit>().attributes.Find(d => d.eName == "mp").PlusValue(-costMP);
        }

    }
}
