using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour {
    private static DialogManager instance;
    private List<Unit> Units;
    private Vector3 uiOffset;
    private Dictionary<Transform, Vector3> dialogUIDic = new Dictionary<Transform, Vector3>();
    private Dictionary<string, Transform> unitsUIDic = new Dictionary<string, Transform>();
    private GameObject dialogBackground;
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
        var go1 = Resources.Load("Prefabs/UI/DialogBackground") as GameObject;
        dialogBackground = Instantiate(go1, GameObject.Find("Canvas").transform);
        dialogBackground.SetActive(false);
        foreach (var unit in Units.FindAll(u => ((CharacterStatus)u).characterIdentity == CharacterStatus.CharacterIdentity.noumenon))
        {
            var dialogUI = Instantiate(go, dialogBackground.transform);
            dialogUI.SetActive(false);
            var unitPosition = unit.GetComponent<CharacterStatus>().arrowPosition + unit.transform.position;
            unitsUIDic.Add(((CharacterStatus)unit).roleEName, dialogUI.transform);
            dialogUIDic.Add(dialogUI.transform, unitPosition);
        }

        //Talk("Naruto", "测试文本！！！");
        //Talk("Choji", "测试文本！！！");
        //Talk("Shikamaru", "测试文本！！！");
        //Talk("Kiba", "测试文本！！！");
        //Talk("Neji", "测试文本！！！");
    }

    public void OnTurnStart(object sender, EventArgs e)
    {
        //StartCoroutine(PlayDialog());
    }

    private IEnumerator PlayDialog()
    {
        dialogBackground.SetActive(true);
        yield return new WaitForSeconds(RoundManager.GetInstance().turnStartTime);
        Talk("Naruto", "我是鸣人！！！");
        yield return new WaitForSeconds(1);
        ClearDialog();
        Talk("Choji", "我是丁次！");
        yield return new WaitForSeconds(1);
        ClearDialog();
        Talk("Shikamaru", "我是鹿丸！！");
        yield return new WaitForSeconds(1);
        ClearDialog();
        Talk("Kiba", "我是牙！");
        yield return new WaitForSeconds(1);
        ClearDialog();
        Talk("Neji", "我是宁次！！！！");
        yield return new WaitForSeconds(1);
        ClearDialog();
        dialogBackground.SetActive(false);
        
    }

    public void Talk(string speaker, string content)
    {
        var dialogUI = unitsUIDic[speaker].gameObject;
        dialogUI.SetActive(true);
        dialogUI.transform.Find("Text").GetComponent<Text>().text = content;
    }

    private void ClearDialog()
    {
        foreach(var item in dialogUIDic)
        {
            item.Key.gameObject.SetActive(false);
        }
    }

    void LateUpdate()
    {
        foreach (var dialogUI in dialogUIDic)
        {
            dialogUI.Key.position = Camera.main.WorldToScreenPoint(dialogUI.Value);
        }
    }
}

public class Conversation
{
    string speaker;
    string content;
}
