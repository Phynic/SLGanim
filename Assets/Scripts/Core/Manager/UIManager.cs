using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using DG.Tweening;

public class UIManager : MonoBehaviour {
    public static UIManager instance;
    public static UIManager GetInstance()
    {
        return instance;
    }

    public static Color hpColor = new Color(248f / 255f, 168f / 255f, 0f);
    public static Color mpColor = new Color(80f / 255f, 248f / 255f, 144f / 255f);

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
        _SkillButtonImages = (GameObject)Resources.Load("Prefabs/UI/SkillButtonImages_Single");

        var images = Resources.LoadAll("Textures/SkillButtonImages/Single", typeof(Sprite));

        foreach (var i in images)
        {
            imagesList.Add((Sprite)i);
        }

#if (!UNITY_EDITOR && (UNITY_IOS || UNITY_ANDROID))
        GameController.GetInstance().ThreeTouches += BackSpace;
#endif
    }

    void Update () {
        //GetMousePosition();
#if (UNITY_STANDALONE || UNITY_EDITOR)
        if (Input.GetMouseButtonDown(1))
        {
            BackSpace(this, null);
        }
#endif
    }

    private void BackSpace(object sender, EventArgs e)
    {
        if (SkillManager.GetInstance().skillQueue.Count > 0)
        {
            if (character)
            {
                //Debug.Log("UIManager : " + SkillManager.GetInstance().skillQueue.Peek().Key.CName + " 队列剩余 " + SkillManager.GetInstance().skillQueue.Count);
                if (!SkillManager.GetInstance().skillQueue.Peek().Key.done)
                {
                    SkillManager.GetInstance().skillQueue.Peek().Key.Reset();
                }
            }
        }
        else
        {
            var outline = Camera.main.GetComponent<RenderBlurOutline>();
            if (outline)
                outline.CancelRender();
            foreach (var f in BattleFieldManager.GetInstance().floors)
            {
                f.Value.SetActive(false);
            }
            if (RoundManager.GetInstance().RoundState != null)
                ((RoundStateWaitingForInput)RoundManager.GetInstance().RoundState).DestroyPanel();
        }
    }

    public GameObject CreateButtonList(Transform character, Skill sender, out List<GameObject> allButtons, ref Dictionary<GameObject, PrivateItemData> buttonRecord, Func<UnitSkill,bool> f)
    {
        
        var unitSkillData = character.GetComponent<CharacterStatus>().skills;
        var unitItemData = character.GetComponent<CharacterStatus>().items;
        GameObject button;
        var listUI = UnityEngine.Object.Instantiate(_SkillOrToolList, GameObject.Find("Canvas").transform);
        var UIContent = listUI.transform.Find("SkillPanel").Find("Scroll View").Find("Viewport").Find("Content");
        var skillInfoPanel = listUI.transform.Find("SkillInfoPanel");
        skillInfoPanel.gameObject.SetActive(false);
        var descriptionPanel = listUI.transform.Find("DescriptionPanel");
        descriptionPanel.gameObject.SetActive(false);
        allButtons = new List<GameObject>();

        var roleInfoPanel = CreateRoleInfoPanel(character);
        roleInfoPanel.transform.SetParent(listUI.transform);
        
        //忍术
        foreach (var skill in unitSkillData)
        {
            var tempSkill = (UnitSkill)SkillManager.GetInstance().skillList.Find(s => (s is UnitSkill && s.EName == skill.Key));
            //作显示数据使用。技能中使用的是深度复制实例。

            if (tempSkill != null && skill.Value > 0)   //等级大于0。
            {
                tempSkill.SetLevel(skill.Value);
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

                EventTriggerListener.Get(button).onEnter = g =>
                {
                    skillInfoPanel.gameObject.SetActive(true);
                    if (tempSkill.description.Length > 0)
                        descriptionPanel.gameObject.SetActive(true);
                    LogSkillInfo(tempSkill, descriptionPanel, skillInfoPanel, g.transform);
                };

                EventTriggerListener.Get(button).onExit = g =>
                {
                    skillInfoPanel.gameObject.SetActive(false);
                    descriptionPanel.gameObject.SetActive(false);
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
        //高级分身无法使用忍具
        if(character.GetComponent<CharacterStatus>().characterIdentity == CharacterStatus.CharacterIdentity.noumenon)
        {
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

                    EventTriggerListener.Get(button).onEnter = g =>
                    {
                        skillInfoPanel.gameObject.SetActive(true);
                        if(tempSkill.description.Length > 0)
                            descriptionPanel.gameObject.SetActive(true);
                        LogSkillInfo(tempSkill, descriptionPanel, skillInfoPanel, g.transform);
                    };

                    EventTriggerListener.Get(button).onExit = g =>
                    {
                        skillInfoPanel.gameObject.SetActive(false);
                        descriptionPanel.gameObject.SetActive(false);
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
        }
        //listUI.transform.Find("Scroll View").Find("Scrollbar Vertical").gameObject.SetActive(false);
        UIContent.GetComponent<RectTransform>().sizeDelta = new Vector2(UIContent.GetComponent<RectTransform>().sizeDelta.x, allButtons[0].GetComponent<RectTransform>().sizeDelta.y * (allButtons.Count));

        //设置按钮位置
        for (int i = 0; i < allButtons.Count; i++)
        {
            
            allButtons[i].transform.localPosition = new Vector3(0, -(int)(i * (allButtons[i].GetComponent<RectTransform>().sizeDelta.y)), 0);
            
        }
        
        //信息显示
        //listUI.transform.Find("RoleNamePanel").GetComponentInChildren<Text>().text = character.GetComponent<CharacterStatus>().roleCName;

        //var currentHP = character.GetComponent<CharacterStatus>().attributes.Find(d => d.eName == "hp").value;
        //var currentMP = character.GetComponent<CharacterStatus>().attributes.Find(d => d.eName == "mp").value;

        //listUI.transform.Find("RoleInfoPanel").Find("Info").GetComponentInChildren<Text>().text = currentHP + "\n" + currentMP;

        if(sender is UnitSkill)
        {
            listUI.transform.Find("DescriptionPanel").Find("SkillDescription").Find("SkillCombo").gameObject.SetActive(true);
        }
        else
        {
            listUI.transform.Find("DescriptionPanel").Find("SkillDescription").Find("SkillCombo").gameObject.SetActive(false);
        }

        
        return listUI;
    }
    
    public void CreateDebugMenuButton(Transform parent)
    {
        if (parent.Find("Content").childCount > 0)
            return;
        int menuButtonNum = 5;
        List<GameObject> list = new List<GameObject>();
        for(int i = 0; i < menuButtonNum; i++)
        {
            GameObject button;
            button = GameObject.Instantiate(_Button, parent.Find("Content"));
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
        list[2].GetComponent<Button>().onClick.AddListener(RoundManager.GetInstance().Exit);

        list[4].GetComponentInChildren<Text>().text = "关闭菜单";
        list[4].name = "CloseMenuButton";
        list[4].GetComponent<Button>().onClick.AddListener(() => { parent.gameObject.SetActive(false); });
    }

    private void LogSkillInfo(UnitSkill unitSkill, Transform descriptionPanel, Transform skillInfoPanel, Transform button)
    {
        //确保不出边界。
        
        var syncY = button.position.y - button.GetComponent<RectTransform>().sizeDelta.y / 2;

        var minY = descriptionPanel.parent.position.y + skillInfoPanel.GetComponent<RectTransform>().sizeDelta.y / 2 + 13.4f;

        var y = syncY > minY ? syncY : minY;

        //Debug.Log("y:" + y + " syncY:" + syncY + " minY:" + minY);

        skillInfoPanel.position = new Vector3(skillInfoPanel.position.x, y, skillInfoPanel.position.z);
        
        var costTitle = skillInfoPanel.Find("CostTitle").GetComponent<Text>();
        var effectTitle = skillInfoPanel.Find("EffectTitle").GetComponent<Text>();
        var distanceTitle = skillInfoPanel.Find("DistanceTitle").GetComponent<Text>();
        var rangeTitle = skillInfoPanel.Find("RangeTitle").GetComponent<Text>();
        var durationTitle = skillInfoPanel.Find("DurationTitle").GetComponent<Text>();
        var rateTitle = skillInfoPanel.Find("RateTitle").GetComponent<Text>();

        var costInfo = skillInfoPanel.Find("CostInfo").GetComponent<Text>();
        var effectInfo = skillInfoPanel.Find("EffectInfo").GetComponent<Text>();
        var distanceInfo = skillInfoPanel.Find("DistanceInfo").GetComponent<Text>();
        var rangeInfo = skillInfoPanel.Find("RangeInfo").GetComponent<Text>();
        var durationInfo = skillInfoPanel.Find("DurationInfo").GetComponent<Text>();
        var rateInfo = skillInfoPanel.Find("RateInfo").GetComponent<Text>();


        var skillDescription = descriptionPanel.Find("SkillDescription").GetComponent<Text>();
        
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

        //if (descriptionPanel.Find("SkillDescription").Find("SkillCombo").gameObject.activeInHierarchy)
        //{
        //    skillDescription.text = "  " + unitSkill.description;
        //}
        //else
        //{
        //    skillDescription.text = unitSkill.description;
        //}
        skillDescription.text = unitSkill.description;
    }

    public GameObject CreateRoleInfoPanel(Transform character)
    {
        //GameObject roleInfoPanel = GameObject.Find("Canvas")?.transform.Find("RoleInfoPanel(Clone)")?.gameObject;
        //if(roleInfoPanel == null)
        GameObject roleInfoPanel = GameObject.Instantiate((GameObject)Resources.Load("Prefabs/UI/RoleInfoPanel"), GameObject.Find("Canvas").transform);
        
        var roleName = roleInfoPanel.transform.Find("Content").Find("RoleName");
        var roleIdentity = roleInfoPanel.transform.Find("Content").Find("RoleIdentity");
        var roleState = roleInfoPanel.transform.Find("Content").Find("RoleState");
        var healthSlider = roleInfoPanel.transform.Find("Content").Find("Health");
        var chakraSlider = roleInfoPanel.transform.Find("Content").Find("Chakra");
        var info = roleInfoPanel.transform.Find("Content").Find("Info");

        roleName.GetComponent<Text>().text = character.GetComponent<CharacterStatus>().roleCName.Replace(" ", "");
        roleIdentity.GetComponent<Text>().text = character.GetComponent<CharacterStatus>().identity;
        roleState.GetComponent<Text>().text = character.GetComponent<Unit>().UnitEnd ? "结束" : "待机";
        roleState.GetComponent<Text>().color = character.GetComponent<Unit>().UnitEnd ? new Color(255, 0, 0) : new Color(112.0f / 255.0f, 32.0f / 255.0f, 248.0f / 255.0f);
        healthSlider.GetComponent<Slider>().maxValue = character.GetComponent<CharacterStatus>().attributes.Find(d => d.eName == "hp").valueMax;
        healthSlider.GetComponent<Slider>().value = character.GetComponent<CharacterStatus>().attributes.Find(d => d.eName == "hp").value;
        chakraSlider.GetComponent<Slider>().maxValue = character.GetComponent<CharacterStatus>().attributes.Find(d => d.eName == "mp").valueMax;
        chakraSlider.GetComponent<Slider>().value = character.GetComponent<CharacterStatus>().attributes.Find(d => d.eName == "mp").value;
        info.GetComponent<Text>().text = healthSlider.GetComponent<Slider>().value + "\n" + chakraSlider.GetComponent<Slider>().value;
        
        return roleInfoPanel;
    }

    Dictionary<Transform, Vector3> flyNums = new Dictionary<Transform, Vector3>();

    public void FlyNum(Vector3 position, string value, Color color, bool canRepeat = false)
    {
        if (value.Length > 4)
        {
            Debug.LogError("伤害显示溢出！");
        }
        else
        {
            GameObject flyNum = GameObject.Instantiate((GameObject)Resources.Load("Prefabs/UI/FlyNum"), GameObject.Find("Canvas").transform);
            flyNum.transform.position = Camera.main.WorldToScreenPoint(position);
            var flyNumText1 = flyNum.transform.Find("1").GetComponent<Text>();
            flyNumText1.text = value[0].ToString();
            flyNumText1.color = color;
            if (value.Length >= 2)
            {
                var flyNumText2 = flyNum.transform.Find("2").GetComponent<Text>();
                flyNumText2.text = value[1].ToString();
                flyNumText2.color = color;
            }
            else
                flyNum.transform.Find("2").GetComponent<Text>().text = "";
            if (value.Length >= 3)
            {
                var flyNumText3 = flyNum.transform.Find("3").GetComponent<Text>();
                flyNumText3.text = value[2].ToString();
                flyNumText3.color = color;
            }
            else
                flyNum.transform.Find("3").GetComponent<Text>().text = "";
            if (value.Length >= 4)
            {
                var flyNumText4 = flyNum.transform.Find("4").GetComponent<Text>();
                flyNumText4.text = value[3].ToString();
                flyNumText4.color = color;
            }
            else
                flyNum.transform.Find("4").GetComponent<Text>().text = "";


            flyNums.Add(flyNum.transform, position);

            if (canRepeat)
            {
                var n1 = flyNum.transform.Find("1");
                var n2 = flyNum.transform.Find("2");
                var n3 = flyNum.transform.Find("3");
                var n4 = flyNum.transform.Find("4");
                float randomValue1 = UnityEngine.Random.Range(-1f, 1f);
                float randomValue2 = UnityEngine.Random.Range(-1f, 1f);
                float randomValue3 = UnityEngine.Random.Range(-1f, 1f);
                SetFlyNum(n1, randomValue1, randomValue2, randomValue3);
                SetFlyNum(n2, randomValue1, randomValue2, randomValue3);
                SetFlyNum(n3, randomValue1, randomValue2, randomValue3);
                SetFlyNum(n4, randomValue1, randomValue2, randomValue3);
            }
            else
            {
                var factor = 25;
                RoundManager.GetInstance().Invoke(() => { flyNum.transform.Find("1").DOPunchPosition(Vector3.up * factor, 0.6f, 1, 0, true); }, 0.09f);
                RoundManager.GetInstance().Invoke(() => { flyNum.transform.Find("2").DOPunchPosition(Vector3.up * factor, 0.6f, 1, 0, true); }, 0.18f);
                RoundManager.GetInstance().Invoke(() => { flyNum.transform.Find("3").DOPunchPosition(Vector3.up * factor, 0.6f, 1, 0, true); }, 0.27f);
                RoundManager.GetInstance().Invoke(() => { flyNum.transform.Find("4").DOPunchPosition(Vector3.up * factor, 0.6f, 1, 0, true); }, 0.36f);
            }

            RoundManager.GetInstance().Invoke(() => {
                flyNums.Remove(flyNum.transform);
                Destroy(flyNum);
            }, 1.5f);
        }
    }

    public void SetFlyNum(Transform n, float random1, float random2, float random3)
    {
        var t0 = n.DOMoveY(n.position.y + 80 + random1 * 30, 0.2f);
        t0.SetEase(Ease.OutQuad);
        var t1 = n.DOMoveY(n.position.y + random2 * 20, 1f);
        t1.SetEase(Ease.OutBounce);

        var dir = random1 < 0 ? -1 : 1;
        var t2 = n.DOMoveX(n.position.x + (170 + random3 * 20) * dir, 1.2f);
        t2.SetEase(Ease.OutQuad);
        var t3 = n.GetComponent<Text>().DOFade(0, 1.2f);
        t3.SetEase(Ease.InExpo);

        Sequence s1 = DOTween.Sequence();
        s1.Append(t0);
        s1.Append(t1);
    }

    private void LateUpdate()
    {
        foreach(var flyNum in flyNums)
        {
            flyNum.Key.position = Camera.main.WorldToScreenPoint(flyNum.Value);
        }
    }
}