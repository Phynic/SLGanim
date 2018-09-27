using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemMenu : MonoBehaviour {

    private GameObject _Button;
    private GameObject _SkillButtonImages;
    private List<Sprite> imagesList = new List<Sprite>();
    private List<GameObject> allButtons = new List<GameObject>();

    private void Awake()
    {
        _Button = (GameObject)Resources.Load("Prefabs/UI/Button");
        _SkillButtonImages = (GameObject)Resources.Load("Prefabs/UI/SkillButtonImages_Single");
        var images = Resources.LoadAll("Textures/SkillButtonImages/Single", typeof(Sprite));

        foreach (var i in images)
        {
            imagesList.Add((Sprite)i);
        }
    }

    public void UpdateView()
    {
        gameObject.SetActive(true);
        foreach (var b in allButtons)
        {
            Destroy(b);
        }
        CreateItemList();
    }

    public void CreateItemList()
    {
        var itemsData = Global.GetInstance().playerDB.items;
        var UIContent = transform.Find("Scroll View").Find("Viewport").Find("Content");
        var skillInfoPanel = transform.Find("SkillInfoPanel");
        var descriptionPanel = transform.Find("DescriptionPanel");
        skillInfoPanel.gameObject.SetActive(false);
        descriptionPanel.gameObject.SetActive(false);
        var roleInfoPanel = transform.parent;

        allButtons.Clear();
        GameObject button;

        foreach (var itemData in itemsData)
        {
            var t = SkillManager.GetInstance().skillList.Find(s => s.EName == itemData.itemName).GetType();
            //作显示数据使用。技能中使用的是深度复制实例。
            var tempItem = Activator.CreateInstance(t) as INinjaTool;
            tempItem.SetItem(itemData);
            var tempSkill = (UnitSkill)tempItem;
            button = GameObject.Instantiate(_Button, UIContent);

            Destroy(button.GetComponent<Button>());

            button.GetComponentInChildren<Text>().alignment = TextAnchor.MiddleLeft;
            button.GetComponentInChildren<Text>().text = tempSkill.CName;
            button.GetComponentInChildren<Text>().resizeTextForBestFit = false;
            button.GetComponentInChildren<Text>().fontSize = 45;
            button.GetComponentInChildren<Text>().GetComponent<RectTransform>().sizeDelta = new Vector2(-30, 0);
            button.name = tempSkill.CName;

            button.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 72);

            button.GetComponent<RectTransform>().pivot = new Vector2(0f, 1f);
            button.GetComponent<RectTransform>().anchorMin = new Vector2(0, 1);
            button.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);
            allButtons.Add(button);
            
            if(tempItem.Equipped.Length > 0)
            {
                button.GetComponentInChildren<Text>().color = UIManager.redTextColor;
            }

            var imageUI = UnityEngine.Object.Instantiate(_SkillButtonImages, button.transform);

            var _Class = imageUI.transform.Find("SkillClass").GetComponent<Image>();
            var _Type = imageUI.transform.Find("SkillType").GetComponent<Image>();
            var _Combo = imageUI.transform.Find("SkillCombo").GetComponent<Image>();
            
            if (tempSkill is UnitSkill)
            {
                var tempUnitSkill = (UnitSkill)tempSkill;
                _Class.sprite = imagesList.Find(i => i.name.Substring(11) == tempUnitSkill.skillClass.ToString());
                _Type.sprite = imagesList.Find(i => i.name.Substring(10) == tempUnitSkill.skillType.ToString());
                _Combo.gameObject.SetActive(tempUnitSkill.comboType != UnitSkill.ComboType.cannot);
            }
            else
            {
                _Class.sprite = imagesList.Find(i => i.name.Substring(11) == UnitSkill.SkillClass.passive.ToString());
                _Type.gameObject.SetActive(false);
                _Combo.gameObject.SetActive(false);
            }


            EventTriggerListener.Get(button).onEnter = g =>
            {
                if (tempSkill.description.Length > 0)
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

        skillInfoPanel.position = new Vector3(skillInfoPanel.position.x, y, skillInfoPanel.position.z);

        //Debug.Log("y:" + y + " syncY:" + syncY + " minY:" + minY);

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
        skillDescription.text = skill.description;

        INinjaTool ninjaTool;
        if (skill is INinjaTool)
        {
            ninjaTool = (INinjaTool)skill;
            if (ninjaTool.Equipped.Length > 0)
            {
                var cName = Global.GetInstance().characterDB.characterDataList.Find(c => c.roleEName == ninjaTool.Equipped).roleCName;
                //取空格后的名字
                costTitle.text = cName.Substring(cName.IndexOf(" ") + 1);
                costInfo.text = "装备中";
            }
            else
            {
                costTitle.text = "";
                costInfo.text = "";
            }
        }

        UnitSkill unitSkill;

        if (skill is UnitSkill)
        {
            unitSkill = (UnitSkill)skill;
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
        }
    }

    public void Clear(object sender, EventArgs e)
    {
        foreach (var b in allButtons)
        {
            Destroy(b);
        }
        gameObject.SetActive(false);
    }
}
