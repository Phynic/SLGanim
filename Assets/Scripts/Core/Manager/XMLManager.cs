using System;
using System.Collections.Generic;
using UnityEngine;
using SLG;

using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Collections;

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
        Global.GetInstance();
    }
    
    //SAVE
    public void SaveCharacters()
    {
        XmlSerializer serializer = new XmlSerializer(typeof(CharacterDataBase));
        var encoding = System.Text.Encoding.GetEncoding("UTF-8");
        StreamWriter stream = new StreamWriter(Application.streamingAssetsPath + "/XML/characterData.xml", false, encoding);
        serializer.Serialize(stream, characterDB);
        stream.Close();
    }

    public void SaveGameData()
    {
        XmlSerializer serializer = new XmlSerializer(typeof(GameDataBase));
        var encoding = System.Text.Encoding.GetEncoding("UTF-8");
        StreamWriter stream = new StreamWriter(Application.streamingAssetsPath + "/XML/gameData.xml", false, encoding);
        serializer.Serialize(stream, gameDB);
        stream.Close();
    }

    ////LOAD
    //public void LoadCharacters()
    //{
    //    XmlSerializer serializer = new XmlSerializer(typeof(CharacterDataBase));
    //    string path = Application.streamingAssetsPath + "/XML/characterData.xml";
    //    StreamReader stream = new StreamReader(path);
    //    characterDB = serializer.Deserialize(stream) as CharacterDataBase;
    //    stream.Close();
    //}

    //public void LoadGameData()
    //{
    //    XmlSerializer serializer = new XmlSerializer(typeof(GameDataBase));
    //    string path = Application.streamingAssetsPath + "/XML/gameData.xml";

    //    StreamReader stream = new StreamReader(path);

    //    gameDB = serializer.Deserialize(stream) as GameDataBase;
    //    stream.Close();
    //}


    //深坑：这里的path只能外部传进来，写在内部在打包后无法读取。
    public IEnumerator LoadGameData(string path)
    {
        WWW www = new WWW(path);
        yield return www;
        XmlSerializer serializer = new XmlSerializer(typeof(GameDataBase));

        StringReader sr = new StringReader(www.text);
        sr.Read();      //跳过BOM头
        gameDB = serializer.Deserialize(sr) as GameDataBase;
        sr.Close();

        Global.GetInstance().OnLoadGameDataComplete();
    }

    public IEnumerator LoadCharacters(string path)
    {
        WWW www = new WWW(path);
        yield return www;
        XmlSerializer serializer = new XmlSerializer(typeof(CharacterDataBase));

        StringReader sr = new StringReader(www.text);
        sr.Read();      //跳过BOM头
        characterDB = serializer.Deserialize(sr) as CharacterDataBase;
        sr.Close();

        Global.GetInstance().OnLoadCharactersComplete();
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
}

[System.Serializable]
public class PrivateItemData
{
    public string itemName;
    public int itemLevel;
    public SLG.Material itemMaterial;
    public int itemPosition;
    public PrivateItemData() { }
    
}

[System.Serializable]
[XmlInclude(typeof(UnitSkillData))]
[XmlInclude(typeof(PassiveSkillData))]
public class SkillData
{
    public string cName;
    public string eName;
    public string description;
    public int maxLevel;
    public SkillData() { }
    
}

[System.Serializable]
[XmlInclude(typeof(AttackSkillData))]
public class UnitSkillData : SkillData
{
    public int costMP;
    public int costHP;
    public int skillRange;
    public int hoverRange;
    public int skillRate;
    public UnitSkill.ComboType comboType;
    public UnitSkill.SkillType skillType;
    public UnitSkill.SkillClass skillClass;
    public UnitSkill.RangeType rangeType;
    public int animID;

    public UnitSkillData() { }
}

[System.Serializable]
public class PassiveSkillData : SkillData
{
    public PassiveSkillData() { }
}

[System.Serializable]
public class AttackSkillData : UnitSkillData
{
    public int damageFactor;
    public int hit;
    public int extraCrit;
    public int extraPounce;

    public AttackSkillData() { }
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