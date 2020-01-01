using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    protected override void ShowConfirm()
    {
        ConfirmView.GetInstance().Open(Reset, Confirm);
    }

    protected override void InitSkill()
    {
        shupian = animator.GetBoneTransform(HumanBodyBones.LeftHand).Find("Shupian");
        shupian.gameObject.SetActive(true);
        Utils_Coroutine.GetInstance().Invoke(() => {
            shupian.Find("mesh").gameObject.SetActive(true);
        }, 0.2f);
        
        base.InitSkill();
    }

    public override void Effect()
    {
        base.Effect();
        var hpAttribute = character.GetComponent<Unit>().attributes.Find(d => d.eName == "hp");
        var mpAttribute = character.GetComponent<Unit>().attributes.Find(d => d.eName == "mp");
        var currentHP = hpAttribute.Value;
        var currentMP = mpAttribute.Value;

        var hp = currentHP + restoreHP;
        var mp = currentMP + restoreMP;
        hpAttribute.ChangeValueTo(hp);
        mpAttribute.ChangeValueTo(mp);
        DebugLogPanel.GetInstance().Log("吃掉薯片，恢复了 " + restoreHP + "体力、" + restoreMP + "查克拉！");
        UIManager.GetInstance().FlyNum(character.GetComponent<Unit>().arrowPosition / 2 + character.position, restoreHP.ToString(), Utils_Color.hpColor);
        UIManager.GetInstance().FlyNum(character.GetComponent<Unit>().arrowPosition / 2 + character.position + Vector3.down * 0.3f, "+" + restoreMP.ToString(), new Color(80f / 255f, 248f / 255f, 144f / 255f));
        var skills = character.GetComponent<Unit>().skills;
        skills.Remove(SkillInfoID);
        shupian.gameObject.SetActive(false);
    }
    

    public override void Complete()
    {
        base.Complete();
    }
}
