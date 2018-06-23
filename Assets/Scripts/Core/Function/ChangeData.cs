using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeData {
    //valueMax用作外部显示，包含bonus部分在内。如需拆分显示，需要手动减去bonus值。
    public static bool ChangeValue(Transform character, string dataName, int value)
    {
        var list = character.GetComponent<CharacterStatus>().attributes;
        SLG.Attribute data;
        if (list.Find(d => d.eName == dataName) == null)
            return false;
        else
            data = list.Find(d => d.eName == dataName);
        data.value = value;
        //值大于零小于最大值，用于抹除值大于最大值的部分。
        data.value = Mathf.Clamp(data.value, 0, data.valueMax);
        return true;
    }

    //升级时调用改动。
    public static bool ChangeValueMax(Transform character, string dataName, int value)
    {
        var list = character.GetComponent<CharacterStatus>().attributes;
        SLG.Attribute data;
        if (list.Find(d => d.eName == dataName) == null)
            return false;
        else
            data = list.Find(d => d.eName == dataName);
        data.valueMax = value;
        //最大值大于等于零
        data.valueMax = Mathf.Clamp(data.valueMax, 0, data.valueMax);
        //值大于零小于最大值，用于抹除值大于最大值的部分。
        data.value = Mathf.Clamp(data.value, 0, data.valueMax);
        return true;
    }

    //技能提升时改动。
    public static bool ChangeBonus(Transform character, string dataName, int value)
    {
        var list = character.GetComponent<CharacterStatus>().attributes;
        SLG.Attribute data;
        if (list.Find(d => d.eName == dataName) == null)
            return false;
        else
            data = list.Find(d => d.eName == dataName);
        data.bonus = value;
        //奖励值大于等于零
        data.bonus = Mathf.Clamp(data.bonus, 0, data.bonus);
        //valueMax适应bonus改变。
        data.valueMax = Mathf.Clamp(data.valueMax + data.bonus, 0, data.valueMax + data.bonus);
        //值大于零小于最大值，用于抹除值大于最大值的部分。
        data.value = Mathf.Clamp(data.value, 0, data.valueMax);
        return true;
    }
}
