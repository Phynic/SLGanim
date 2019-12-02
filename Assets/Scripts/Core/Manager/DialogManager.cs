using System;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Xml.Serialization;
using System.IO;

public class DialogManager : SingletonComponent<DialogManager>
{
    private List<Unit> Units;

    private Dictionary<Transform, Vector3> dialogUIDic = new Dictionary<Transform, Vector3>();
    private Dictionary<Unit, Transform> unitsUIDic = new Dictionary<Unit, Transform>();
    private GameObject dialogBackground;
    private bool next = false;

    private List<BattleDialog> dialogs = new List<BattleDialog>();
    void Start () {
        var go = Resources.Load("Prefabs/UI/Dialog") as GameObject;
        var go1 = Resources.Load("Prefabs/UI/DialogBackground") as GameObject;

        var list = BattleDialogDictionary.GetParamList().FindAll(d => d.levelID == Global.LevelID);

        if (dialogs.Count == 0)
        {
            Debug.Log("本场景无对话内容。");
        }
        else
        {
            dialogs = list;
        }

        dialogBackground = Instantiate(go1, GameObject.Find("Canvas").transform);
        dialogBackground.SetActive(false);

        Utils_Coroutine.GetInstance().Invoke(() =>
        {
            Units = RoundManager.GetInstance().Units;
            foreach (var unit in Units.FindAll(u => ((Unit)u).characterIdentity == Unit.CharacterIdentity.noumenon))
            {
                var dialogUI = Instantiate(go, dialogBackground.transform);
                dialogUI.SetActive(false);
                var unitPosition = unit.GetComponent<Unit>().arrowPosition + unit.transform.position;
                unitsUIDic.Add(unit, dialogUI.transform);
                dialogUIDic.Add(dialogUI.transform, unitPosition);
            }
        }, 0.1f);

    }
    
    public IEnumerator PlayDialog(int roundNumber, int playerNumber)
    {
        List<BattleDialog> conversations = new List<BattleDialog>();

        var list = dialogs.FindAll(d => d.trigger.Split('_').Length == 2);
        foreach (var item in list)
        {
            var array = item.trigger.Split('_');
            if(array[0] == roundNumber.ToString() && array[1] == playerNumber.ToString())
            {
                conversations.Add(item);
            }
        }

        if (conversations.Count > 0)
        {
            yield return StartCoroutine(Play(conversations));
        }
    }

    public IEnumerator PlayFinalDialog(bool win)
    {
        List<BattleDialog> conversations = new List<BattleDialog>();
        if (win)
            conversations = dialogs.FindAll(d => d.trigger == "win");
        else
            conversations = dialogs.FindAll(d => d.trigger == "lose");
        if(conversations.Count > 0)
        {
            yield return StartCoroutine(Play(conversations));
        }
    }
    
    IEnumerator Play(List<BattleDialog> conversations)
    {
        enabled = true;
        dialogBackground.SetActive(true);

        //更新UI位置。
        foreach (var unit in Units.FindAll(u => ((Unit)u).characterIdentity == Unit.CharacterIdentity.noumenon))
        {
            var unitPosition = unit.GetComponent<Unit>().arrowPosition + unit.transform.position;
            dialogUIDic[unitsUIDic[unit]] = unitPosition;
        }

        GameObject.Find("Canvas").transform.Find("MenuButton").gameObject.SetActive(false);
        yield return new WaitForSeconds(RoundManager.GetInstance().TurnStartTime);
        for (int i = 0; i < conversations.Count; i++)
        {
            ClearDialog();

            List<Tweener> textTweens = new List<Tweener>();
            var multi = conversations[i];
            for (int j = 0; j < multi.speakers.Length; j++)
            {
                var un = Units.Find(u => u.GetComponent<Unit>().roleEName == multi.speakers[j]);

                if (un == null)
                {
                    Debug.LogWarning("对话角色不存在！");
                    continue;
                }
                else
                {
                    if(j == 0)
                    {
                        Camera.main.GetComponent<RTSCamera>().FollowTarget(un.transform.position);
                    }
                }

                textTweens.Add(Talk(un, multi.contents[j]));
            }
            if (i == 0)
                yield return new WaitForSeconds(0.5f);
            yield return StartCoroutine(WaitNext(textTweens));
        }
        ClearDialog();
        dialogBackground.SetActive(false);
        enabled = false;
    }

    IEnumerator WaitNext(Tweener textTween)
    {
        while (true)
        {
            if (next)
            {
                next = false;
                if (textTween.IsPlaying())
                    textTween.Complete();
                else
                    break;
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    IEnumerator WaitNext(List<Tweener> textTweens)
    {
        while (true)
        {
            if (next)
            {
                next = false;
                if (textTweens[0].IsPlaying())
                {
                    foreach(var t in textTweens)
                    {
                        t.Complete();
                    }
                }
                else
                    break;
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void Next()
    {
        next = true;
    }
    
    public Tweener Talk(Unit unit, string content)
    {
        var dialogUI = unitsUIDic[unit].gameObject;
        dialogUI.SetActive(true);
        
        var sizeX = 750 + 129.3f * (content.Length > 2 ? (content.Length - 2) : 0);
        dialogUI.GetComponent<RectTransform>().sizeDelta = new Vector2(sizeX, 510);
        dialogUI.transform.Find("Text").GetComponent<RectTransform>().sizeDelta = new Vector2(sizeX, 180);

        dialogUI.transform.Find("Text").GetComponent<Text>().text = "";
        var textT = dialogUI.transform.Find("Text").GetComponent<Text>().DOText(content, content.Length * 0.1f);
        textT.SetEase(Ease.Linear);
        return textT;
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

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Next();
        }
    }
}