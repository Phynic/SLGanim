using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Utils_Save
{
    public static void Save(string index)
    {
        string fileName = GetSaveFileName(index);
        Save save = new Save();
        save.SetGameDataToSave(index);
        string[] savecontent = new string[] { /*JsonUtility.ToJson(save)*/Utils_Decrypt.Encrypt(JsonUtility.ToJson(save)) };
        Utils_File.CreateFile(GetSavePath(), fileName, savecontent);
    }

    public static void Save(Save save, string index)
    {
        string fileName = GetSaveFileName(index);
        string[] savecontent = new string[] { /*JsonUtility.ToJson(save)*/Utils_Decrypt.Encrypt(JsonUtility.ToJson(save)) };
        Utils_File.CreateFile(GetSavePath(), fileName, savecontent);
    }

    public static void Load(Save save)
    {
        save.SetSaveDataToGame(true);
    }

    public static List<Save> LoadSaveList()
    {
        List<Save> saves = new List<Save>();
        for (int i = 0; i < Global.maxSaveCount; i++)
        {
            string index = GameManager.IndexToString(i);
            string fileName = GetSaveFileName(index);
            Save save = null;
            try
            {
                //存在存档文件
                if (File.Exists(GetSavePath() + "/" + fileName))
                {
                    string[] strs = Utils_File.ReadFileAllLines(GetSavePath(), fileName);
                    save = ConvertStringToSave(strs[0]);
                    saves.Add(save);
                }
                else
                {
                    continue;
                }
            }
            catch
            {
                Debug.LogError("读取存档失败！");
            }
        }
        return saves;
    }

    public static Save ConvertStringToSave(string str)
    {
        Save save = null;
        save = JsonUtility.FromJson<Save>(Utils_Decrypt.Decrypt(str));

        save.playerRecord = new List<StringIntKV>();

        save.itemRecords = new List<ItemRecord>();
        save.characterRecords = new List<CharacterRecord>();

        return save;
    }

    public static string GetSavePath()
    {
        string path = null;
#if UNITY_STANDALONE || UNITY_EDITOR || UNITY_EDITOR_OSX
        path = Application.persistentDataPath;
#elif UNITY_IOS
        path = Application.persistentDataPath;
#elif UNITY_ANDROID
        path = Application.persistentDataPath;
#endif
        return path;
    }

    public static string GetSaveFileName(string index)
    {
        return "save" + index + ".sav";
    }
}

[Serializable]
public class Save
{
    public string saveName;
    public string createDate;
    public string saveVersion;
    public string saveDate;
    public string procedure;
    public List<StringIntKV> playerRecord;
    public List<CharacterRecord> characterRecords;
    public List<ItemRecord> itemRecords;

    public void CreateNewSave(string saveName)
    {
        PlayerPrefs.DeleteAll();
        createDate = Utils_Time.GenerateTimeStamp();
        //写入基础数据
        this.saveName = saveName;
        saveVersion = Global.version;
        saveDate = createDate;
        procedure = "Procedure_Gal";

        playerRecord = new List<StringIntKV>
        {
            new StringIntKV("GalSetID", 0),
            new StringIntKV("LevelID", 1)
        };

        characterRecords = new List<CharacterRecord>
        {
            CreateNewCharacter(1001, 10),
            CreateNewCharacter(1002, 10),
            CreateNewCharacter(1003, 10),
            CreateNewCharacter(1004, 10),
            CreateNewCharacter(1005, 10)
        };

        itemRecords = new List<ItemRecord>();
    }

    public CharacterRecord CreateNewCharacter(int characterInfoID, int level)
    {
        var character = new CharacterRecord(characterInfoID, level);
        CharacterInfo info = CharacterInfoDictionary.GetParam(characterInfoID);
        character.exp = 0;
        character.skp = level + info.skp;
        character.skillRecords = new List<SkillRecord>();

        foreach (var skillID in info.commonSkillList)
        {
            character.skillRecords.Add(new SkillRecord(skillID, SkillInfoDictionary.GetParam(skillID).initialLevel));
        }
        foreach (var skillID in info.uniqueSkillList)
        {
            character.skillRecords.Add(new SkillRecord(skillID, SkillInfoDictionary.GetParam(skillID).initialLevel));
        }

        character.itemCharacterRecords = new List<ItemRecord>();
        
        return character;
    }

    public void SetGameDataToSave(string index)
    {
        saveName = "存档" + index;
        saveDate = Utils_Time.GenerateTimeStamp();
        saveVersion = Global.version;
        createDate = Global.createDate;
        
        foreach (var characterData in Global.characterDataList)
        {
            characterRecords.Add(new CharacterRecord(characterData.characterInfoID,characterData.attributes.Find(d => d.eName == "lev").Value));
        }

        foreach (var item in Global.itemRecords.Values)
        {
            itemRecords.Add(item);
        }

        playerRecord = Global.playerRecord.ToArray();

        procedure = GameManager.GetInstance().GetProcedureName();
    }

    public void SetSaveDataToGame(bool changeProcedure)
    {
        Global.createDate = createDate == "" ? createDate : Utils_Time.GenerateTimeStamp();
        
        foreach (var record in characterRecords)
        {
            var data = new CharacterData();
            data.characterInfoID = record.characterInfoID;
            var info = CharacterInfoDictionary.GetParam(data.characterInfoID);
            data.roleEName = info.roleEName;
            data.roleCName = info.roleCName;
            data.skills = record.skillRecords;
            data.items = record.itemCharacterRecords;

            //可能受技能加成所以放后面
            Global.SetCharacterAttributes(record, data);

            Global.characterDataList.Add(data);
        }

        foreach (var item in itemRecords)
        {
            Global.itemRecords.Add(item.uniqueID, item);
        }

        foreach (StringIntKV record in playerRecord)
        {
            Global.playerRecord.SetData(record.name, record.value);
        }

        if (changeProcedure)
        {
            GameManager.GetInstance().ChangeProcedure(procedure);
        }
    }
}



[Serializable]
public class CharacterRecord
{
    public int characterInfoID;
    public int level;
    public int exp;     //当前level已获得的经验值
    public int skp;     //未使用的技能点
    
    public List<SkillRecord> skillRecords;
    public List<ItemRecord> itemCharacterRecords;
    public CharacterRecord(int characterInfoID, int level)
    {
        this.characterInfoID = characterInfoID;
        this.level = level;
    }
}

[Serializable]
public class SkillRecord
{
    public int skillInfoID;
    public int level;
    public SkillRecord(int skillInfoID, int level)
    {
        this.skillInfoID = skillInfoID;
        this.level = level;
    }
}

[Serializable]
public class ItemRecord
{
    public int uniqueID;
    public int skillInfoID;
    public int level;
    public int ownerID;
    public int slotID;

    public ItemRecord(int uniqueID, int skillInfoID, int level)
    {
        this.uniqueID = uniqueID;
        this.skillInfoID = skillInfoID;
        this.level = level;
        ownerID = 0;
        slotID = -1;
    }

    public ItemRecord(int uniqueID, int skillInfoID, int level, int ownerID, int slotID)
    {
        this.uniqueID = uniqueID;
        this.skillInfoID = skillInfoID;
        this.level = level;
        this.ownerID = ownerID;
        this.slotID = slotID;
    }
}