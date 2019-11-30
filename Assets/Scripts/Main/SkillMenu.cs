using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SkillMenu : MonoBehaviour {
    
    private GameObject _Button;
    private GameObject _SkillButtonImages;
    private GameObject _SkillLevelImages;
    private GameObject _LevelChange;
    private List<Sprite> imagesList = new List<Sprite>();
    private List<GameObject> allButtons = new List<GameObject>();
    private Transform character;
    private void Awake()
    {
        _Button = (GameObject)Resources.Load("Prefabs/UI/Button");
        _SkillButtonImages = (GameObject)Resources.Load("Prefabs/UI/SkillButtonImages_Single");
        _SkillLevelImages = (GameObject)Resources.Load("Prefabs/UI/SkillLevelImages");
        _LevelChange = (GameObject)Resources.Load("Prefabs/UI/LevelChange");
        var images = Resources.LoadAll("Textures/SkillButtonImages/Single", typeof(Sprite));

        foreach (var i in images)
        {
            imagesList.Add((Sprite)i);
        }
    }

    public void UpdateView(Transform character)
    {
        this.character = character;
        gameObject.SetActive(true);
        foreach (var b in allButtons)
        {
            Destroy(b);
        }
        CreateSkillList(character);
    }

    public void CreateSkillList(Transform character)
    {
        var unitSkillData = Global.characterDataList.Find(d => d.characterInfoID == character.GetComponent<CharacterStatus>().characterInfoID).skills;
        var UIContent = transform.Find("Scroll View").Find("Viewport").Find("Content");
        var skillInfoPanel = transform.Find("SkillInfoPanel");
        var descriptionPanel = transform.Find("DescriptionPanel");
        skillInfoPanel.gameObject.SetActive(false);
        descriptionPanel.gameObject.SetActive(false);
        var roleInfoPanel = transform.parent;

        allButtons.Clear();
        GameObject button;
        foreach (var skill in unitSkillData)
        {
            //深度复制
            Type t = SkillManager.GetInstance().skillList.Find(s => s.SkillInfoID == skill.skillInfoID).GetType();
            var tempSkill = Activator.CreateInstance(t) as Skill;

            tempSkill.SetLevel(skill.level > 0 ? skill.level : 1);
            button = GameObject.Instantiate(_Button, UIContent);
            
            Destroy(button.GetComponent<Button>());

            button.GetComponentInChildren<Text>().alignment = TextAnchor.MiddleLeft;
            button.GetComponentInChildren<Text>().text = tempSkill.CName;
            button.GetComponentInChildren<Text>().resizeTextForBestFit = false;
            button.GetComponentInChildren<Text>().fontSize = 45;
            button.GetComponentInChildren<Text>().GetComponent<RectTransform>().sizeDelta = new Vector2(-30, 0);
            button.name = SkillInfoDictionary.GetParam(skill.skillInfoID).ID.ToString();
            
            button.GetComponent<RectTransform>().sizeDelta = new Vector2(-72 * 2, 72);
            
            button.GetComponent<RectTransform>().pivot = new Vector2(0f, 1f);
            button.GetComponent<RectTransform>().anchorMin = new Vector2(0, 1);
            button.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);
            allButtons.Add(button);

            var levelChange = GameObject.Instantiate(_LevelChange, button.transform);

            levelChange.transform.Find("LevelUp").GetComponent<Button>().onClick.AddListener(OnButtonClick);
            levelChange.transform.Find("LevelDown").GetComponent<Button>().onClick.AddListener(OnButtonClick);

            //技能等级小于零 约定为技能未解锁。
            if (skill.level < 0)
            {
                button.GetComponentInChildren<Text>().color = new Color(0.6f, 0.6f, 0.6f);
                levelChange.transform.Find("LevelUp").GetComponent<Button>().interactable = false;
                levelChange.transform.Find("LevelDown").GetComponent<Button>().interactable = false;
                levelChange.transform.Find("LevelUp").GetComponentInChildren<Text>().color = new Color(0.6f, 0.6f, 0.6f);
                levelChange.transform.Find("LevelDown").GetComponentInChildren<Text>().color = new Color(0.6f, 0.6f, 0.6f);
            }

            var imageUI = UnityEngine.Object.Instantiate(_SkillButtonImages, button.transform);

            var _Class = imageUI.transform.Find("SkillClass").GetComponent<Image>();
            var _Type = imageUI.transform.Find("SkillType").GetComponent<Image>();
            var _Combo = imageUI.transform.Find("SkillCombo").GetComponent<Image>();

            var halfSize = imageUI.GetComponent<RectTransform>().rect.width / 2;

            _Class.transform.localPosition = new Vector3(halfSize - 320, 0, 0);
            _Type.transform.localPosition = new Vector3(halfSize - 260, 0, 0);
            _Combo.transform.localPosition = new Vector3(halfSize - 200, 0, 0);

            if (tempSkill is UnitSkill)
            {
                var tempUnitSkill = (UnitSkill)tempSkill;
                _Class.sprite = imagesList.Find(i => i.name.Substring(11) == tempUnitSkill.skillInfo.skillClass.ToString());
                _Type.sprite = imagesList.Find(i => i.name.Substring(10) == tempUnitSkill.skillInfo.skillType.ToString());
                _Combo.gameObject.SetActive(tempUnitSkill.skillInfo.comboType != ComboType.cannot);
            }
            else
            {
                _Class.sprite = imagesList.Find(i => i.name.Substring(11) == SkillClass.passive.ToString());
                _Type.gameObject.SetActive(false);
                _Combo.gameObject.SetActive(false);
            }


            EventTriggerListener.Get(button).onEnter = g =>
            {
                if (tempSkill.skillInfo.description.Length > 0)
                    descriptionPanel.gameObject.SetActive(true);
                if (tempSkill is UnitSkill)
                    skillInfoPanel.gameObject.SetActive(true);
                LogSkillInfo(tempSkill, descriptionPanel, skillInfoPanel, roleInfoPanel, g.transform);
            };

            EventTriggerListener.Get(button).onExit = g =>
            {
                skillInfoPanel.gameObject.SetActive(false);
                descriptionPanel.gameObject.SetActive(false);
            };

            var levelUI = UnityEngine.Object.Instantiate(_SkillLevelImages, button.transform);

            for (int i = 0; tempSkill.skillInfo.maxLevel > i; i++)
            {
                var toggle = levelUI.transform.Find("Level" + (i + 1).ToString()).gameObject;
                toggle.SetActive(true);
                if (skill.level > i)
                {
                    toggle.GetComponent<Toggle>().isOn = true;
                }
            }
        }

        UIContent.GetComponent<RectTransform>().sizeDelta = new Vector2(UIContent.GetComponent<RectTransform>().sizeDelta.x, allButtons[0].GetComponent<RectTransform>().sizeDelta.y * (allButtons.Count));

        //设置按钮位置
        for (int i = 0; i < allButtons.Count; i++)
        {
            allButtons[i].transform.localPosition = new Vector3(allButtons[i].transform.localPosition.x, -(int)(i * (allButtons[i].GetComponent<RectTransform>().sizeDelta.y)), 0);
        }
    }

    //A method copied from UIManager
    private void LogSkillInfo(Skill skill, Transform descriptionPanel, Transform skillInfoPanel, Transform roleInfoPanel, Transform button)
    {
        //确保不出边界。
        var skillPanelRect = descriptionPanel.parent.GetComponent<RectTransform>();
        var skillInfoPanelRect = skillInfoPanel.GetComponent<RectTransform>();

        var syncY = button.position.y - button.GetComponent<RectTransform>().sizeDelta.y / 2;
        var minY = descriptionPanel.parent.position.y + skillInfoPanelRect.sizeDelta.y / 2 * skillInfoPanelRect.lossyScale.y;
        var maxY = descriptionPanel.parent.position.y + skillPanelRect.sizeDelta.y * skillPanelRect.lossyScale.y - skillInfoPanelRect.sizeDelta.y / 2 * skillInfoPanelRect.lossyScale.y;

        float y;
        if (syncY >= maxY)
            y = maxY;
        else if (syncY <= minY)
            y = minY;
        else
            y = syncY;

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
        skillDescription.text = skill.skillInfo.description;

        UnitSkill unitSkill;

        if (skill is UnitSkill)
            unitSkill = (UnitSkill)skill;
        else
            return;

        switch (unitSkill.skillInfo.skillClass)
        {
            case SkillClass.ninjutsu:
                costTitle.text = "消耗查克拉";
                costInfo.text = unitSkill.skillInfo.costMP.ToString();
                break;
            case SkillClass.taijutsu:
                costTitle.text = "消耗体力";
                costInfo.text = unitSkill.skillInfo.costHP.ToString();
                break;
            default:
                costTitle.text = "";
                costInfo.text = "";
                break;
        }

        var skillLog = unitSkill.LogSkillEffect();

        effectTitle.text = skillLog[0];
        effectInfo.text = skillLog[1];

        if (unitSkill.skillInfo.skillRange > 0)
        {
            distanceTitle.text = "距离";
            distanceInfo.text = unitSkill.skillInfo.skillRange.ToString();
        }
        else
        {
            distanceTitle.text = "";
            distanceInfo.text = "";
        }
        if (unitSkill.skillInfo.hoverRange >= 0 && unitSkill.skillInfo.skillRange > 0)
        {
            rangeTitle.text = "范围";
            rangeInfo.text = (unitSkill.skillInfo.hoverRange + 1).ToString();
            switch (unitSkill.skillInfo.rangeType)
            {
                case RangeType.common:
                    rangeTitle.text += "      普通型";

                    break;
                case RangeType.straight:
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
        rateInfo.text = unitSkill.skillInfo.skillRate + "%";

        //if (descriptionPanel.Find("SkillDescription").Find("SkillCombo").gameObject.activeInHierarchy)
        //{
        //    skillDescription.text = "  " + unitSkill.description;
        //}
        //else
        //{
        //    skillDescription.text = unitSkill.description;
        //}
        
    }

    private void OnButtonClick()
    {
        var btn = EventSystem.current.currentSelectedGameObject;
        if(btn.name == "LevelUp")
        {
            LevelUp(SkillInfoDictionary.GetParam(int.Parse(btn.transform.parent.parent.name)).ID);
        }
        else if(btn.name == "LevelDown")
        {
            LevelDown(SkillInfoDictionary.GetParam(int.Parse(btn.transform.parent.parent.name)).ID);
        }
    }


    public void LevelUp(int skillInfoID)
    {
        
        var DB = Global.characterDataList.Find(d => d.characterInfoID == character.GetComponent<CharacterStatus>().characterInfoID);

        if(DB.attributes.Find(d => d.eName == "skp").Value > 0)
        {
            var tempSkill = SkillManager.GetInstance().skillList.Find(s => s.SkillInfoID == skillInfoID);

            if (DB.skills.Find(s => s.skillInfoID == skillInfoID).level < tempSkill.skillInfo.maxLevel)
            {
                DB.skills.Find(s => s.skillInfoID == skillInfoID).level++;
                DB.attributes.Find(d => d.eName == "skp").ChangeValueTo(DB.attributes.Find(d => d.eName == "skp").Value - 1);
                UpdateView(character);
                transform.parent.GetComponent<BaseInfo>().UpdateView(character);
            }
        }
    }

    public void LevelDown(int skillInfoID)
    {
        var DB = Global.characterDataList.Find(d => d.characterInfoID == character.GetComponent<CharacterStatus>().characterInfoID);

        if (DB.skills.Find(s => s.skillInfoID == skillInfoID).level > 0)
        {
            DB.skills.Find(s => s.skillInfoID == skillInfoID).level--;
            DB.attributes.Find(d => d.eName == "skp").ChangeValueTo(DB.attributes.Find(d => d.eName == "skp").Value + 1);


            UpdateView(character);
            transform.parent.GetComponent<BaseInfo>().UpdateView(character);
        }
    }

    public void Clear(object sender, EventArgs e)
    {
        foreach(var b in allButtons)
        {
            Destroy(b);
        }
        gameObject.SetActive(false);
    }
}
