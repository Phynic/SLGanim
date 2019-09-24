using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Global
{
    #region 运行时数据
    public static int playerNumber;
    public static CharacterDataBase characterDB;
    public static CharacterDataBase levelCharacterDB;
    public static List<ItemData> items = new List<ItemData>();
    #endregion
}
