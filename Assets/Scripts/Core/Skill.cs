using UnityEngine;
using System.Collections.Generic;
using System;

//技能是HumanPlayer的入口，而AIPlayer直接调用各个分组件进行技能实现。
public abstract class Skill
{
    public Transform character;
    public SkillInfo skillInfo;
    public string CName { get { return skillInfo.cName; } }
    public string EName { get { return skillInfo.eName; } }
    public int SkillInfoID { get { return skillInfo.ID; } }
    public bool isAI
    {
        get
        {
            var player = RoundManager.GetInstance().Players.Find(p => p.playerNumber == SkillManager.GetInstance().skillQueue.Peek().Value.GetComponent<CharacterStatus>().playerNumber);

            if (player is AIPlayer && ((AIPlayer)player).AIControl)
                return true;
            else
                return false;
        }
        private set { }
    }

    //结束输入，但技能效果并未完结。
    public bool done = false;

    public enum SkillState
    {
        init,
        waitForInput,   //每帧执行。
        confirm,        //单次执行后跳转。
        applyEffect,    //每帧执行。
        reset
    }
    public SkillState skillState = SkillState.init;

    public Skill()
    {
        var skillID = SkillInfoDictionary.GetParamList().Find(i => i.eName == GetType().ToString()).ID;
        skillInfo = SkillInfoDictionary.GetNewParam(skillID);
    }

    public abstract bool Init(Transform character);     //初始化技能
    public abstract bool OnUpdate(Transform character); //每帧执行函数

    public virtual void SetLevel(int level)
    {
        skillInfo.factor = skillInfo.factor + (level - 1) * skillInfo.growFactor;
    }

    public virtual void Reset()
    {
        //回退至上一个技能。
        if (SkillManager.GetInstance().skillQueue.Count == 1)
        {
            character.GetComponent<Unit>().action.Pop();
            character.GetComponent<CharacterAction>().SetSkill(character.GetComponent<Unit>().action.Pop().EName);
            skillState = SkillState.reset;
        }
        else
        {
            Debug.LogWarning("队列长度 ： " + SkillManager.GetInstance().skillQueue.Count);
            foreach (var a in SkillManager.GetInstance().skillQueue)
            {
                Debug.LogWarning(a.Key.CName);
            }
        }
    }

    public abstract bool Check();   //检查技能执行条件，由技能内部决定何时调用

}
