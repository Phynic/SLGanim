using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SkillOrToolList : Skill
{
    
    
    private Dictionary<GameObject, ItemRecord> buttonRecord = new Dictionary<GameObject, ItemRecord>();
    private GameObject skillOrToolListUI;

    public override bool Init(Transform character)
    {
        this.character = character;
        
        CreateUI();
        if (Check())
        {
            //if (RoundManager.GetInstance().currentPlayer.GetType() == typeof(HumanPlayer))
            //{
                ShowUI();
            //}
            return true;
        }
        return false;
    }

    private void CreateUI()
    {
        List<GameObject> allButtons;
        skillOrToolListUI = UIManager.GetInstance().CreateButtonList(character, this, out allButtons, ref buttonRecord, skill => { return skill.skillInfo.skillType != SkillType.dodge; });
        foreach(var button in allButtons)
        {
            button.GetComponent<Button>().onClick.AddListener(OnButtonClick);
        }
        skillOrToolListUI.transform.Find("Return").GetComponent<Button>().onClick.AddListener(Reset);
        
    }

    public void OnButtonClick()
    {
        var btn = EventSystem.current.currentSelectedGameObject;
        
        if (character.GetComponent<CharacterAction>().SetSkill(btn.name))
        {
            if (skillOrToolListUI)
            {
                UnityEngine.Object.Destroy(skillOrToolListUI);
                skillState = SkillState.confirm;
            }
        }
        else
        {
            //SetItem
            character.GetComponent<CharacterAction>().SetItem(btn.name, buttonRecord[btn]);
            if (skillOrToolListUI)
            {
                UnityEngine.Object.Destroy(skillOrToolListUI);
                skillState = SkillState.confirm;
            }
        }
    }

    //public void OnButtonEnter(GameObject go)
    //{
    //    PrivateItemData itemData;
    //    if(buttonRecord.TryGetValue(go,out itemData))
    //    {
    //        var tempItem = (INinjaTool)SkillManager.GetInstance().skillList.Find(s => s.EName == itemData.itemName);
    //        //作显示数据使用。技能中使用的是深度复制实例。
    //        tempItem.SetItem(itemData);
    //        UIManager.GetInstance().LogSkillInfo((UnitSkill)tempItem, skillOrToolListUI);
    //    }
    //    else
    //    {
    //        var unitSkillData = character.GetComponent<CharacterStatus>().skills;
    //        var tempSkill = (UnitSkill)SkillManager.GetInstance().skillList.Find(s => s.EName == go.name);
    //        //作显示数据使用。技能中使用的是深度复制实例。
    //        tempSkill.SetLevel(unitSkillData[go.name]);
    //    }
    //}

    private void ShowUI()
    {
        skillOrToolListUI.SetActive(true);
    }

    public override bool OnUpdate(Transform character)
    {
        switch (skillState)
        {
            case SkillState.init:
                Init(character);
                skillState = SkillState.waitForInput;
                break;
            case SkillState.waitForInput:
                //UIManager.GetInstance().LogSkillInfo(skillOrToolListUI);
                break;
            case SkillState.confirm:
                return true;
            case SkillState.reset:
                return true;
        }
        return false;
    }


    //AI
    //public List<UnitSkill> Confirm()
    //{
    //    return unitSkillList;
    //}

    public override void Reset()
    {
        if (skillOrToolListUI)
        {
            UnityEngine.Object.Destroy(skillOrToolListUI);
        }
        base.Reset();
    }

    public override bool Check()
    {
        return true;
    }
}
