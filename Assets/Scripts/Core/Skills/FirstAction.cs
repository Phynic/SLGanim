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
    
    public override bool Init(Transform character)
    {
        this.character = character;
        var go = (GameObject)Resources.Load("Prefabs/UI/Button");
        firstActionPanel = GameObject.Instantiate((GameObject)Resources.Load("Prefabs/UI/FirstAction"), GameObject.Find("Canvas").transform);

        firstAction = character.GetComponent<CharacterStatus>().firstAction;
        
        GameObject button;

        for (int i = 0; i < firstAction.Count; i++)
        {
            button = GameObject.Instantiate(go, firstActionPanel.transform);
            button.GetComponentInChildren<Text>().text = firstAction[i].CName;
            button.name = firstAction[i].EName;
            button.transform.position = new Vector3((int)(Screen.width * 0.125), (int)(Screen.height * 0.65) - (int)(i * Screen.height * 0.08), 0);
            button.GetComponent<Button>().onClick.AddListener(OnButtonClick);
        }

        return true;
    }

    public void OnButtonClick()
    {
        var btn = EventSystem.current.currentSelectedGameObject;

        if (character.GetComponent<CharacterAction>().SetSkill(btn.name))
        {
            if (firstActionPanel)
                GameObject.Destroy(firstActionPanel);
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
        character.GetComponent<Unit>().action.Pop();
        skillState = SkillState.reset;
        RoundManager.GetInstance().RoundState = new RoundStateWaitingForInput(RoundManager.GetInstance());
    }

    public override bool Check()
    {
        throw new NotImplementedException();
    }
}
