using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BattlePrepareView : ViewBase<BattlePrepareView>
{
    public void Open(LevelInfo levelInfo, UnityAction onInit = null)
    {
        if (!isInit)
        {
            var task = transform.Find("Task");
            task.Find("TaskTitle").GetComponent<Text>().text = levelInfo.taskTitle;
            task.Find("TaskContent").GetComponent<Text>().text = "\n　　" + levelInfo.taskContent + "\n\n" + "胜利条件：\n　　" + levelInfo.vectoryCondition + "\n\n" + "失败条件：\n　　" + levelInfo.failureCondition;

        }
        base.Open(onInit);
    }
}
