using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SLG;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class CharacterStatus : Unit {
    public CharacterInfo characterInfo;
    public int characterInfoID;
    public string roleEName;        //人物名称。      
    public string roleCName;
    public CharacterIdentity characterIdentity = CharacterIdentity.noumenon;
    public string identity = "本体";
    public string state;
    public int killExp = 100;       //击杀经验
    public int bonusExp = 0;        //人头奖励
    public enum CharacterIdentity
    {
        noumenon,               //本体
        clone,                  //分身
        advanceClone,           //高级分身
        beastClone,             //赤丸
        obstacle                //障碍
    }

    //战斗场景中只读使用。
    /// <summary>
    /// <uniqueID, ItemRecord>
    /// </summary>
    public Dictionary<int, ItemRecord> itemCharacterRecords = new Dictionary<int, ItemRecord>();
    //public List<ItemCharacterRecord> itemRecords = new List<ItemCharacterRecord>();             //忍具列表
    public Dictionary<int, int> skills; //忍术列表<忍术ID，技能等级>

    public Vector3 arrowPosition = new Vector3(0, 1.1f, 0);
    public List<Skill> firstAction;                 //第一次行动列表
    public List<Skill> secondAction;                //第二次行动列表
    
    public override void Initialize()
    {
        base.Initialize();
        
        var attributeInfoList = AttributeInfoDictionary.GetParamList();
        characterInfo = CharacterInfoDictionary.GetParamList().Find(d => d.roleEName == roleEName);
        characterInfoID = characterInfo.ID;
        foreach (var info in attributeInfoList)
        {
            attributes.Add(new SLG.Attribute(info.ID));
        }

        foreach (var attribute in attributes)
        {
            attribute.ChangeValueTo((int)characterInfo.GetType().GetField(attribute.eName).GetValue(attribute));
        }

        switch (characterIdentity)
        {
            case CharacterIdentity.noumenon:
                SetNoumenon();
                break;
            case CharacterIdentity.obstacle:
                SetObstacle();
                break;
        }
    }
    
    public bool IsEnemy(CharacterStatus unit)
    {
        var identity = unit.characterIdentity;
        switch (identity)
        {
            
            default:
                return playerNumber != unit.playerNumber;
        }
    }

    public void SetNoumenon()
    {
        characterIdentity = CharacterIdentity.noumenon;
        identity = "本体";
        rend = GetComponentsInChildren<Renderer>();
        firstAction = new List<Skill>();
        secondAction = new List<Skill>();
        skills = new Dictionary<int, int>();

        firstAction.Add(SkillManager.GetInstance().skillList.Find(s => s.EName == "Move"));
        firstAction.Add(SkillManager.GetInstance().skillList.Find(s => s.EName == "SkillOrToolList"));
        firstAction.Add(SkillManager.GetInstance().skillList.Find(s => s.EName == "RestoreChakra"));
        firstAction.Add(SkillManager.GetInstance().skillList.Find(s => s.EName == "Rest"));
        firstAction.Add(SkillManager.GetInstance().skillList.Find(s => s.EName == "EndRound"));

        secondAction.Add(SkillManager.GetInstance().skillList.Find(s => s.EName == "SkillOrToolList"));
        secondAction.Add(SkillManager.GetInstance().skillList.Find(s => s.EName == "EndRound"));


        var characterData = Global.characterRecords.Find(d => d.characterInfoID == characterInfoID);



        foreach (var data in characterData.skillRecords)
        {
            skills.Add(data.skillInfoID, data.level);
        }

        foreach (var item in characterData.itemCharacterRecords)
        {
            itemCharacterRecords.Add(item.uniqueID, item);
        }
    }

    public void SetObstacle()
    {
        characterIdentity = CharacterIdentity.obstacle;
        identity = "障碍";
        rend = GetComponentsInChildren<Renderer>();
        firstAction = new List<Skill>();
        secondAction = new List<Skill>();
        skills = new Dictionary<int, int>();
    }

    public void SetClone(Transform noumenon)
    {
        base.Initialize();
        identity = "分身";
        
        characterIdentity = CharacterIdentity.clone;
        firstAction = new List<Skill>();
        secondAction = new List<Skill>();
        skills = new Dictionary<int, int>();

        firstAction.Add(SkillManager.GetInstance().skillList.Find(s => s.EName == "Move"));
        firstAction.Add(SkillManager.GetInstance().skillList.Find(s => s.EName == "EndRound"));
        
    }

    public void SetAdvancedClone(Transform noumenon)
    {
        base.Initialize();

        characterIdentity = CharacterIdentity.advanceClone;
        firstAction = new List<Skill>();
        secondAction = new List<Skill>();
        skills = new Dictionary<int, int>();

        firstAction.Add(SkillManager.GetInstance().skillList.Find(s => s.EName == "Move"));
        firstAction.Add(SkillManager.GetInstance().skillList.Find(s => s.EName == "SkillOrToolList"));
        firstAction.Add(SkillManager.GetInstance().skillList.Find(s => s.EName == "EndRound"));

        secondAction.Add(SkillManager.GetInstance().skillList.Find(s => s.EName == "SkillOrToolList"));
        secondAction.Add(SkillManager.GetInstance().skillList.Find(s => s.EName == "EndRound"));

        var characterRecord = Global.characterRecords.Find(d => d.characterInfoID == characterInfoID);

        foreach (var data in characterRecord.skillRecords)
        {
            skills.Add(data.skillInfoID, data.level);
        }
    }

    public void SetBeastClone(Transform noumenon)
    {
        base.Initialize();

        
        characterIdentity = CharacterIdentity.beastClone;
        firstAction = new List<Skill>();
        secondAction = new List<Skill>();
        skills = new Dictionary<int, int>();

        firstAction.Add(SkillManager.GetInstance().skillList.Find(s => s.EName == "Move"));
        firstAction.Add(SkillManager.GetInstance().skillList.Find(s => s.EName == "SkillOrToolList"));
        firstAction.Add(SkillManager.GetInstance().skillList.Find(s => s.EName == "EndRound"));

        secondAction.Add(SkillManager.GetInstance().skillList.Find(s => s.EName == "SkillOrToolList"));
        secondAction.Add(SkillManager.GetInstance().skillList.Find(s => s.EName == "EndRound"));

        var characterData = Global.characterRecords.Find(d => d.characterInfoID == characterInfoID);

        foreach (var data in characterData.skillRecords)
        {
            skills.Add(data.skillInfoID, data.level);
        }
    }

    public override void OnDestroyed()
    {
        base.OnDestroyed();
        Debug.Log(transform.name + " is Dead!");
        
        switch (characterIdentity)
        {
            case CharacterIdentity.noumenon:
                GetComponent<Animator>().SetBool("Dead", true);
                Destroy(gameObject, 4f);
                break;
            case CharacterIdentity.clone:
                Destroy(gameObject);
                break;
            case CharacterIdentity.advanceClone:
                Destroy(gameObject);
                break;
            case CharacterIdentity.beastClone:
                GetComponent<Animator>().SetBool("Dead", true);
                Destroy(gameObject, 4f);
                break;
            default:
                break;
        }
    }
}
