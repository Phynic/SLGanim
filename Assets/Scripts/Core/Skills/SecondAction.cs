using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class SecondAction : Skill
{
    private GameObject confirmUI;
    private GameObject roleInfoPanel;
    public List<Skill> secondAction = new List<Skill>();                //第二次行动列表
    GameObject secondActionPanel;
    
    public override bool Init(Transform character)
    {
        this.character = character;
        Camera.main.GetComponent<RenderBlurOutline>().RenderOutLine(character);
        //第二阶段没有技能就直接显示确定面板结束回合。
        if (character.GetComponent<CharacterStatus>().secondAction.Count == 0)
        {
            ShowConfirm();
            return true;
        }

        var go = (GameObject)Resources.Load("Prefabs/UI/Button");
        secondActionPanel = GameObject.Instantiate((GameObject)Resources.Load("Prefabs/UI/SecondAction"), GameObject.Find("Canvas").transform);

        var secondActionContent = secondActionPanel.transform.Find("Content");

        secondAction = character.GetComponent<CharacterStatus>().secondAction;
        
        GameObject button;

        for (int i = 0; i < secondAction.Count; i++)
        {
            button = GameObject.Instantiate(go, secondActionContent.transform);
            button.GetComponentInChildren<Text>().text = secondAction[i].CName;
            button.name = secondAction[i].EName;

            button.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 60);
            button.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 1f);
            button.GetComponent<RectTransform>().anchorMin = new Vector2(0, 1);
            button.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);

            if (secondAction[i].EName == "SkillOrToolList")
            {
                if (character.GetComponent<CharacterStatus>().characterIdentity != CharacterStatus.CharacterIdentity.noumenon)
                {
                    button.GetComponentInChildren<Text>().text = "术";
                }
            }

            button.transform.localPosition = new Vector3(0, -(int)(i * button.GetComponent<RectTransform>().sizeDelta.y), 0);
            button.GetComponent<Button>().onClick.AddListener(OnButtonClick);
        }

        roleInfoPanel = UIManager.GetInstance().CreateRoleInfoPanel(character);
        return true;
    }

    protected virtual void ShowConfirm()
    {
        var go = (GameObject)Resources.Load("Prefabs/UI/Confirm");
        confirmUI = UnityEngine.Object.Instantiate(go, GameObject.Find("Canvas").transform);
        confirmUI.transform.Find("Return").GetComponent<Button>().onClick.AddListener(Reset);
        confirmUI.transform.Find("Confirm").GetComponent<Button>().onClick.AddListener(Confirm);
    }
    
    //分身或变身，在无第二行动时的出口。
    private void Confirm()
    {
        if(confirmUI)
            GameObject.Destroy(confirmUI);
        if (roleInfoPanel)
            GameObject.Destroy(roleInfoPanel);
        Camera.main.GetComponent<RenderBlurOutline>().CancelRender();
        character.GetComponent<CharacterAction>().SetSkill("ChooseDirection");
        skillState = SkillState.confirm;
    }

    //出口
    public void OnButtonClick()
    {
        var btn = EventSystem.current.currentSelectedGameObject;

        if (character.GetComponent<CharacterAction>().SetSkill(btn.name))
        {
            if (secondActionPanel)
                GameObject.Destroy(secondActionPanel);
            if (roleInfoPanel)
                GameObject.Destroy(roleInfoPanel);
            Camera.main.GetComponent<RenderBlurOutline>().CancelRender();
            skillState = SkillState.confirm;
        }
        else
        {
            Debug.Log("SetSkill FALSE");
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

    public override void Reset()
    {
        if (secondActionPanel)
            GameObject.Destroy(secondActionPanel);
        if (confirmUI)
            GameObject.Destroy(confirmUI);
        if (roleInfoPanel)
            GameObject.Destroy(roleInfoPanel);
        character.GetComponent<Unit>().action.Pop();
        character.GetComponent<Unit>().action.Peek().Reset();
        skillState = SkillState.reset;
    }

    public override bool Check()
    {
        throw new NotImplementedException();
    }
}
