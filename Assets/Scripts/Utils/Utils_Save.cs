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
    public List<ItemPlayerRecord> itemRecords;
    public List<CharacterRecord> characterRecords;

    public void CreateNewSave()
    {
        PlayerPrefs.DeleteAll();
        CreateVersion = "1.0";
        CreateDate = Utils_Time.GenerateTimeStamp();
        //写入基础数据
        saveName = "存档1";
        SaveVersion = CreateVersion;
        SaveDate = CreateDate;
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

    /// <summary>
    /// skillInfoID_level
    /// </summary>
    public List<SkillCharacterRecord> skillCharacterRecords = new List<SkillCharacterRecord>();
    /// <summary>
    /// uniqueID_slotID
    /// </summary>
    public List<ItemCharacterRecord> itemCharacterRecords = new List<ItemCharacterRecord>();
}

[Serializable]
public class SkillCharacterRecord
{
    public int skillInfoID;
    public int level;
}

[Serializable]
public class ItemCharacterRecord
{
    public int uniqueID;
    public int slotID;
}