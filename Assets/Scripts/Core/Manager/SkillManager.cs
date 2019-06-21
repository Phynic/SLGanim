using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class SkillManager : Singleton<SkillManager>
{
    public Queue<KeyValuePair<Skill, Transform>> skillQueue = new Queue<KeyValuePair<Skill, Transform>>();              //技能执行队列
    public List<Skill> skillList = new List<Skill>();                   //所有技能
    
    private void Start()
    {
        Util_Coroutine.GetInstance().Invoke(() => { InitSkillList(); }, 0.1f);
    }
    
    private void InitSkillList()
    {
        foreach (var s in Global.GetInstance().gameDB.skillDataList)
        {
            var skill = Activator.CreateInstance(Type.GetType(s.eName)) as Skill;
            skillList.Add(skill);
        }
    }

    void Update()
    {
        if (skillQueue.Count == 0)
            return;

        //Debug.Log(skillQueue.Peek().Key.CName + " by " + skillQueue.Peek().Value.name + "     队列容量： " + skillQueue.Count);

        if (skillQueue.Peek().Key.OnUpdate(skillQueue.Peek().Value))
        {
            //Debug.Log("已完成的技能: " + skillQueue.Peek().Key.CName);
            skillQueue.Dequeue();
            return;
        }
    }
}
