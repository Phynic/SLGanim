using UnityEngine;
using System.Collections.Generic;
using System;


//技能是HumanPlayer的入口，而AIPlayer直接调用各个分组件进行技能实现。
public abstract class Skill {
    protected string _cName;
    protected string _eName;
    public Transform character;
    public string CName { get { return _cName; } }
    public string EName { get { return _eName; } }
    public int Id { get; protected set; }
    protected SkillData skillData;
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
        skillData = XMLManager.GetInstance().gameDB.skillDataList.Find(d => d.eName == GetType().ToString());
        
        _eName = skillData.eName;
        _cName = skillData.cName;
    }


    public abstract bool Init(Transform character);     //初始化技能
    public abstract bool OnUpdate(Transform character);                    //每帧执行函数
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

    

    public abstract bool Check();                    //检查技能执行条件，由技能内部决定何时调用

    
    
}
