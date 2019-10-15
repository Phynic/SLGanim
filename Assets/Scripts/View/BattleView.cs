using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BattleView : ViewBase<BattleView>
{
    private Transform GameStart;
    private Transform RoundStart;
    private Transform TurnStart;
    private Transform cameraTurnLeft;

    public Button menuButton;
    public Transform debugMenu;
    public override void Open(UnityAction onInit = null)
    {
        if (!isInit)
        {
            GameStart = transform.Find("GameStart");
            RoundStart = transform.Find("RoundStart");
            TurnStart = transform.Find("TurnStart");
            cameraTurnLeft = transform.Find("Left");
            menuButton = transform.Find("MenuButton").GetComponent<Button>();
            debugMenu = transform.Find("DebugMenu");

            GameStart.gameObject.SetActive(false);
            RoundStart.gameObject.SetActive(false);
            TurnStart.gameObject.SetActive(false);

            RoundManager.GetInstance().GameStarted += () => { StartCoroutine(OnGameStart()); };
            RoundManager.GetInstance().RoundStarted += () => { StartCoroutine(OnRoundStart()); };
            RoundManager.GetInstance().TurnStarted += () => { StartCoroutine(OnTurnStart()); };
            CreateDebugMenuButton(debugMenu);
#if (!UNITY_EDITOR && (UNITY_IOS || UNITY_ANDROID))
            GameController.GetInstance().TwoTouches += BackSpace;
#endif
        }
        base.Open(onInit);
    }

    public IEnumerator OnGameStart()
    {
        GameStart.gameObject.SetActive(true);
        yield return new WaitForSeconds(RoundManager.GetInstance().GameStartTime);
        GameStart.gameObject.SetActive(false);
        cameraTurnLeft.gameObject.SetActive(true);
    }

    public IEnumerator OnRoundStart()
    {
        //转换为中文回合。
        DigitToChnText.DigitToChnText obj = new DigitToChnText.DigitToChnText();

        var go = GameStart.gameObject;
        go.GetComponentInChildren<Text>().text = "第" + obj.Convert(RoundManager.GetInstance().roundNumber.ToString(), false).ToString() + "回合";
        go.SetActive(true);
        yield return new WaitForSeconds(RoundManager.GetInstance().RoundStartTime);
        go.SetActive(false);
    }

    public IEnumerator OnTurnStart()
    {
        var go = GameStart.gameObject;
        if (RoundManager.GetInstance().CurrentPlayerNumber == 0)
        {
            go.GetComponentInChildren<Text>().text = "我方回合";
        }
        else
        {
            go.GetComponentInChildren<Text>().text = "敌方回合";
        }
        go.SetActive(true);
        yield return new WaitForSeconds(RoundManager.GetInstance().RoundStartTime);
        go.SetActive(false);
    }

    public void CreateDebugMenuButton(Transform parent)
    {
        if (parent.Find("Content").childCount > 0)
            return;
        int menuButtonNum = 5;
        List<GameObject> list = new List<GameObject>();
        for (int i = 0; i < menuButtonNum; i++)
        {
            GameObject button;
            button = PrefabManager.GetInstance().GetPrefabIns("Prefabs/UI/Button", Vector3.zero, Vector3.one, parent.Find("Content"));
            button.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 60);
            button.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 1f);
            button.GetComponent<RectTransform>().anchorMin = new Vector2(0, 1);
            button.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);
            button.transform.localPosition = new Vector3(0, -(int)(i * button.GetComponent<RectTransform>().sizeDelta.y), 0);
            list.Add(button);
        }


        list[0].GetComponentInChildren<Text>().text = "结束回合";
        list[0].name = "EndTurnButton";
        list[0].GetComponent<Button>().onClick.AddListener(RoundManager.GetInstance().ForceEndTurn);
        list[0].GetComponent<Button>().onClick.AddListener(() => { parent.gameObject.SetActive(false); });

        list[1].GetComponentInChildren<Text>().text = "重新开始";
        list[1].name = "RestartButton";
        list[1].GetComponent<Button>().onClick.AddListener(RoundManager.GetInstance().Restart);

        list[2].GetComponentInChildren<Text>().text = "结束游戏";
        list[2].name = "ExitButton";
        list[2].GetComponent<Button>().onClick.AddListener(GameManager.GetInstance().Exit);

        list[4].GetComponentInChildren<Text>().text = "关闭菜单";
        list[4].name = "CloseMenuButton";
        list[4].GetComponent<Button>().onClick.AddListener(() => { parent.gameObject.SetActive(false); });
    }
}
