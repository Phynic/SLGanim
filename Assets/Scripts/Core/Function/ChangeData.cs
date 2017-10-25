using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeData {
    
    public static bool ChangeValue(Transform character, string dataName, int value)
    {
        var list = character.GetComponent<CharacterStatus>().attributes;
        if (list.Find(d => d.eName == dataName) == null)
            return false;
        list.Find(d => d.eName == dataName).value = value;
        //值大于零小于最大值
        list.Find(d => d.eName == dataName).value = Mathf.Clamp(list.Find(d => d.eName == dataName).value, 0, list.Find(d => d.eName == dataName).valueMax);
        return true;
    }

    public static bool ChangeValueMax(Transform character, string dataName, int value)
    {
        var list = character.GetComponent<CharacterStatus>().attributes;
        if (list.Find(d => d.eName == dataName) == null)
            return false;
        list.Find(d => d.eName == dataName).valueMax = value;
        //最大值大于等于零
        list.Find(d => d.eName == dataName).valueMax = Mathf.Clamp(list.Find(d => d.eName == dataName).valueMax, 0, list.Find(d => d.eName == dataName).valueMax);
        //值大于零小于最大值
        list.Find(d => d.eName == dataName).value = Mathf.Clamp(list.Find(d => d.eName == dataName).value, 0, list.Find(d => d.eName == dataName).valueMax);
        return true;
    }

    public static bool ChangeBonus(Transform character, string dataName, int value)
    {
        var list = character.GetComponent<CharacterStatus>().attributes;
        if (list.Find(d => d.eName == dataName) == null)
            return false;
        list.Find(d => d.eName == dataName).bonus = value;
        //奖励值大于等于零
        list.Find(d => d.eName == dataName).bonus = Mathf.Clamp(list.Find(d => d.eName == dataName).bonus, 0, list.Find(d => d.eName == dataName).bonus);
        //值大于零小于最大值
        list.Find(d => d.eName == dataName).value = Mathf.Clamp(list.Find(d => d.eName == dataName).value, 0, list.Find(d => d.eName == dataName).valueMax);
        return true;
    }
}
