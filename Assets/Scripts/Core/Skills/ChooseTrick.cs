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
        
    }

    private void ShowUI()
    {
        chooseTrickUI.SetActive(true);
    }

    private void ShowConfirm(GameObject btn)
    {
        if (chooseTrickUI)
            UnityEngine.Object.Destroy(chooseTrickUI);
        var go = (GameObject)Resources.Load("Prefabs/UI/Confirm");
        confirmUI = UnityEngine.Object.Instantiate(go, GameObject.Find("Canvas").transform);
        confirmUI.transform.Find("Return").GetComponent<Button>().onClick.AddListener(ResetSelf);
        confirmUI.transform.Find("Confirm").GetComponent<Button>().onClick.AddListener(() => { Confirm(btn); });
    }
    
    private void OnButtonClick()
    {
        var btn = EventSystem.current.currentSelectedGameObject;
        dodgeSkillName = btn.name;
        dodgeSkill = (UnitSkill)SkillManager.GetInstance().skillList.Find(s => s.EName == dodgeSkillName);
        if(dodgeSkill is INinjaTool)
            ((INinjaTool)dodgeSkill).SetItem(buttonRecord[btn]);
        costMP = dodgeSkill.costMP;
        if (Check())
        {
            ShowConfirm(btn);
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

    private void Confirm(GameObject btn)
    {
        var buff = new DodgeBuff(1, dodgeSkill.EName);
        if (dodgeSkill is INinjaTool)
        {
            buff.itemData = buttonRecord[btn];
            ((INinjaTool)dodgeSkill).RemoveSelf(character);
        }
            
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
