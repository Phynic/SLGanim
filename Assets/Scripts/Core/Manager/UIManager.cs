using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class UIManager : MonoBehaviour {
    public static UIManager instance;
    public static UIManager GetInstance()
    {
        return instance;
    }
    
    private Transform character;
    
    private List<Transform> UI = new List<Transform>();
    
    public void OnGameStart(object sender, EventArgs e)
    {
        StartCoroutine(OnGameStart());
    }

    public IEnumerator OnGameStart()
    {
        UI.Find(g => g.name == "GameStart").gameObject.SetActive(true);
        yield return new WaitForSeconds(RoundManager.GetInstance().gameStartTime);
        UI.Find(g => g.name == "GameStart").gameObject.SetActive(false);
    }

    public void OnRoundStart(object sender, EventArgs e)
    {
        StartCoroutine(OnRoundStart());
    }

    public void OnTurnStart(object sender, EventArgs e)
    {
        StartCoroutine(OnTurnStart());
    }

    public void OnUnitSelected(object sender, EventArgs e)
    {
        character = (sender as Unit).transform;
        character.GetComponent<CharacterAction>().SetSkill("FirstAction");
    }
    
    public IEnumerator OnRoundStart()
    {
        var go = UI.Find(g => g.name == "GameStart").gameObject;
        go.GetComponentInChildren<Text>().text = "第" + RoundManager.GetInstance().roundNumber + "回合";
        go.SetActive(true);
        yield return new WaitForSeconds(RoundManager.GetInstance().roundStartTime);
        go.SetActive(false);
    }

    public IEnumerator OnTurnStart()
    {
        var go = UI.Find(g => g.name == "GameStart").gameObject;
        if(RoundManager.GetInstance().CurrentPlayerNumber == 0)
        {
            go.GetComponentInChildren<Text>().text = "我方回合";
        }
        else
        {
            go.GetComponentInChildren<Text>().text = "敌方回合";
        }
        go.SetActive(true);
        yield return new WaitForSeconds(RoundManager.GetInstance().roundStartTime);
        go.SetActive(false);
    }

    public void OnRoundEnd(object sender, EventArgs e)
    {
        
    }

    void Awake()
    {
        instance = this;
    }

    void Start () {
        
        RoundManager.GetInstance().GameStarted += OnGameStart;
        RoundManager.GetInstance().RoundStarted += OnRoundStart;
        RoundManager.GetInstance().RoundEnded += OnRoundEnd;
        RoundManager.GetInstance().TurnStarted += OnTurnStart;

        UnitManager.GetInstance().units.ForEach(u => u.GetComponent<Unit>().UnitSelected += OnUnitSelected);

        UI.Add(GameObject.Find("GameStart").transform);
        UI.Add(GameObject.Find("RoundStart").transform);
        UI.Add(GameObject.Find("TurnStart").transform);

        GameObject.Find("GameStart").SetActive(false);
        GameObject.Find("RoundStart").SetActive(false);
        GameObject.Find("TurnStart").SetActive(false);
    }
	
	void Update () {
        //GetMousePosition();

        if (character)
        {
            if (Input.GetMouseButtonDown(1))
            {
                if (SkillManager.GetInstance().skillQueue.Count > 0)
                {
                    //Debug.Log("UIManager : " + SkillManager.GetInstance().skillQueue.Peek().Key.CName + " 队列剩余 " + SkillManager.GetInstance().skillQueue.Count);
                    SkillManager.GetInstance().skillQueue.Peek().Key.Reset();
                }
            }
        }
    }

    public GameObject CreateButtonList(Transform character, Skill sender, out List<GameObject> allButtons, ref Dictionary<GameObject, PrivateItemData> buttonRecord, Func<UnitSkill,bool> f)
    {
        var go = (GameObject)Resources.Load("Prefabs/UI/SkillOrToolList");
        var b = (GameObject)Resources.Load("Prefabs/UI/Button");
        var unitSkillData = character.GetComponent<CharacterStatus>().skills;
        var unitItemData = character.GetComponent<CharacterStatus>().items;
        GameObject button;
        var listUI = UnityEngine.Object.Instantiate(go, GameObject.Find("Canvas").transform);
        var UIContent = listUI.transform.Find("Scroll View").Find("Viewport").Find("Content");
        allButtons = new List<GameObject>();
        //忍术
        foreach (var skill in unitSkillData)
        {
            var tempSkill = (UnitSkill)SkillManager.GetInstance().skillList.Find(s => s.EName == skill.Key);
            //作显示数据使用。技能中使用的是深度复制实例。
            tempSkill.SetLevel(skill.Value);
            if (tempSkill != null)
            {
                if (f(tempSkill))
                {
                    button = GameObject.Instantiate(b, UIContent);
                    button.GetComponentInChildren<Text>().alignment = TextAnchor.MiddleLeft;
                    button.GetComponentInChildren<Text>().text = " " + tempSkill.CName + "   " + "消耗：" + tempSkill.costHP + "体力" + tempSkill.costMP + "查克拉";
                    button.name = skill.Key;
                    //button.GetComponent<Button>().onClick.AddListener(OnButtonClick);
                    button.GetComponent<RectTransform>().sizeDelta = new Vector2(860, 60);
                    button.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 1f);
                    allButtons.Add(button);
                    if (!tempSkill.Filter(sender))
                    {
                        button.GetComponent<Button>().interactable = false;
                    }
                }
            }
        }
        //忍具
        foreach (var item in unitItemData)
        {
            var tempItem = (INinjaTool)SkillManager.GetInstance().skillList.Find(s => s.EName == item.itemName);
            //作显示数据使用。技能中使用的是深度复制实例。
            tempItem.SetItem(item);
            var tempSkill = (UnitSkill)tempItem;
            //作显示数据使用。技能中使用的是深度复制实例。
            tempSkill.SetLevel(item.itemLevel);
            if (tempSkill != null)
            {
                if (tempSkill.skillType != UnitSkill.SkillType.dodge)
                {
                    button = GameObject.Instantiate(b, UIContent);
                    button.GetComponentInChildren<Text>().alignment = TextAnchor.MiddleLeft;
                    button.GetComponentInChildren<Text>().text = " " + tempSkill.CName;
                    button.name = item.itemName;
                    //button.GetComponent<Button>().onClick.AddListener(OnButtonClick);
                    button.GetComponent<RectTransform>().sizeDelta = new Vector2(860, 60);
                    button.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 1f);
                    allButtons.Add(button);
                    buttonRecord.Add(button, item);
                }
            }
        }
        //listUI.transform.Find("Scroll View").Find("Scrollbar Vertical").gameObject.SetActive(false);
        UIContent.GetComponent<RectTransform>().sizeDelta = new Vector2(UIContent.GetComponent<RectTransform>().sizeDelta.x, allButtons[0].GetComponent<RectTransform>().sizeDelta.y * (1.2f * (allButtons.Count - 1) + 2));

        //设置按钮位置
        for (int i = 0; i < allButtons.Count; i++)
        {
            
            allButtons[i].transform.localPosition = new Vector3(500, -(int)(i * allButtons[i].GetComponent<RectTransform>().sizeDelta.y * 1.2f), 0);
        }

        //listUI.transform.Find("Return").GetComponent<Button>().onClick.AddListener(Reset);
        listUI.SetActive(false);
        return listUI;
    }
}
