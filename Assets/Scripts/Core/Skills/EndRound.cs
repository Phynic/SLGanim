using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndRound : Skill {

    private GameObject endRoundUI;
    
    public override bool Init(Transform character)
    {
        this.character = character;
        CreateUI();
        if (Check())
        {
            //if(RoundManager.GetInstance().currentPlayer.GetType() == typeof(HumanPlayer))
            //{
            //    ShowUI();
            //}
            ShowUI();
            return true;
        }
        return false;
    }

    private void CreateUI()
    {
        var go = (GameObject)Resources.Load("Prefabs/UI/Confirm");
        endRoundUI = UnityEngine.Object.Instantiate(go, GameObject.Find("Canvas").transform);
        endRoundUI.name = "endRoundPanel";
        endRoundUI.transform.Find("Return").GetComponent<Button>().onClick.AddListener(Reset);
        endRoundUI.transform.Find("Confirm").GetComponent<Button>().onClick.AddListener(Confirm);
        endRoundUI.SetActive(false);
    }

    private void ShowUI()
    {
        endRoundUI.SetActive(true);
    }

    public void Confirm()
    {
        character.GetComponent<Unit>().action.Clear();  //禁止回退
        character.GetComponent<CharacterAction>().SetSkill("ChooseDirection");
        UnityEngine.Object.Destroy(endRoundUI);
        skillState = SkillState.confirm;
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
        if (endRoundUI)
        {
            UnityEngine.Object.Destroy(endRoundUI);
        }
        base.Reset();
    }

    public override bool Check()
    {
        return true;
    }
}
