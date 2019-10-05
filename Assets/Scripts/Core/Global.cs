using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Global
{
    #region 游戏参数
    public static string version = "0.2";

    public static int maxSaveCount = 5;
    #endregion

    #region 游戏数据
    public static int playerNumber = 1;
    public static List<CharacterData> characterDataList = new List<CharacterData>();
    public static int LevelID
    {
        get { return playerRecord.GetValue("LevelID"); }
        set { playerRecord.SetData("LevelID", value); }
    }

    public static int GalSetID
    {
        get { return playerRecord.GetValue("GalSetID"); }
        set { playerRecord.SetData("GalSetID", value); }
    }
    #endregion

    #region 存档数据
    public static string createVersion;
    public static string createDate;
    public static DataRegister playerRecord = new DataRegister();
    public static Dictionary<int, ItemRecord> itemRecords = new Dictionary<int, ItemRecord>();
    #endregion


    public static void SetCharacterAttributes(CharacterRecord characterRecord, CharacterData data)
    {
        var characterInfo = CharacterInfoDictionary.GetParamList().Find(g => g.ID == data.characterInfoID);
        foreach (var attributeInfo in AttributeInfoDictionary.GetParamList())
        {
            var attribute = new SLG.Attribute(attributeInfo.ID);
            data.attributes.Add(attribute);
        }
        var level = characterRecord.level;

        data.attributes.Find(d => d.eName == "lev").ChangeValueTo(level);

        data.attributes.Find(d => d.eName == "hp").ChangeValueMaxTo((int)(characterInfo.hpGrowth * (level + 10) + 100));;
        data.attributes.Find(d => d.eName == "hp").ChangeValueTo(data.attributes.Find(d => d.eName == "hp").ValueMax);
        data.attributes.Find(d => d.eName == "mp").ChangeValueMaxTo((int)(3 + characterInfo.mpGrowth * level));
        data.attributes.Find(d => d.eName == "mp").ChangeValueTo(data.attributes.Find(d => d.eName == "mp").ValueMax);
        data.attributes.Find(d => d.eName == "atk").ChangeValueTo((int)(characterInfo.atkGrowth * (level + 10)));
        data.attributes.Find(d => d.eName == "def").ChangeValueTo((int)(characterInfo.defGrowth * (level + 10)));
        data.attributes.Find(d => d.eName == "dex").ChangeValueTo((int)(characterInfo.dexGrowth * (level + 10)));
        data.attributes.Find(d => d.eName == "exp").ChangeValueMaxTo((int)(255 + 15 * level * characterInfo.expGrowth));

        data.attributes.Find(d => d.eName == "exp").ChangeValueTo(characterRecord.exp);
        data.attributes.Find(d => d.eName == "skp").ChangeValueTo(characterRecord.skp);

    }
}

[System.Serializable]
public class CharacterData
{
    public int characterInfoID;
    public string roleEName;
    public string roleCName;
    public int playerNumber;
    //level在这里面
    public List<SLG.Attribute> attributes = new List<SLG.Attribute>();
    public List<SkillRecord> skills = new List<SkillRecord>();
    public List<ItemRecord> items = new List<ItemRecord>();
}