using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.CompilerServices;

public class CharacterAction : MonoBehaviour {

    private Skill skill = null;
    
    public Skill GetSkill()
    {
        return skill;
    }

    public bool SetSkill(string skillName)
    {
        Type t = SkillManager.GetInstance().skillList.Find(skill => skill.EName == skillName).GetType();
        
        skill = Activator.CreateInstance(t) as Skill;
        if (skill is INinjaTool)
            return false;
        var pair = new KeyValuePair<Skill, Transform>(skill, transform);
        SkillManager.GetInstance().skillQueue.Enqueue(pair);
        GetComponent<Unit>().action.Push(skill);
        return true;
    }

    public bool SetItem(string itemName, int position)
    {
        Type t = SkillManager.GetInstance().skillList.Find(item => item.EName == itemName).GetType();

        skill = Activator.CreateInstance(t) as Skill;
        var pair = new KeyValuePair<Skill, Transform>(skill, transform);
        SkillManager.GetInstance().skillQueue.Enqueue(pair);
        GetComponent<Unit>().action.Push(skill);
        if(skill is INinjaTool)
        {
            var data = GetComponent<CharacterStatus>().items[position];
            ((INinjaTool)skill).SetItem(data);
        }
        return true;
    }

    public void Effect()
    {
        if(skill is UnitSkill)
        {
            var unitSkill = (UnitSkill)skill;
            //Debug.Log(unitSkill.CName);
            unitSkill.Effect();
        }
    }
}
