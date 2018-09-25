using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ChooseTrick : Skill {
    
    //private Dictionary<string, int> unitSkillData = new Dictionary<string, int>();      //角色处获得来的角色技能数据
    private Dictionary<GameObject, ItemData> buttonRecord = new Dictionary<GameObject, ItemData>();
    private GameObject chooseTrickUI;
    private GameObject confirmUI;
    private int costMP;
    private UnitSkill dodgeSkill;

    string dodgeSkillName;
    
    public override bool Init(Transform character)
    {
        this.character = character;
        
        //unitSkillData = character.GetComponent<CharacterStatus>().skills;
        
        CreateUI();
        
        //if (RoundManager.GetInstance().currentPlayer.GetType() == typeof(HumanPlayer))
        //{
            ShowUI();
        //}
        return true;
    }

    private void CreateUI()
    {
        List<GameObject> allButtons;
        chooseTrickUI = UIManager.GetInstance().CreateButtonList(character, this, out allButtons, ref buttonRecord, skill => { return skill.skillType == UnitSkill.SkillType.dodge; });
        foreach (var button in allButtons)
        {
            button.GetComponent<Button>().onClick.AddListener(OnButtonClick);
        }
        chooseTrickUI.transform.Find("Return").GetComponent<Button>().onClick.AddListener(Reset);
        
        //var go = (GameObject)Resources.Load("Prefabs/UI/SkillOrToolList");
        //var b = (GameObject)Resources.Load("Prefabs/UI/Button");
        //GameObject button;
        //chooseTrickUI = UnityEngine.Object.Instantiate(go, GameObject.Find("Canvas").transform);
        //var UIContent = chooseTrickUI.transform.Find("Scroll View").Find("Viewport").Find("Content");


        //List<GameObject> temp = new List<GameObject>();

        //foreach (var skill in unitSkillData)
        //{
        //    var tempSkill = (UnitSkill)SkillManager.GetInstance().skillList.Find(s => s.EName == skill.Key);
        //    if (tempSkill != null)
        //    {
        //        if (tempSkill.skillType == UnitSkill.SkillType.dodge)
        //        {
        //            button = GameObject.Instantiate(b, UIContent);
        //            button.GetComponentInChildren<Text>().text = " " + tempSkill.CName + "   " + "消耗：" + tempSkill.costHP + "体力" + tempSkill.costMP + "查克拉";
        //            button.name = skill.Key;
        //            button.GetComponent<Button>().onClick.AddListener(OnButtonClick);
        //            button.GetComponent<RectTransform>().sizeDelta = new Vector2(860, 60);
        //            button.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 1f);
        //            temp.Add(button);
        //            if (!tempSkill.Filter(this))
        //            {
        //                button.GetComponent<Button>().interactable = false;
        //            }
        //        }
        //    }
        //}

        //var unitItemData = character.GetComponent<CharacterStatus>().items;
        ////忍具
        //foreach (var item in unitItemData)
        //{
        //    var tempItem = (INinjaTool)SkillManager.GetInstance().skillList.Find(s => s.EName == item.itemName);
        //    //作显示数据使用。技能中使用的是深度复制实例。
        //    tempItem.SetItem(item);
        //    var tempSkill = (UnitSkill)tempItem;
        //    //作显示数据使用。技能中使用的是深度复制实例。
        //    tempSkill.SetLevel(item.itemLevel);
        //    if (tempSkill != null)
        //    {
        //        if (tempSkill.skillType == UnitSkill.SkillType.dodge)
        //        {
        //            button = GameObject.Instantiate(b, UIContent);
        //            button.GetComponentInChildren<Text>().alignment = TextAnchor.MiddleLeft;
        //            button.GetComponentInChildren<Text>().text = " " + tempSkill.CName;
        //            button.name = item.itemName;
        //            button.GetComponent<Button>().onClick.AddListener(OnButtonClick);
        //            button.GetComponent<RectTransform>().sizeDelta = new Vector2(860, 60);
        //            button.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 1f);
        //            temp.Add(button);
        //            buttonRecord.Add(button, item);
        //        }
        //    }
        //}


        ////设置Panel长宽
        //chooseTrickUI.transform.Find("Scroll View").Find("Scrollbar Vertical").gameObject.SetActive(false);
        //UIContent.GetComponent<RectTransform>().sizeDelta = new Vector2(UIContent.GetComponent<RectTransform>().sizeDelta.x, temp[0].GetComponent<RectTransform>().sizeDelta.y * (1.2f * (temp.Count - 1) + 3));
        
        ////设置按钮位置
        //for (int i = 0; i < temp.Count; i++)
        //{
        //    temp[i].transform.localPosition = new Vector3(500, - (int)(i * temp[i].GetComponent<RectTransform>().sizeDelta.y * 1.2f), 0);
        //}

        //chooseTrickUI.transform.Find("Return").GetComponent<Button>().onClick.AddListener(Reset);
        //chooseTrickUI.SetActive(false);
    }

    private void ShowUI()
    {
        chooseTrickUI.SetActive(true);
    }

    private void ShowConfirm()
    {
        if (chooseTrickUI)
            UnityEngine.Object.Destroy(chooseTrickUI);
        var go = (GameObject)Resources.Load("Prefabs/UI/Confirm");
        confirmUI = UnityEngine.Object.Instantiate(go, GameObject.Find("Canvas").transform);
        confirmUI.transform.Find("Return").GetComponent<Button>().onClick.AddListener(ResetSelf);
        confirmUI.transform.Find("Confirm").GetComponent<Button>().onClick.AddListener(Confirm);
    }
    
    private void OnButtonClick()
    {
        var btn = EventSystem.current.currentSelectedGameObject;
        dodgeSkillName = btn.name;
        dodgeSkill = (UnitSkill)SkillManager.GetInstance().skillList.Find(s => s.EName == dodgeSkillName);
        costMP = dodgeSkill.costMP;
        if (Check())
        {
            ShowConfirm();
        }
    }

    public override bool OnUpdate(Transform character)
    {
        switch (skillState)
        {
            case SkillState.init:
                Init(character);
                skillState = SkillState.waitForInput;
                break;
            case SkillState.confirm:
                return true;
            case SkillState.reset:
                return true;
        }
        return false;
    }

    private void Confirm()
    {
        var buff = new DodgeBuff(1, dodgeSkill.EName);
        character.GetComponent<CharacterStatus>().Buffs.Add(buff);
        if (chooseTrickUI)
            UnityEngine.Object.Destroy(chooseTrickUI);
        if (confirmUI)
            UnityEngine.Object.Destroy(confirmUI);
        skillState = SkillState.confirm;
        character.GetComponent<Unit>().OnUnitEnd();   //真正的回合结束所应执行的逻辑。
        RoundManager.GetInstance().EndTurn();
        
    }
    
    public override void Reset()
    {
        if (chooseTrickUI)
            UnityEngine.Object.Destroy(chooseTrickUI);
        if (confirmUI)
            UnityEngine.Object.Destroy(confirmUI);

        base.Reset();
    }

    public void ResetSelf()
    {
        if (chooseTrickUI)
            UnityEngine.Object.Destroy(chooseTrickUI);
        if (confirmUI)
            UnityEngine.Object.Destroy(confirmUI);
        character.GetComponent<CharacterAction>().SetSkill(character.GetComponent<Unit>().action.Pop().EName);
        skillState = SkillState.reset;
    }

    public override bool Check()
    {
        if (character.GetComponent<CharacterStatus>().attributes.Find(d => d.eName == "mp").value >= costMP)
        {
            return true;
        }
        else
        {
            DebugLogPanel.GetInstance().Log("查克拉不足！");
            return false;
        }
    }
}
