using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Save : MonoBehaviour
{
    public int ID;
    public string saveName;
    public string saveVersion;
    public string createDate;
    public string saveDate;
    public List<StringIntKV> playerData;
    public List<ItemData> items;
}

[System.Serializable]
public class ItemData
{
    public int ID;
    public int itemID;
    public int ownerID;
    public int itemLevel;
    public string itemName;
    public string equipped;
    
    public ItemData(int ID, int itemID, int ownerID, string itemName)
    {
        this.ID = ID;
        this.itemID = itemID;
        this.ownerID = ownerID;
        this.itemName = itemName;
        itemLevel = 0;
        equipped = "";
    }
}
