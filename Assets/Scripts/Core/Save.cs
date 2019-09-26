using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Save : MonoBehaviour
{
    public string saveName;
    public string saveVersion;
    public string createDate;
    public string saveDate;
    public List<StringIntKV> playerData;

    public List<ItemPlayerRecord> itemRecords;
    public List<CharacterRecord> characterRecords;
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