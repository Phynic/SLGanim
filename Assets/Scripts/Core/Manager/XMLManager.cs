using System;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;
using System.IO;
using System.Collections;
using UnityEngine.Networking;

public static class XMLManager
{
    //Create on disk
    public static void CreateXML<T>(string path) where T : new()
    {
        T t = new T();
        Save(t, path);
    }

    //SAVE
    public static void Save<T>(T t, string path)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(T));
        var encoding = System.Text.Encoding.GetEncoding("UTF-8");
        
        StreamWriter stream = new StreamWriter(path, false, encoding);
        serializer.Serialize(stream, t);
        stream.Close();
    }
    
    //LOAD
    public static T Load<T>(string path)
    {
        T t;
        XmlSerializer serializer = new XmlSerializer(typeof(T));
        StreamReader stream = new StreamReader(path);
        t = (T)serializer.Deserialize(stream);
        stream.Close();
        return t;
    }
    
    //深坑：这里的path只能外部传进来，写在内部在打包后无法读取。
    public static IEnumerator LoadAsync<T>(string path, Action<T> action)
    {
        T t;
        UnityWebRequest uwr = UnityWebRequest.Get(path);
        yield return uwr.SendWebRequest();

        XmlSerializer serializer = new XmlSerializer(typeof(T));
        StringReader sr = new StringReader(uwr.downloadHandler.text);
        try
        {
            sr.Read();      //跳过BOM头
            t = (T)serializer.Deserialize(sr);
            action(t);
            sr.Close();
        }
        catch
        {
            Debug.Log("无内容，或内容格式错误。");
        }
    }
}

[System.Serializable]
public class Config
{
    public bool showFPS;
    public int qualityLevel;
}

[System.Serializable]
public class Growth
{
    public string roleEName;
    public float hpG;
    public float mpG;
    public float atkG;
    public float defG;
    public float dexG;
    public float expG;

    public Growth() { }
}

[System.Serializable]
public class CharacterData
{
    public string roleEName;
    public string roleCName;
    public int playerNumber;

    public List<SLG.Attribute> attributes = new List<SLG.Attribute>();
    public List<SkillCharacterRecord> skills = new List<SkillCharacterRecord>();
    public List<ItemCharacterRecord> items = new List<ItemCharacterRecord>();

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
}

[System.Serializable]
[XmlInclude(typeof(UnitSkillData))]
[XmlInclude(typeof(PassiveSkillData))]
public class SkillData
{
    public string cName;
    public string eName;
    public string description;
    public int maxLevel;        //被动技能中要用，所以放在父类。下同。
    public int factor;
    public float growFactor;

    public SkillData() { }
    
}

[System.Serializable]
public class PrivateSkillData
{
    public string skillName;
    public int skillLevel;

    public PrivateSkillData() { }
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
    
    public ComboType comboType;
    public SkillType skillType;
    public SkillClass skillClass;
    public RangeType rangeType;
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
    public int damage;
    public int hit;
    public int extraCrit;
    public int extraPounce;
    public AttackSkillData() { }
}

[System.Serializable]
public class ItemData
{
    public int uniqueID;
    public int itemID;
    public int ownerID;
    public int itemLevel;
    public string itemName;
    public string equipped;

    public ItemData(int ID, int itemID, int ownerID, string itemName)
    {
        this.uniqueID = ID;
        this.itemID = itemID;
        this.ownerID = ownerID;
        this.itemName = itemName;
        itemLevel = 0;
        equipped = "";
    }
}

//[System.Serializable]
//public class PrivateItemData
//{
//    public int ID;
//    public string itemName;
//    public int itemPosition;
//    public PrivateItemData() { }
//    public PrivateItemData(int ID, string itemName, int itemPosition)
//    {
//        this.ID = ID;
//        this.itemName = itemName;
//        this.itemPosition = itemPosition;
//    }
//}