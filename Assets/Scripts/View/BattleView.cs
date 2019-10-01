using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BattleView : ViewBase<BattleView>
{
    Transform GameStart;
    Transform RoundStart;
    Transform TurnStart;
    Transform cameraTurnLeft;
    public override void Open(UnityAction onInit = null)
    {
        if (!isInit)
        {
            GameStart = transform.Find("GameStart");
            RoundStart = transform.Find("RoundStart");
            TurnStart = transform.Find("TurnStart");
            cameraTurnLeft = transform.Find("Left");

            GameStart.gameObject.SetActive(false);
            RoundStart.gameObject.SetActive(false);
            TurnStart.gameObject.SetActive(false);

            RoundManager.GetInstance().GameStarted += () => { StartCoroutine(OnGameStart()); };
            RoundManager.GetInstance().RoundStarted += () => { StartCoroutine(OnRoundStart()); };
            RoundManager.GetInstance().TurnStarted += () => { StartCoroutine(OnTurnStart()); };

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
}
