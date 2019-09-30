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
        string[] savecontent = new string[] { Utils_Decrypt.Encrypt(JsonUtility.ToJson(save)) };
        Utils_File.CreateFile(GetSavePath(), fileName, savecontent);
    }

    public static Save Load(string index)
    {
        string fileName = GetSaveFileName(index);
        Save save = null;
        try
        {
            //存在存档文件
            if (File.Exists(GetSavePath() + "/" + fileName))
            {
                string[] strs = Utils_File.ReadFileAllLines(GetSavePath(), fileName);
                save = ConvertStringToSave(strs[0]);
            }
            else
            {
                save = new Save();
                save.CreateNewSave();
            }
        }
        catch
        {
            Debug.LogError("读取存档失败！");
        }

        return save;
    }



    public static Save ConvertStringToSave(string str)
    {
        Save save = null;
        save = JsonUtility.FromJson<Save>(Utils_Decrypt.Decrypt(str));

        save.playerData = new List<StringIntKV>();

        save.itemRecords = new List<ItemPlayerRecord>();
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
public class Save : MonoBehaviour
{
    public string saveName;

    public string CreateVersion { get; private set; }
    public string CreateDate { get; private set; }
    public string SaveVersion { get; set; }
    public string SaveDate { get; set; }

    public List<StringIntKV> playerData;
    public List<CharacterRecord> characterRecords;
    public List<ItemPlayerRecord> itemRecords;

    public void CreateNewSave()
    {
        PlayerPrefs.DeleteAll();
        CreateVersion = "1.0";
        CreateDate = Utils_Time.GenerateTimeStamp();
        //写入基础数据
        saveName = "新存档";
        SaveVersion = CreateVersion;
        SaveDate = CreateDate;
        playerData = new List<StringIntKV>();
        characterRecords = new List<CharacterRecord>();
        characterRecords.Add(CreateNewCharacter(1001, 10));
        characterRecords.Add(CreateNewCharacter(1002, 10));
        characterRecords.Add(CreateNewCharacter(1003, 10));
        characterRecords.Add(CreateNewCharacter(1004, 10));
        characterRecords.Add(CreateNewCharacter(1005, 10));
        itemRecords = new List<ItemPlayerRecord>();
    }

    public CharacterRecord CreateNewCharacter(int characterInfoID, int level)
    {
        var character = new CharacterRecord(characterInfoID, level);
        CharacterInfo info = CharacterInfoDictionary.GetParam(characterInfoID);
        character.exp = 0;
        character.skp = level + info.skp;
        character.skillCharacterRecords = new List<SkillCharacterRecord>();

        foreach (var skillID in info.commonSkillList)
        {
            character.skillCharacterRecords.Add(new SkillCharacterRecord(skillID, 0));
        }
        foreach (var skillID in info.uniqueSkillList)
        {
            character.skillCharacterRecords.Add(new SkillCharacterRecord(skillID, 0));
        }

        character.itemCharacterRecords = new List<ItemCharacterRecord>();
        
        return character;
    }
}

[Serializable]
public class ItemPlayerRecord
{
    public int uniqueID;
    public int skillInfoID;
    public int level;
}

[Serializable]
public class CharacterRecord
{
    public int characterInfoID;
    public int level;
    public int exp;     //当前level已获得的经验值
    public int skp;     //未使用的技能点
    
    public List<SkillCharacterRecord> skillCharacterRecords;
    public List<ItemCharacterRecord> itemCharacterRecords;
    public CharacterRecord(int characterInfoID, int level)
    {
        this.characterInfoID = characterInfoID;
        this.level = level;
    }
}

[Serializable]
public class SkillCharacterRecord
{
    public int skillInfoID;
    public int level;
    public SkillCharacterRecord(int skillInfoID, int level)
    {
        this.skillInfoID = skillInfoID;
        this.level = level;
    }
}

[Serializable]
public class ItemCharacterRecord
{
    public int uniqueID;
    public int slotID;
    public ItemCharacterRecord(int uniqueID, int slotID)
    {
        this.uniqueID = uniqueID;
        this.slotID = slotID;
    }
}