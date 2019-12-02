using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Rest : Skill
{
    private GameObject restUI;
    
    public override bool Check()
    {
        return true;
    }

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
        var go = (GameObject)Resources.Load("Prefabs/UI/Confirm");
        restUI = UnityEngine.Object.Instantiate(go, GameObject.Find("Canvas").transform);
        restUI.name = "endRoundPanel";
        restUI.transform.Find("Return").GetComponent<Button>().onClick.AddListener(Reset);
        restUI.transform.Find("Confirm").GetComponent<Button>().onClick.AddListener(Confirm);
        restUI.SetActive(false);
    }

    private void ShowUI()
    {
        restUI.SetActive(true);
    }

    public void Confirm()
    {
        //技能逻辑
        var hpAttribute = character.GetComponent<Unit>().attributes.Find(d => d.eName == "hp");
        var currentHp = hpAttribute.Value;
        var currentHPMax = hpAttribute.ValueMax;
        //“修养”接口预留(通过控制factor)

        Utils_Coroutine.GetInstance().Invoke(() => {

            var restValue = (int)(currentHPMax * skillInfo.factor * 0.01f);
            restValue = currentHp + restValue > currentHPMax ? currentHPMax - currentHp : restValue;

            var hp = currentHp + restValue;

            UIManager.GetInstance().FlyNum(character.GetComponent<Unit>().arrowPosition / 2 + character.position + Vector3.down * 0.2f, restValue.ToString(), Utils_Color.hpColor);

            hpAttribute.ChangeValueTo(hp);

            //持续到回合结束的防御力debuff
            var buff = new DataBuff(1, "def", -5);
            buff.Apply(character);
            character.GetComponent<Unit>().Buffs.Add(buff);

            //回合直接结束
            UnityEngine.Object.Destroy(restUI);
            skillState = SkillState.confirm;
            character.GetComponent<Unit>().OnUnitEnd();
            RoundManager.GetInstance().EndTurn();
        }, 0.5f);

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
        if (restUI)
        {
            UnityEngine.Object.Destroy(restUI);
        }
        base.Reset();
    }
}
