using System;
using UnityEngine;
using System.Collections.Generic;

public class SkillManager : MonoBehaviour {
    private static SkillManager instance;
    
    public Queue<KeyValuePair<Skill, Transform>> skillQueue = new Queue<KeyValuePair<Skill, Transform>>();              //技能执行队列
    public List<Skill> skillList = new List<Skill>();                   //所有技能
    
    public static SkillManager GetInstance()
    {
        return instance;
    }

    void Awake()
    {
        instance = this;

        foreach(var s in XMLManager.GetInstance().gameDB.skillDataList)
        {
            var skill = Activator.CreateInstance(Type.GetType(s.eName)) as Skill;
            skillList.Add(skill);
        }

        //skillList.Add(new Move());
        //skillList.Add(new SkillOrToolList());
        //skillList.Add(new RestoreChakra());
        //skillList.Add(new Rest());
        //skillList.Add(new EndRound());
        //skillList.Add(new ChooseDirection());
        //skillList.Add(new FirstAction());
        //skillList.Add(new SecondAction());
        //skillList.Add(new ChooseTrick());
        //skillList.Add(new NinjaCombo());
        //skillList.Add(new NinjaCombo1());
        //skillList.Add(new Substitute());
        //skillList.Add(new Telesport());
        //skillList.Add(new Clone());
        //skillList.Add(new Transfiguration());
        //skillList.Add(new MagicShuriken());
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
