using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemMenu_Role : MonoBehaviour {

    private GameObject _Button;
    private GameObject _SkillButtonImages;
    private List<Sprite> imagesList = new List<Sprite>();
    private List<GameObject> allButtons = new List<GameObject>();
    private Transform character;
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

    public void UpdateView(Transform character)
    {
        this.character = character;
        gameObject.SetActive(true);
        foreach (var b in allButtons)
        {
            Destroy(b);
        }
        CreateItemList(character);
    }

    public void CreateItemList(Transform character)
    {
        var items = Global.characterDataList.Find(c => c.roleEName == character.GetComponent<CharacterStatus>().roleEName).items;
        var UIContent = transform.Find("Scroll View/Viewport/Content");
        var skillInfoPanel = transform.Find("SkillInfoPanel");
        var descriptionPanel = transform.Find("DescriptionPanel");
        skillInfoPanel.gameObject.SetActive(false);
        descriptionPanel.gameObject.SetActive(false);
        var roleInfoPanel = transform.parent;

        allButtons.Clear();
        GameObject button;
        var characterData = Global.characterDataList.Find(c => c.characterInfoID == character.GetComponent<CharacterStatus>().characterInfoID);
        int itemNum = characterData.attributes.Find(d => d.eName == "itemNum").Value;
        
        //空按钮
        for (int i = 0; i < itemNum; i++)
        {
            button = Instantiate(_Button, UIContent);
            button.GetComponentInChildren<Text>().alignment = TextAnchor.MiddleLeft;
            button.GetComponentInChildren<Text>().alignByGeometry = false;
            button.GetComponentInChildren<Text>().text = "-- -- -- -- -- -- -- --";
            button.GetComponentInChildren<Text>().resizeTextForBestFit = false;
            button.GetComponentInChildren<Text>().fontSize = 45;
            button.GetComponentInChildren<Text>().GetComponent<RectTransform>().sizeDelta = new Vector2(-30, 0);
            
            button.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 72);

            button.GetComponent<RectTransform>().pivot = new Vector2(0f, 1f);
            button.GetComponent<RectTransform>().anchorMin = new Vector2(0, 1);
            button.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);


            button.GetComponent<Button>().onClick.AddListener(OnButtonClick);

            allButtons.Add(button);
            button.name = "item_" + allButtons.IndexOf(button);
        }
        //忍具按钮
        for(int i = 0; i < items.Count; i++)
        {
            var t = SkillManager.GetInstance().skillList.Find(s => s.SkillInfoID == Global.itemRecords[items[i].uniqueID].skillInfoID).GetType();
            //作显示数据使用。技能中使用的是深度复制实例。
            var tempItem = Activator.CreateInstance(t) as INinjaTool;
            tempItem.SetItem(Global.itemRecords[items[i].uniqueID]);
            var tempSkill = (UnitSkill)tempItem;
            
            button = allButtons[i];
            button.GetComponentInChildren<Text>().alignByGeometry = true;
            button.GetComponentInChildren<Text>().text = tempSkill.CName;
            button.GetComponent<RectTransform>().sizeDelta = new Vector2(-72, 72);

            var removeItemButton = Instantiate(_Button, button.transform);
            
            removeItemButton.GetComponentInChildren<Text>().fontSize = 45;
            removeItemButton.name = "RemoveItemButton";
            removeItemButton.GetComponentInChildren<Text>().GetComponent<RectTransform>().sizeDelta = new Vector2(-30, 0);
            removeItemButton.GetComponentInChildren<Text>().text = "卸";
            removeItemButton.GetComponent<RectTransform>().pivot = new Vector2(0f, 0.5f);
            removeItemButton.GetComponent<RectTransform>().anchorMin = new Vector2(1, 0.5f);
            removeItemButton.GetComponent<RectTransform>().anchorMax = new Vector2(1, 0.5f);
            removeItemButton.GetComponent<RectTransform>().sizeDelta = new Vector2(72, 72);
            removeItemButton.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            removeItemButton.GetComponent<Button>().onClick.AddListener(RemoveItem);
            var imageUI = Instantiate(_SkillButtonImages, button.transform);
            
            var _Class = imageUI.transform.Find("SkillClass").GetComponent<Image>();
            var _Type = imageUI.transform.Find("SkillType").GetComponent<Image>();
            var _Combo = imageUI.transform.Find("SkillCombo").GetComponent<Image>();

            var halfSize = imageUI.GetComponent<RectTransform>().rect.width / 2;

            _Class.transform.localPosition = new Vector3(halfSize - 130, 0, 0);
            _Type.transform.localPosition = new Vector3(halfSize - 70, 0, 0);
            _Combo.transform.localPosition = new Vector3(halfSize - 10, 0, 0);

            if (tempSkill is UnitSkill)
            {
                var tempUnitSkill = (UnitSkill)tempSkill;
                _Class.sprite = imagesList.Find(img => img.name.Substring(11) == tempUnitSkill.skillInfo.skillClass.ToString());
                _Type.sprite = imagesList.Find(img => img.name.Substring(10) == tempUnitSkill.skillInfo.skillType.ToString());
                _Combo.gameObject.SetActive(tempUnitSkill.skillInfo.comboType != ComboType.cannot);
            }
            else
            {
                _Class.sprite = imagesList.Find(img => img.name.Substring(11) == SkillClass.passive.ToString());
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
        skillDescription.text = skill.skillInfo.description;

        INinjaTool ninjaTool;
        if (skill is INinjaTool)
        {
            ninjaTool = (INinjaTool)skill;
            var ownerID = Global.itemRecords[ninjaTool.UniqueID].ownerID;
            if (ownerID > 0)
            {
                var cName = CharacterInfoDictionary.GetParam(ownerID).roleCName;
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
        }
    }

    private void OnButtonClick()
    {
        BattlePrepareView.GetInstance().itemMenu.UpdateView();
        var buttonRecord = BattlePrepareView.GetInstance().itemMenu.buttonRecord;
        if(buttonRecord.Count == 0)
        {
            return;
        }
        gameObject.SetActive(false);
        var btn = EventSystem.current.currentSelectedGameObject;
        var slotID = allButtons.IndexOf(btn);
        foreach(var pair in buttonRecord)
        {
            pair.Key.GetComponent<Button>().onClick.RemoveAllListeners();
            pair.Key.GetComponent<Button>().onClick.AddListener(() =>
            {
                ItemRecord tempItemData = Global.itemRecords[pair.Value];
                var items = Global.characterDataList.Find(c => c.roleEName == character.GetComponent<CharacterStatus>().roleEName).items;
                //原位置有装备
                if (items.Find(i => i.slotID == slotID) != null)
                {
                    Global.itemRecords[items.Find(i => i.slotID == slotID).uniqueID].ownerID = 0;
                    items.Remove(items.Find(i => i.slotID == slotID));
                }
                //选中的忍具已经被装备
                if (tempItemData.ownerID > 0)
                {
                    var itemsOther = Global.characterDataList.Find(c => c.characterInfoID == tempItemData.ownerID).items;
                    itemsOther.Remove(itemsOther.Find(item => item.uniqueID == pair.Value));
                }
                tempItemData.ownerID = character.GetComponent<CharacterStatus>().characterInfoID;
                tempItemData.slotID = slotID;
                items.Add(tempItemData);
                BattlePrepareView.GetInstance().itemMenu.gameObject.SetActive(false);
                UpdateView(character);
            });
        }
    }

    private void RemoveItem()
    {
        var btn = EventSystem.current.currentSelectedGameObject;
        var itemPosition = allButtons.IndexOf(btn.transform.parent.gameObject);
        var items = Global.characterDataList.Find(c => c.roleEName == character.GetComponent<CharacterStatus>().roleEName).items;
        var item = items.Find(i => i.slotID == itemPosition);
        ItemRecord tempItemData = Global.itemRecords[item.uniqueID];
        tempItemData.ownerID = 0;
        items.Remove(item);
        UpdateView(character);
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
