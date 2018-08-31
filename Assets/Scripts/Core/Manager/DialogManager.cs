using System;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Xml.Serialization;
using System.IO;

public class DialogManager : MonoBehaviour {
    private static DialogManager instance;
    private List<Unit> Units;
    private Vector3 uiOffset;
    private Dictionary<Transform, Vector3> dialogUIDic = new Dictionary<Transform, Vector3>();
    private Dictionary<Unit, Transform> unitsUIDic = new Dictionary<Unit, Transform>();
    private GameObject dialogBackground;
    private List<Conversation> conversations = new List<Conversation>();
    private bool next = false;

    private SceneDialog sceneDialog = new SceneDialog();

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
        var go = Resources.Load("Prefabs/UI/Dialog") as GameObject;
        var go1 = Resources.Load("Prefabs/UI/DialogBackground") as GameObject;
        try
        {
            StartCoroutine(LoadDialog(Application.streamingAssetsPath + "/XML/sceneDialog_" + SceneManager.GetActiveScene().name + ".xml"));
        }
        catch
        {
            Debug.Log("本场景无对话内容。");
        }

        dialogBackground = Instantiate(go1, GameObject.Find("Canvas").transform);
        dialogBackground.SetActive(false);
        foreach (var unit in Units.FindAll(u => ((CharacterStatus)u).characterIdentity == CharacterStatus.CharacterIdentity.noumenon))
        {
            var dialogUI = Instantiate(go, dialogBackground.transform);
            dialogUI.SetActive(false);
            var unitPosition = unit.GetComponent<CharacterStatus>().arrowPosition + unit.transform.position;
            unitsUIDic.Add(unit, dialogUI.transform);
            dialogUIDic.Add(dialogUI.transform, unitPosition);
        }
        
        //var multi = new MultiConversation("Shikamaru", speakers, contents);

        //var single = new Conversation("Neji", "测试文本！");

        //var turn = new TurnDialog();
        //turn.conversations.Add(multi);
        //turn.conversations.Add(single);
        //var round = new RoundDialog();
        //round.turnDialogList.Add(turn);
        //sceneDialog.roundDialogList.Add(round);

        //SaveDialog();
    }
    
    private bool ConversationInit(int roundNumber, int playerNumber)
    {
        //Debug.Log("roundNumber : " + roundNumber);
        //Debug.Log("playerNumber : " + playerNumber);
        conversations.Clear();
        if (sceneDialog.roundDialogList.Count >= roundNumber)
            if (sceneDialog.roundDialogList[roundNumber - 1].turnDialogList.Count > playerNumber)
                conversations = sceneDialog.roundDialogList[roundNumber - 1].turnDialogList[playerNumber].conversations;
        return conversations.Count > 0;
    }

    public IEnumerator PlayDialog(int roundNumber, int playerNumber)
    {
        if (ConversationInit(roundNumber, playerNumber))
        {
            enabled = true;
            dialogBackground.SetActive(true);

            //更新UI位置。
            foreach (var unit in Units.FindAll(u => ((CharacterStatus)u).characterIdentity == CharacterStatus.CharacterIdentity.noumenon))
            {
                var unitPosition = unit.GetComponent<CharacterStatus>().arrowPosition + unit.transform.position;
                dialogUIDic[unitsUIDic[unit]] = unitPosition;
            }

            GameObject.Find("Canvas").transform.Find("MenuButton").gameObject.SetActive(false);
            yield return new WaitForSeconds(RoundManager.GetInstance().turnStartTime);
            for (int i = 0; i < conversations.Count; i++)
            {
                ClearDialog();

                var unit = Units.Find(u => u.GetComponent<CharacterStatus>().roleEName == conversations[i].speaker);
                if (unit == null)
                {
                    Debug.LogWarning("对话角色不存在！");
                    continue;
                }
                    
                Camera.main.GetComponent<RTSCamera>().FollowTarget(unit.transform.position);

                if (conversations[i] is MultiConversation)
                {
                    List<Tweener> textTweens = new List<Tweener>();
                    var multi = (MultiConversation)conversations[i];
                    for (int j = 0; j < multi.speakers.Count; j++)
                    {
                        var un = Units.Find(u => u.GetComponent<CharacterStatus>().roleEName == multi.speakers[j]);
                        textTweens.Add(Talk(un, multi.contents[j]));
                    }
                    if (i == 0)
                        yield return new WaitForSeconds(0.5f);
                    yield return StartCoroutine(WaitNext(textTweens));
                }
                else
                {
                    var textTween = Talk(unit, conversations[i].content);
                    if (i == 0)
                        yield return new WaitForSeconds(0.5f);
                    yield return StartCoroutine(WaitNext(textTween));
                }
            }
            ClearDialog();
            dialogBackground.SetActive(false);
            enabled = false;
        }
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

    //public void SaveDialog()
    //{
    //    XmlSerializer serializer = new XmlSerializer(typeof(SceneDialog));
    //    var encoding = System.Text.Encoding.GetEncoding("UTF-8");
    //    StreamWriter stream = new StreamWriter(Application.streamingAssetsPath + "/XML/sceneDialog_" + SceneManager.GetActiveScene().name + ".xml", false, encoding);
    //    serializer.Serialize(stream, sceneDialog);
    //    stream.Close();
    //}

    public IEnumerator LoadDialog(string path)
    {
        WWW www = new WWW(path);
        yield return www;

        if(www.text.Length == 0)
        {
            Debug.Log("本场景无对话内容。");
        }
        else
        {
            XmlSerializer serializer = new XmlSerializer(typeof(SceneDialog));
            StringReader sr = new StringReader(www.text);
            sr.Read();      //跳过BOM头
            sceneDialog = serializer.Deserialize(sr) as SceneDialog;
            sr.Close();
        }
    }
}


//序列化需要一个无参的构造函数。
[System.Serializable]
public class SceneDialog
{
    public List<RoundDialog> roundDialogList = new List<RoundDialog>();
}

[System.Serializable]
public class RoundDialog
{
    public List<TurnDialog> turnDialogList = new List<TurnDialog>();
}

[System.Serializable]
public class TurnDialog
{
    public List<Conversation> conversations = new List<Conversation>();
}

[System.Serializable]
[XmlInclude(typeof(MultiConversation))]
public class Conversation
{
    public string speaker;
    public string content;

    public Conversation() { }

    public Conversation(string speaker)
    {
        this.speaker = speaker;
    }

    public Conversation(string speaker, string content)
    {
        this.speaker = speaker;
        this.content = content;
    }
}

[System.Serializable]
public class MultiConversation : Conversation
{
    public List<string> speakers = new List<string>();
    public List<string> contents = new List<string>();



    public MultiConversation() { }

    public MultiConversation(string speaker, List<string> speakers, List<string> contents) : base(speaker)
    {
        this.speakers = speakers;
        this.contents = contents;
    }
}
