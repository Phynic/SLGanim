using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SLG;

using System.Xml;
using System.Xml.Serialization;
using System.IO;

public class XMLManager : MonoBehaviour
{
    private static XMLManager instance;
    public CharacterDataBase characterDB = new CharacterDataBase();
    public GameDataBase gameDB = new GameDataBase();
    public static XMLManager GetInstance()
    {
        return instance;
    }
    

    void Awake()
    {
        instance = this;

        //characterDB.characterDataList.Add(new CharacterData("Naruto", "漩涡 鸣人"));
        //characterDB.characterDataList.Add(new CharacterData("Sasuke", "宇智波 佐助"));
        //characterDB.characterDataList.Add(new CharacterData("Kakashi", "旗木 卡卡西"));
        //characterDB.characterDataList.Add(new CharacterData("Rock", "岩墙"));
        //foreach (var a in characterDB.characterDataList)
        //{

        //    a.attributes.Add(new Attribute("lev", "等级", 1, 99, 0));
        //    a.attributes.Add(new Attribute("exp", "经验值", 0, 450, 0));
        //    a.attributes.Add(new Attribute("hp", "体力", 550, 550, 0));
        //    a.attributes.Add(new Attribute("mp", "查克拉", 5, 5, 0));
        //    a.attributes.Add(new Attribute("itemNum", "忍具数量", 0, 5, 0));
        //    a.attributes.Add(new Attribute("atk", "攻击力", 10, 99, 0));
        //    a.attributes.Add(new Attribute("def", "防御力", 10, 99, 0));
        //    a.attributes.Add(new Attribute("dex", "敏捷度", 10, 99, 0));
        //    a.attributes.Add(new Attribute("mrg", "移动力", 6, 9, 0));
        //    a.attributes.Add(new Attribute("mud", "印", 25, 25, 0));
        //    a.attributes.Add(new Attribute("skp", "技能点数", 0, 99, 0));

        //    a.skills.Add(new PrivateSkillData("NinjaCombo", 1));
        //    a.skills.Add(new PrivateSkillData("NinjaCombo1", 1));
        //    a.skills.Add(new PrivateSkillData("Telesport", 1));
        //    a.skills.Add(new PrivateSkillData("Substitute", 1));
        //    a.skills.Add(new PrivateSkillData("Clone", 1));
        //    a.skills.Add(new PrivateSkillData("Transfiguration", 1));
        //}

        //gameDB.skillDataList.Add(new SkillData("ChooseDirection", "决定人物朝向"));
        //gameDB.skillDataList.Add(new SkillData("ChooseTrick", "回避忍术列表"));
        //gameDB.skillDataList.Add(new SkillData("EndRound", "回合结束"));
        //gameDB.skillDataList.Add(new SkillData("FirstAction", "第一阶段"));
        //gameDB.skillDataList.Add(new SkillData("Move", "移动"));
        //gameDB.skillDataList.Add(new SkillData("Rest", "休息"));
        //gameDB.skillDataList.Add(new SkillData("RestoreChakra", "查克拉"));
        //gameDB.skillDataList.Add(new SkillData("SecondAction", "第二阶段"));
        //gameDB.skillDataList.Add(new SkillData("SkillOrToolList", "术·忍具"));

        //gameDB.skillDataList.Add(new UnitSkillData("Clone", "分身术", 2, 0, "", -1, 0, 100, UnitSkill.ComboType.cannot, UnitSkill.SkillType.effect, UnitSkill.RangeType.common, 2));
        //gameDB.skillDataList.Add(new UnitSkillData("Substitute", "替身术", 3, 0, "", -1, 0, 100, UnitSkill.ComboType.cannot, UnitSkill.SkillType.dodge, UnitSkill.RangeType.common, 2));
        //gameDB.skillDataList.Add(new UnitSkillData("Telesport", "瞬身术", 1, 0, "", -1, 0, 100, UnitSkill.ComboType.cannot, UnitSkill.SkillType.effect, UnitSkill.RangeType.common, 2));
        //gameDB.skillDataList.Add(new UnitSkillData("Transfiguration", "变化术", 2, 0, "", 1, 0, 100, UnitSkill.ComboType.cannot, UnitSkill.SkillType.effect, UnitSkill.RangeType.common, 2));

        //gameDB.skillDataList.Add(new AttackSkillData("NinjaCombo", "忍者组合拳", 0, 0, "", 1, 0, 98, UnitSkill.ComboType.cannot, UnitSkill.SkillType.attack, UnitSkill.RangeType.common, 1, -1, 1, 5, 5));
        //gameDB.skillDataList.Add(new AttackSkillData("NinjaCombo1", "八方手里剑（无限）", 0, 0, "", 3, 1, 98, UnitSkill.ComboType.cannot, UnitSkill.SkillType.attack, UnitSkill.RangeType.straight, 2, -1, 3, 5, 5));


        //SaveGameData();
        //SaveCharacters();
        LoadGameData();
        LoadCharacters();
    }
    
    //SAVE
    public void SaveCharacters()
    {
        XmlSerializer serializer = new XmlSerializer(typeof(CharacterDataBase));
        var encoding = System.Text.Encoding.GetEncoding("UTF-8");
        StreamWriter stream = new StreamWriter(Application.dataPath + "/StreamingAssets/XML/characterData.xml", false, encoding);
        serializer.Serialize(stream, characterDB);
        stream.Close();
    }

    public void SaveGameData()
    {
        XmlSerializer serializer = new XmlSerializer(typeof(GameDataBase));
        var encoding = System.Text.Encoding.GetEncoding("UTF-8");
        StreamWriter stream = new StreamWriter(Application.dataPath + "/StreamingAssets/XML/gameData.xml", false, encoding);
        serializer.Serialize(stream, gameDB);
        stream.Close();
    }

    //LOAD
    public void LoadCharacters()
    {
        XmlSerializer serializer = new XmlSerializer(typeof(CharacterDataBase));
        string path = Application.dataPath + "/StreamingAssets/XML/characterData.xml";
        StreamReader stream = new StreamReader(path);
        characterDB = serializer.Deserialize(stream) as CharacterDataBase;
        stream.Close();
    }

    public void LoadGameData()
    {
        XmlSerializer serializer = new XmlSerializer(typeof(GameDataBase));
        string path = Application.dataPath + "/StreamingAssets/XML/gameData.xml";
        StreamReader stream = new StreamReader(path);
        gameDB = serializer.Deserialize(stream) as GameDataBase;
        stream.Close();
    }
}



[System.Serializable]

public class CharacterData
{
    public string roleEName;
    public string roleCName;

    public List<SLG.Attribute> attributes = new List<SLG.Attribute>();
    public List<PrivateSkillData> skills = new List<PrivateSkillData>();
    public List<PrivateItemData> items = new List<PrivateItemData>();
    public CharacterData() { }

    public CharacterData(string eName, string cName)
    {
        roleEName = eName;
        roleCName = cName;
    }
    
}
 

[System.Serializable]
public class CharacterDataBase
{
    public List<CharacterData> characterDataList = new List<CharacterData>();
}

[System.Serializable]
public class GameDataBase
{
    public List<SkillData> skillDataList = new List<SkillData>();
    //public List<ToolData> toolDataList = new List<ToolData>();
    //public List<MaterialData> materialDataList = new List<MaterialData>();
}

[System.Serializable]
public class PrivateSkillData
{
    public string skillName;
    public int skillLevel;

    public PrivateSkillData() { }

    public PrivateSkillData(string name, int level)
    {
        skillName = name;
        skillLevel = level;
    }
}

[System.Serializable]
public class PrivateItemData
{
    public string itemName;
    public int itemLevel;
    public SLG.Material itemMaterial;
    public int itemPosition;
    public PrivateItemData() { }

    public PrivateItemData(string name, int level, SLG.Material material)
    {
        itemName = name;
        itemLevel = level;
        itemMaterial = material;
    }
}

[System.Serializable]
[XmlInclude(typeof(UnitSkillData))]
public class SkillData
{
    public string cName;
    public string eName;

    public SkillData() { }

    public SkillData(string eName, string cName)
    {
        this.eName = eName;
        this.cName = cName;
    }
}

[System.Serializable]
[XmlInclude(typeof(AttackSkillData))]
public class UnitSkillData : SkillData
{
    public int costMP;
    public int costHP;
    public string description;
    public int skillRange;
    public int hoverRange;
    public int skillRate;
    public UnitSkill.ComboType comboType;
    public UnitSkill.SkillType skillType;
    public UnitSkill.RangeType rangeType;
    public int animID;

    public UnitSkillData() { }

    public UnitSkillData(string eName, string cName, int costMP, int costHP, string description, int skillRange, int hoverRange, int skillRate, UnitSkill.ComboType comboType, UnitSkill.SkillType skillType, UnitSkill.RangeType rangeType, int animID) : base(eName, cName)
    {
        this.costMP = costMP;
        this.costHP = costHP;
        this.description = description;
        this.skillRange = skillRange;
        this.hoverRange = hoverRange;
        this.skillRate = skillRate;
        this.comboType = comboType;
        this.skillType = skillType;
        this.rangeType = rangeType;
        this.animID = animID;
    }
}

[System.Serializable]
public class AttackSkillData : UnitSkillData
{
    public int damageFactor;
    public int hit;
    public int extraCrit;
    public int extraPounce;

    public AttackSkillData() { }

    public AttackSkillData(string eName, string cName, int costMP, int costHP, string description, int skillRange, int hoverRange, int skillRate, UnitSkill.ComboType comboType, UnitSkill.SkillType skillType, UnitSkill.RangeType rangeType, int animID, int damageFactor, int hit, int extraCrit, int extraPounce) : base(eName, cName, costMP, costHP, description, skillRange, hoverRange, skillRate, comboType, skillType, rangeType, animID)
    {
        this.damageFactor = damageFactor;
        this.hit = hit;
        this.extraCrit = extraCrit;
        this.extraPounce = extraPounce;
    }
}














//[System.Serializable]
//public class ToolData
//{
//    public string skillName;
//    public int skillLevel;

//    public ToolData() { }

//    public ToolData(string name, int level)
//    {
//        skillName = name;
//    }
//}

//[System.Serializable]
//public class MaterialData
//{
//    public string skillName;
//    public int skillLevel;

//    public MaterialData() { }

//    public MaterialData(string name, int level)
//    {
//        skillName = name;
//    }
//}