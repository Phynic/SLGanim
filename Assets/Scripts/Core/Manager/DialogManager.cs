using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour {
    private static DialogManager instance;
    private List<Unit> Units;
    private Vector3 unitPosition;
    private GameObject[] dialogUIList;
    private GameObject dialogUI;
    public static DialogManager GetInstance()
    {
        return instance;
    }

    void Awake()
    {
        instance = this;
    }

    void Start () {
        Units = UnitManager.GetInstance().units;
        RoundManager.GetInstance().TurnStarted += OnTurnStart;
        var go = Resources.Load("Prefabs/UI/Dialog") as GameObject;
        dialogUI = Transform.Instantiate(go, GameObject.Find("Canvas").transform);

        Talk(dialogUI, "Kiba", "测试文本！！！");
    }

    public void OnTurnStart(object sender, EventArgs e)
    {
        RoundManager.GetInstance().Invoke(() => {

        }, RoundManager.GetInstance().turnStartTime);
    }

    public void Talk(GameObject ui, string speaker, string content)
    {
        var unit = Units.Find(u => ((CharacterStatus)u).roleEName == speaker);
        dialogUI.transform.Find("Text").GetComponent<Text>().text = content;
        unitPosition = unit.GetComponent<CharacterStatus>().arrowPosition + unit.transform.position;
        dialogUI.transform.position = Camera.main.WorldToScreenPoint(unitPosition);
    }

    void LateUpdate()
    {
        dialogUI.transform.position = Camera.main.WorldToScreenPoint(unitPosition);
    }
}

public class Conversation
{
    string speaker;
    string content;
}
