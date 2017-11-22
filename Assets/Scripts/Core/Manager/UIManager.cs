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

    private List<Sprite> imagesList = new List<Sprite>();

    private GameObject _SkillOrToolList;
    private GameObject _Button;
    private GameObject _SkillButtonImages;
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
        //转换为中文回合。
        DigitToChnText.DigitToChnText obj = new DigitToChnText.DigitToChnText();
        

        var go = UI.Find(g => g.name == "GameStart").gameObject;
        go.GetComponentInChildren<Text>().text = "第" + obj.Convert(RoundManager.GetInstance().roundNumber.ToString(), false).ToString() + "回合";
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
        

        _SkillOrToolList = (GameObject)Resources.Load("Prefabs/UI/SkillOrToolList");
        _Button = (GameObject)Resources.Load("Prefabs/UI/Button");
        _SkillButtonImages = (GameObject)Resources.Load("Prefabs/UI/SkillButtonImages");

        var images = Resources.LoadAll("Textures/SkillButtonImages", typeof(Sprite));

        foreach (var i in images)
        {
            imagesList.Add((Sprite)i);
        }

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
        var unitSkillData = character.GetComponent<CharacterStatus>().skills;
        var unitItemData = character.GetComponent<CharacterStatus>().items;
        GameObject button;
        var listUI = UnityEngine.Object.Instantiate(_SkillOrToolList, GameObject.Find("Canvas").transform);
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
                
                button = GameObject.Instantiate(_Button, UIContent);
                
                button.GetComponentInChildren<Text>().alignment = TextAnchor.MiddleLeft;
                button.GetComponentInChildren<Text>().text = tempSkill.CName;
                button.GetComponentInChildren<Text>().resizeTextForBestFit = false;
                button.GetComponentInChildren<Text>().fontSize = 45;
                button.GetComponentInChildren<Text>().GetComponent<RectTransform>().sizeDelta = new Vector2(-30, 0);
                button.name = skill.Key;
                //button.GetComponent<Button>().onClick.AddListener(OnButtonClick);
                button.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 72);
                button.GetComponent<RectTransform>().pivot = new Vector2(0f, 1f);
                button.GetComponent<RectTransform>().anchorMin = new Vector2(0, 1);
                button.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);
                allButtons.Add(button);
                if (!f(tempSkill) || !tempSkill.Filter(sender))
                {
                    button.GetComponent<Button>().interactable = false;
                    button.GetComponentInChildren<Text>().color = new Color(0.6f, 0.6f, 0.6f);
                }
                EventTriggerListener.Get(button).onEnter = g => {
                    LogSkillInfo(tempSkill, listUI);
                };

                var imageUI = UnityEngine.Object.Instantiate(_SkillButtonImages, button.transform);

                var _Class = imageUI.transform.Find("SkillClass").GetComponent<Image>();
                var _Type = imageUI.transform.Find("SkillType").GetComponent<Image>();
                var _Combo = imageUI.transform.Find("SkillCombo").GetComponent<Image>();
                //Debug.Log(imagesList[0].name.Substring(11));
                _Class.sprite = imagesList.Find(i => i.name.Substring(11) == tempSkill.skillClass.ToString());
                _Type.sprite = imagesList.Find(i => i.name.Substring(10) == tempSkill.skillType.ToString());
                _Combo.gameObject.SetActive(tempSkill.comboType != UnitSkill.ComboType.cannot);
            }
        }
        //忍具
        foreach (var item in unitItemData)
        {
            var t = SkillManager.GetInstance().skillList.Find(s => s.EName == item.itemName).GetType();
            //作显示数据使用。技能中使用的是深度复制实例。
            var tempItem = Activator.CreateInstance(t) as INinjaTool;
            tempItem.SetItem(item);
            var tempSkill = (UnitSkill)tempItem;
            //作显示数据使用。技能中使用的是深度复制实例。
            if (tempSkill != null)
            {
                button = GameObject.Instantiate(_Button, UIContent);
                button.GetComponentInChildren<Text>().alignment = TextAnchor.MiddleLeft;
                button.GetComponentInChildren<Text>().text = tempSkill.CName;
                button.GetComponentInChildren<Text>().GetComponent<RectTransform>().sizeDelta = new Vector2(-30, 0);
                button.GetComponentInChildren<Text>().resizeTextForBestFit = false;
                button.GetComponentInChildren<Text>().fontSize = 45;
                button.name = item.itemName;
                //button.GetComponent<Button>().onClick.AddListener(OnButtonClick);
                button.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 72);
                button.GetComponent<RectTransform>().pivot = new Vector2(0f, 1f);
                button.GetComponent<RectTransform>().anchorMin = new Vector2(0, 1);
                button.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);
                allButtons.Add(button);
                buttonRecord.Add(button, item);

                if (!f(tempSkill) || !tempSkill.Filter(sender))
                {
                    button.GetComponent<Button>().interactable = false;
                    button.GetComponentInChildren<Text>().color = new Color(0.6f, 0.6f, 0.6f);
                }

                EventTriggerListener.Get(button).onEnter = g => {
                    LogSkillInfo(tempSkill, listUI);
                };

                var imageUI = UnityEngine.Object.Instantiate(_SkillButtonImages, button.transform);

                var _Class = imageUI.transform.Find("SkillClass").GetComponent<Image>();
                var _Type = imageUI.transform.Find("SkillType").GetComponent<Image>();
                var _Combo = imageUI.transform.Find("SkillCombo").GetComponent<Image>();
                //Debug.Log(imagesList[0].name.Substring(11));
                _Class.sprite = imagesList.Find(i => i.name.Substring(11) == tempSkill.skillClass.ToString());
                _Type.sprite = imagesList.Find(i => i.name.Substring(10) == tempSkill.skillType.ToString());
                _Combo.gameObject.SetActive(tempSkill.comboType != UnitSkill.ComboType.cannot);

            }
        }
        //listUI.transform.Find("Scroll View").Find("Scrollbar Vertical").gameObject.SetActive(false);
        UIContent.GetComponent<RectTransform>().sizeDelta = new Vector2(UIContent.GetComponent<RectTransform>().sizeDelta.x, allButtons[0].GetComponent<RectTransform>().sizeDelta.y * (allButtons.Count));

        //设置按钮位置
        for (int i = 0; i < allButtons.Count; i++)
        {
            
            allButtons[i].transform.localPosition = new Vector3(0, -(int)(i * (allButtons[i].GetComponent<RectTransform>().sizeDelta.y)), 0);
            
        }

        
        //信息显示
        listUI.transform.Find("RoleNamePanel").GetComponentInChildren<Text>().text = character.GetComponent<CharacterStatus>().roleCName;

        var currentHP = character.GetComponent<CharacterStatus>().attributes.Find(d => d.eName == "hp").value;
        var currentMP = character.GetComponent<CharacterStatus>().attributes.Find(d => d.eName == "mp").value;

        listUI.transform.Find("RoleInfoPanel").Find("Info").GetComponentInChildren<Text>().text = currentHP + "\n" + currentMP;
        
        return listUI;
    }
    
    public void  LogSkillInfo(UnitSkill unitSkill, GameObject listUI)
    {
        var costTitle = listUI.transform.Find("SkillInfoPanel").Find("CostTitle").GetComponent<Text>();
        var effectTitle = listUI.transform.Find("SkillInfoPanel").Find("EffectTitle").GetComponent<Text>();
        var distanceTitle = listUI.transform.Find("SkillInfoPanel").Find("DistanceTitle").GetComponent<Text>();
        var rangeTitle = listUI.transform.Find("SkillInfoPanel").Find("RangeTitle").GetComponent<Text>();
        var durationTitle = listUI.transform.Find("SkillInfoPanel").Find("DurationTitle").GetComponent<Text>();
        var rateTitle = listUI.transform.Find("SkillInfoPanel").Find("RateTitle").GetComponent<Text>();

        var costInfo = listUI.transform.Find("SkillInfoPanel").Find("CostInfo").GetComponent<Text>();
        var effectInfo = listUI.transform.Find("SkillInfoPanel").Find("EffectInfo").GetComponent<Text>();
        var distanceInfo = listUI.transform.Find("SkillInfoPanel").Find("DistanceInfo").GetComponent<Text>();
        var rangeInfo = listUI.transform.Find("SkillInfoPanel").Find("RangeInfo").GetComponent<Text>();
        var durationInfo = listUI.transform.Find("SkillInfoPanel").Find("DurationInfo").GetComponent<Text>();
        var rateInfo = listUI.transform.Find("SkillInfoPanel").Find("RateInfo").GetComponent<Text>();


        var skillDescription = listUI.transform.Find("DescriptionPanel").Find("SkillDescription").GetComponent<Text>();
        
        switch (unitSkill.skillClass)
        {
            case UnitSkill.SkillClass.ninjutsu:
                costTitle.text = "消耗查克拉";
                costInfo.text = unitSkill.costMP.ToString();
                break;
            case UnitSkill.SkillClass.taijutsu:
                costTitle.text = "消耗体力";
                costInfo.text = unitSkill.costHP.ToString();
                break;
            default:
                costTitle.text = "";
                costInfo.text = "";
                break;
        }

        var skillLog = unitSkill.LogSkillEffect();

        effectTitle.text = skillLog[0];
        effectInfo.text = skillLog[1];

        if (unitSkill.skillRange > 0)
        {
            distanceTitle.text = "距离";
            distanceInfo.text = unitSkill.skillRange.ToString();
        }
        else
        {
            distanceTitle.text = "";
            distanceInfo.text = "";
        }
        if (unitSkill.hoverRange >= 0 && unitSkill.skillRange > 0)
        {
            rangeTitle.text = "范围";
            rangeInfo.text = (unitSkill.hoverRange + 1).ToString();
            switch (unitSkill.rangeType)
            {
                case UnitSkill.RangeType.common:
                    rangeTitle.text += "      普通型";
                    
                    break;
                case UnitSkill.RangeType.straight:
                    rangeTitle.text += "      直线型";
                    break;
            }
        }
        else
        {
            rangeTitle.text = "";
            rangeInfo.text = "";
        }

        durationTitle.text = "";
        durationInfo.text = "";

        if (skillLog.Count == 3)
        {
            durationTitle.text = "持续回合";
            durationInfo.text = skillLog[2];
        }
        
        rateTitle.text = "成功率";
        rateInfo.text = unitSkill.skillRate + "%";


        skillDescription.text = unitSkill.CName + "\n" +unitSkill.description;
    }
}