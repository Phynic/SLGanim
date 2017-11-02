using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SkillOrToolList : Skill
{
    
    private Dictionary<string, int> unitSkillData = new Dictionary<string, int>();      //角色处获得来的角色技能数据
    private GameObject skillOrToolListUI;
    
    public override bool Init(Transform character)
    {
        this.character = character;
        
        unitSkillData = character.GetComponent<CharacterStatus>().skills;
        
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
        var go = (GameObject)Resources.Load("Prefabs/UI/SkillOrToolList");
        var b = (GameObject)Resources.Load("Prefabs/UI/Button");
        GameObject button;
        skillOrToolListUI = UnityEngine.Object.Instantiate(go, GameObject.Find("Canvas").transform);
        List<GameObject> temp = new List<GameObject>();
        
        foreach (var skill in unitSkillData)
        {
            var tempSkill = (UnitSkill)SkillManager.GetInstance().skillList.Find(s => s.EName == skill.Key);
            if (tempSkill != null)
            {
                if (tempSkill.skillType != UnitSkill.SkillType.dodge)
                {
                    button = GameObject.Instantiate(b, skillOrToolListUI.transform);
                    button.GetComponentInChildren<Text>().alignment = TextAnchor.MiddleLeft;
                    button.GetComponentInChildren<Text>().text = " " + tempSkill.CName + "   " + "消耗：" + tempSkill.costHP + "体力" + tempSkill.costMP + "查克拉";
                    button.name = skill.Key;
                    button.GetComponent<Button>().onClick.AddListener(OnButtonClick);
                    button.GetComponent<RectTransform>().sizeDelta = new Vector2(860, 60);
                    button.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 1f);
                    temp.Add(button);
                }
            }
        }

        skillOrToolListUI.GetComponent<RectTransform>().sizeDelta = new Vector2(skillOrToolListUI.GetComponent<RectTransform>().sizeDelta.x, temp[0].GetComponent<RectTransform>().sizeDelta.y * (1.2f * (temp.Count - 1) + 3));

        //设置按钮位置
        for (int i = 0; i < temp.Count; i++)
        {
            temp[i].transform.localPosition = new Vector3(0, - temp[i].GetComponent<RectTransform>().sizeDelta.y - (int)(i * temp[i].GetComponent<RectTransform>().sizeDelta.y * 1.2f), 0);
        }

        

        skillOrToolListUI.transform.Find("Return").GetComponent<Button>().onClick.AddListener(Reset);
        skillOrToolListUI.SetActive(false);
    }

    private void OnButtonClick()
    {
        var btn = EventSystem.current.currentSelectedGameObject;

        if (character.GetComponent<CharacterAction>().SetSkill(btn.name))
        {
            //character.GetComponent<CharacterAction>().GetSkill().SkillReset += OnSkillReset;
            //OnSkillSelected();
            if (skillOrToolListUI)
            {
                UnityEngine.Object.Destroy(skillOrToolListUI);
                skillState = SkillState.confirm;
            }
        }
        else
        {
            Debug.Log("SetSkill FALSE");
        }
    }

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
