using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class FirstAction : Skill
{

    public List<Skill> firstAction = new List<Skill>();                 //第一次行动列表
    GameObject firstActionPanel;
    GameObject roleInfoPanel;
    public override bool Init(Transform character)
    {
        this.character = character;
        Camera.main.GetComponent<RenderBlurOutline>().RenderOutLine(character);
        
        var go = (GameObject)Resources.Load("Prefabs/UI/Button");
        firstActionPanel = GameObject.Instantiate((GameObject)Resources.Load("Prefabs/UI/FirstAction"), GameObject.Find("Canvas").transform);
        var firstActionContent = firstActionPanel.transform.Find("Content");
        firstAction = character.GetComponent<CharacterStatus>().firstAction;

        var firstActionRect = firstActionPanel.GetComponent<RectTransform>();
        var contentRect = firstActionPanel.transform.Find("Content").GetComponent<RectTransform>();


        firstActionRect.sizeDelta = new Vector2(firstActionRect.sizeDelta.x, 60 * firstAction.Count + 35.786f);
        contentRect.sizeDelta = new Vector2(contentRect.sizeDelta.x, 60 * firstAction.Count);
        
        GameObject button;

        for (int i = 0; i < firstAction.Count; i++)
        {
            button = GameObject.Instantiate(go, firstActionContent.transform);
            button.GetComponentInChildren<Text>().text = firstAction[i].CName;
            button.name = firstAction[i].EName;

            button.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 60);
            button.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 1f);
            button.GetComponent<RectTransform>().anchorMin = new Vector2(0, 1);
            button.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);

            if (firstAction[i].EName == "SkillOrToolList")
            {
                if (character.GetComponent<CharacterStatus>().characterIdentity != CharacterStatus.CharacterIdentity.noumenon)
                {
                    button.GetComponentInChildren<Text>().text = "术";
                }
            }
            button.transform.localPosition = new Vector3(0, - (int)(i * button.GetComponent<RectTransform>().sizeDelta.y), 0);
            
            button.GetComponent<Button>().onClick.AddListener(OnButtonClick);
        }

        roleInfoPanel = UIManager.GetInstance().CreateRoleInfoPanel(character);

        return true;
    }

    public void OnButtonClick()
    {
        var btn = EventSystem.current.currentSelectedGameObject;

        if (character.GetComponent<CharacterAction>().SetSkill(btn.name))
        {
            Camera.main.GetComponent<RenderBlurOutline>().CancelRender();
            if (firstActionPanel)
                GameObject.Destroy(firstActionPanel);
            if (roleInfoPanel)
                GameObject.Destroy(roleInfoPanel);
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
        if (firstActionPanel)
            GameObject.Destroy(firstActionPanel);
        if (roleInfoPanel)
            GameObject.Destroy(roleInfoPanel);
        Camera.main.GetComponent<RenderBlurOutline>().CancelRender();
        if(character && character.GetComponent<Unit>().action.Count > 0)
            character.GetComponent<Unit>().action.Pop();
        skillState = SkillState.reset;
        RoundManager.GetInstance().RoundState = new RoundStateWaitingForInput(RoundManager.GetInstance());
    }

    public override bool Check()
    {
        throw new NotImplementedException();
    }
}
