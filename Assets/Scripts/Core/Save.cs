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
    public string itemName;
    public int itemLevel;
    public string equipped;

    public ItemData() { }
    public ItemData(int ID, string itemName)
    {
        this.ID = ID;
        this.itemName = itemName;
        itemLevel = 0;
        equipped = "";
    }
}
