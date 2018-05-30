using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eat : UnitSkill
{
    int restoreHP;
    int restoreMP;
    Transform shupian;
    public override void SetLevel(int level)
    {
        restoreHP = 350;
        restoreMP = 6;
    }

    public override List<string> LogSkillEffect()
    {
        string title = "恢复";
        string info = restoreHP + "/" + restoreMP;
        List<string> s = new List<string>
        {
            title,
            info
        };
        return s;
    }

    //protected override bool ApplyEffects()
    //{
    //    if (animator.GetInteger("Skill") == 0 && animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
    //    {
    //        character.GetComponent<CharacterAction>().SetSkill("ChooseDirection");

    //        return true;
    //    }
    //    return false;
    //}

    protected override void InitSkill()
    {
        shupian = animator.GetBoneTransform(HumanBodyBones.LeftHand).Find("Shupian");
        shupian.gameObject.SetActive(true);
        RoundManager.GetInstance().Invoke(() => {
            shupian.Find("mesh").gameObject.SetActive(true);
        }, 0.2f);
        
        base.InitSkill();
    }

    public override void Effect()
    {
        base.Effect();
        var currentHP = character.GetComponent<CharacterStatus>().attributes.Find(d => d.eName == "hp").value;
        var currentMP = character.GetComponent<CharacterStatus>().attributes.Find(d => d.eName == "mp").value;

        var hp = currentHP + restoreHP;
        var mp = currentMP + restoreMP;
        ChangeData.ChangeValue(character, "hp", hp);
        ChangeData.ChangeValue(character, "mp", mp);
        DebugLogPanel.GetInstance().Log("吃掉薯片，恢复了 " + restoreHP + "体力、" + restoreMP + "查克拉！");
        var skills = character.GetComponent<CharacterStatus>().skills;
        skills.Remove(EName);
        shupian.gameObject.SetActive(false);
    }
    

    public override void Complete()
    {
        base.Complete();
        
        
    }
}
