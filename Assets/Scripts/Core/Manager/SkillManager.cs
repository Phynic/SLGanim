using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class SkillManager : SingletonComponent<SkillManager>
{
    public Queue<KeyValuePair<Skill, Transform>> skillQueue = new Queue<KeyValuePair<Skill, Transform>>();              //技能执行队列
    public List<Skill> skillList = new List<Skill>();                   //所有技能

    public void Init()
    {
        skillList.Clear();

        foreach (var skillInfo in SkillInfoDictionary.GetParamList())
        {
            var skill = Activator.CreateInstance(Type.GetType(skillInfo.eName)) as Skill;
            skillList.Add(skill);
        }

        RoundManager.GetInstance().GameEnded += () => { Destroy(gameObject); };
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
