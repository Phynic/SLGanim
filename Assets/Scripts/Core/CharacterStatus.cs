using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SLG;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class CharacterStatus : Unit {
    public string roleEName;                                     //人物名称。      
    public string roleCName;
    public CharacterIdentity characterIdentity = CharacterIdentity.noumenon;
    public enum CharacterIdentity
    {
        noumenon,               //本体
        clone,                  //分身
        transfiguration         //变身
    }


    public List<PrivateItemData> items = new List<PrivateItemData>();             //忍具列表
    public Dictionary<string, int> skills; //忍术列表<忍术名称，技能等级>

    public Vector3 arrowPosition = new Vector3(0, 1.1f, 0);
    public List<Skill> firstAction;                 //第一次行动列表
    public List<Skill> secondAction;                //第二次行动列表
    
    public string GetIdentity()
    {
        switch (characterIdentity)
        {
            case CharacterIdentity.noumenon:
                return "本体";
            case CharacterIdentity.clone:
                return "分身";
            case CharacterIdentity.transfiguration:
                return "变化";
            default:
                return "null";
        }
    }

    public override void Initialize()
    {
        base.Initialize();
        
        attributes = new List<Attribute>();

        var characterData = XMLManager.GetInstance().characterDB.characterDataList.Find(d => d.roleEName == roleEName);

        //序列化和反序列化进行深度复制。
        MemoryStream stream = new MemoryStream();
        BinaryFormatter formatter = new BinaryFormatter();
        formatter.Serialize(stream, characterData.attributes);
        stream.Position = 0;
        attributes = formatter.Deserialize(stream) as List<Attribute>;

        switch (characterIdentity)
        {
            case CharacterIdentity.noumenon:
                SetNoumenon();
                break;
            case CharacterIdentity.clone:
                SetClone();
                break;
            case CharacterIdentity.transfiguration:
                SetTransfiguration();
                break;
        }
    }

    public bool IsEnemy(CharacterStatus unit)
    {
        var identity = unit.characterIdentity;
        switch (identity)
        {
            case CharacterIdentity.noumenon:
                return playerNumber != unit.playerNumber;
            case CharacterIdentity.clone:
                return playerNumber != unit.playerNumber;
            case CharacterIdentity.transfiguration:
                if (playerNumber != unit.playerNumber)
                {
                    var buff = (TransfigurationBuff)unit.Buffs.Find(b => b.GetType() == typeof(TransfigurationBuff));
                    //buff中的目标不是自己人，则一定可以判定为敌人。
                    if (buff.target.GetComponent<Unit>().playerNumber != playerNumber)
                    {
                        return true;
                    }
                    //buff中的目标是自己人，则只有与该目标相同外观的角色能判定其为敌人。
                    else if (unit.GetComponent<Animator>().runtimeAnimatorController == GetComponent<Animator>().runtimeAnimatorController)
                    {
                        return true;
                    }
                }
                break;
        }
        return false;
    }

    public void SetNoumenon()
    {
        characterIdentity = CharacterIdentity.noumenon;

        firstAction = new List<Skill>();
        secondAction = new List<Skill>();
        skills = new Dictionary<string, int>();

        firstAction.Add(SkillManager.GetInstance().skillList.Find(s => s.EName == "Move"));
        firstAction.Add(SkillManager.GetInstance().skillList.Find(s => s.EName == "SkillOrToolList"));
        firstAction.Add(SkillManager.GetInstance().skillList.Find(s => s.EName == "RestoreChakra"));
        firstAction.Add(SkillManager.GetInstance().skillList.Find(s => s.EName == "Rest"));
        firstAction.Add(SkillManager.GetInstance().skillList.Find(s => s.EName == "EndRound"));

        secondAction.Add(SkillManager.GetInstance().skillList.Find(s => s.EName == "SkillOrToolList"));
        secondAction.Add(SkillManager.GetInstance().skillList.Find(s => s.EName == "EndRound"));


        var characterData = XMLManager.GetInstance().characterDB.characterDataList.Find(d => d.roleEName == roleEName);

        foreach (var data in characterData.skills)
        {
            skills.Add(data.skillName, data.skillLevel);
        }

        for(int i = 0; i < characterData.items.Count; i++)
        {
            characterData.items[i].itemPosition = i;
            items.Add(characterData.items[i]);
        }
    }

    public void SetClone()
    {
        characterIdentity = CharacterIdentity.clone;
        firstAction = new List<Skill>();
        secondAction = new List<Skill>();
        skills = new Dictionary<string, int>();

        firstAction.Add(SkillManager.GetInstance().skillList.Find(s => s.EName == "Move"));
        firstAction.Add(SkillManager.GetInstance().skillList.Find(s => s.EName == "EndRound"));
        
    }

    public void SetTransfiguration()
    {
        characterIdentity = CharacterIdentity.transfiguration;
        firstAction = new List<Skill>();
        secondAction = new List<Skill>();
        skills = new Dictionary<string, int>();

        firstAction.Add(SkillManager.GetInstance().skillList.Find(s => s.EName == "Move"));
        firstAction.Add(SkillManager.GetInstance().skillList.Find(s => s.EName == "RestoreChakra"));
        firstAction.Add(SkillManager.GetInstance().skillList.Find(s => s.EName == "Rest"));
        firstAction.Add(SkillManager.GetInstance().skillList.Find(s => s.EName == "EndRound"));

        skills.Add("NinjaCombo", 1);
    }
}
