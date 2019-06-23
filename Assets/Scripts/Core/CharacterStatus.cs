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
    public List<PrivateItemData> items = new List<PrivateItemData>();             //忍具列表
    public Dictionary<string, int> skills; //忍术列表<忍术名称，技能等级>

    public Vector3 arrowPosition = new Vector3(0, 1.1f, 0);
    public List<Skill> firstAction;                 //第一次行动列表
    public List<Skill> secondAction;                //第二次行动列表
    
    public override void Initialize()
    {
        base.Initialize();
        
        attributes = new List<Attribute>();

        var characterData = GameController.GetInstance().characterDB.characterDataList.Find(d => d.roleEName == roleEName && playerNumber == d.playerNumber);
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
        skills = new Dictionary<string, int>();

        firstAction.Add(SkillManager.GetInstance().skillList.Find(s => s.EName == "Move"));
        firstAction.Add(SkillManager.GetInstance().skillList.Find(s => s.EName == "SkillOrToolList"));
        firstAction.Add(SkillManager.GetInstance().skillList.Find(s => s.EName == "RestoreChakra"));
        firstAction.Add(SkillManager.GetInstance().skillList.Find(s => s.EName == "Rest"));
        firstAction.Add(SkillManager.GetInstance().skillList.Find(s => s.EName == "EndRound"));

        secondAction.Add(SkillManager.GetInstance().skillList.Find(s => s.EName == "SkillOrToolList"));
        secondAction.Add(SkillManager.GetInstance().skillList.Find(s => s.EName == "EndRound"));


        var characterData = GameController.GetInstance().characterDB.characterDataList.Find(d => d.roleEName == roleEName);

        foreach (var data in characterData.skills)
        {
            skills.Add(data.skillName, data.skillLevel);
        }

        for(int i = 0; i < characterData.items.Count; i++)
        {
            items.Add(characterData.items[i]);
        }

        //attributes.Find(d => d.eName == "itemNum").value = items.Count;
    }

    public void SetObstacle()
    {
        characterIdentity = CharacterIdentity.obstacle;
        identity = "障碍";
        rend = GetComponentsInChildren<Renderer>();
        firstAction = new List<Skill>();
        secondAction = new List<Skill>();
        skills = new Dictionary<string, int>();

        //firstAction.Add(SkillManager.GetInstance().skillList.Find(s => s.EName == "Move"));
        //firstAction.Add(SkillManager.GetInstance().skillList.Find(s => s.EName == "SkillOrToolList"));
        //firstAction.Add(SkillManager.GetInstance().skillList.Find(s => s.EName == "RestoreChakra"));
        //firstAction.Add(SkillManager.GetInstance().skillList.Find(s => s.EName == "Rest"));
        //firstAction.Add(SkillManager.GetInstance().skillList.Find(s => s.EName == "EndRound"));

        //secondAction.Add(SkillManager.GetInstance().skillList.Find(s => s.EName == "SkillOrToolList"));
        //secondAction.Add(SkillManager.GetInstance().skillList.Find(s => s.EName == "EndRound"));


        //var characterData = XMLManager.GetInstance().characterDB.characterDataList.Find(d => d.roleEName == roleEName);

        //foreach (var data in characterData.skills)
        //{
        //    skills.Add(data.skillName, data.skillLevel);
        //}

        //for (int i = 0; i < characterData.items.Count; i++)
        //{
        //    items.Add(characterData.items[i]);
        //}

        //attributes.Find(d => d.eName == "itemNum").value = items.Count;
    }

    public void SetClone(Transform noumenon)
    {
        base.Initialize();
        identity = "分身";
        attributes = new List<Attribute>();

        //序列化和反序列化进行深度复制。
        MemoryStream stream = new MemoryStream();
        BinaryFormatter formatter = new BinaryFormatter();
        formatter.Serialize(stream, noumenon.GetComponent<CharacterStatus>().attributes);
        stream.Position = 0;
        attributes = formatter.Deserialize(stream) as List<Attribute>;
        
        characterIdentity = CharacterIdentity.clone;
        firstAction = new List<Skill>();
        secondAction = new List<Skill>();
        skills = new Dictionary<string, int>();

        firstAction.Add(SkillManager.GetInstance().skillList.Find(s => s.EName == "Move"));
        firstAction.Add(SkillManager.GetInstance().skillList.Find(s => s.EName == "EndRound"));
        
    }

    public void SetAdvancedClone(Transform noumenon)
    {
        base.Initialize();

        attributes = new List<Attribute>();

        //序列化和反序列化进行深度复制。
        MemoryStream stream = new MemoryStream();
        BinaryFormatter formatter = new BinaryFormatter();
        formatter.Serialize(stream, noumenon.GetComponent<CharacterStatus>().attributes);
        stream.Position = 0;
        attributes = formatter.Deserialize(stream) as List<Attribute>;

        characterIdentity = CharacterIdentity.advanceClone;
        firstAction = new List<Skill>();
        secondAction = new List<Skill>();
        skills = new Dictionary<string, int>();

        firstAction.Add(SkillManager.GetInstance().skillList.Find(s => s.EName == "Move"));
        firstAction.Add(SkillManager.GetInstance().skillList.Find(s => s.EName == "SkillOrToolList"));
        firstAction.Add(SkillManager.GetInstance().skillList.Find(s => s.EName == "EndRound"));

        secondAction.Add(SkillManager.GetInstance().skillList.Find(s => s.EName == "SkillOrToolList"));
        secondAction.Add(SkillManager.GetInstance().skillList.Find(s => s.EName == "EndRound"));

        var characterData = GameController.GetInstance().characterDB.characterDataList.Find(d => d.roleEName == roleEName);

        foreach (var data in characterData.skills)
        {
            skills.Add(data.skillName, data.skillLevel);
        }
    }

    public void SetBeastClone(Transform noumenon)
    {
        base.Initialize();

        attributes = new List<Attribute>();

        //序列化和反序列化进行深度复制。
        MemoryStream stream = new MemoryStream();
        BinaryFormatter formatter = new BinaryFormatter();
        formatter.Serialize(stream, noumenon.GetComponent<CharacterStatus>().attributes);
        stream.Position = 0;
        attributes = formatter.Deserialize(stream) as List<Attribute>;

        characterIdentity = CharacterIdentity.beastClone;
        firstAction = new List<Skill>();
        secondAction = new List<Skill>();
        skills = new Dictionary<string, int>();

        firstAction.Add(SkillManager.GetInstance().skillList.Find(s => s.EName == "Move"));
        firstAction.Add(SkillManager.GetInstance().skillList.Find(s => s.EName == "SkillOrToolList"));
        firstAction.Add(SkillManager.GetInstance().skillList.Find(s => s.EName == "EndRound"));

        secondAction.Add(SkillManager.GetInstance().skillList.Find(s => s.EName == "SkillOrToolList"));
        secondAction.Add(SkillManager.GetInstance().skillList.Find(s => s.EName == "EndRound"));

        var characterData = GameController.GetInstance().characterDB.characterDataList.Find(d => d.roleEName == roleEName);

        foreach (var data in characterData.skills)
        {
            skills.Add(data.skillName, data.skillLevel);
        }
    }
    
    public void LevelUp()
    {
        var growth = GameController.GetInstance().growthData.Find(g => g.roleEName == roleEName);
        var characterData = GameController.GetInstance().characterDB.characterDataList.Find(d => d.roleEName == roleEName && d.playerNumber == playerNumber);

        characterData.attributes.Find(d => d.eName == "lev").value++;
        var level = characterData.attributes.Find(d => d.eName == "lev").value;

        characterData.attributes.Find(d => d.eName == "hp").valueMax = (int)(growth.hpG * (level + 10) + 100);
        characterData.attributes.Find(d => d.eName == "hp").value = characterData.attributes.Find(d => d.eName == "hp").valueMax;
        characterData.attributes.Find(d => d.eName == "mp").valueMax = (int)(3 + growth.mpG * level);
        characterData.attributes.Find(d => d.eName == "mp").value = characterData.attributes.Find(d => d.eName == "mp").valueMax;
        characterData.attributes.Find(d => d.eName == "atk").value = (int)(growth.atkG * (level + 10));
        characterData.attributes.Find(d => d.eName == "def").value = (int)(growth.defG * (level + 10));
        characterData.attributes.Find(d => d.eName == "dex").value = (int)(growth.dexG * (level + 10));
        characterData.attributes.Find(d => d.eName == "exp").valueMax = (int)(255 + 15 * level * growth.expG);
        characterData.attributes.Find(d => d.eName == "exp").value = 0;
        characterData.attributes.Find(d => d.eName == "skp").value++;
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
