using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RestoreChakra : Skill
{
    private GameObject restoreChakraUI;
    private Slider slider;
    
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

    public override bool Check()
    {
        if(character.GetComponent<CharacterStatus>().attributes.Find(d => d.eName == "mp").valueMax == character.GetComponent<CharacterStatus>().attributes.Find(d => d.eName == "mp").value)
        {
            DebugLogPanel.GetInstance().Log("查克拉已经是最大值。");
            Reset();
            return false;
        }
        else
        {
            return true;
        }
    }

    private void CreateUI()
    {
        var go = (GameObject)Resources.Load("Prefabs/UI/RestoreChakra");
        restoreChakraUI = UnityEngine.Object.Instantiate(go, GameObject.Find("Canvas").transform);
        restoreChakraUI.name = "RestoreChakraPanel";
        slider = restoreChakraUI.transform.Find("Slider").GetComponent<Slider>();
        slider.maxValue = character.GetComponent<CharacterStatus>().attributes.Find(d => d.eName == "mp").valueMax;
        slider.value = character.GetComponent<CharacterStatus>().attributes.Find(d => d.eName == "mp").value;
        restoreChakraUI.transform.Find("Return").GetComponent<Button>().onClick.AddListener(Reset);
        restoreChakraUI.transform.Find("Confirm").GetComponent<Button>().onClick.AddListener(Confirm);
        restoreChakraUI.SetActive(false);
    }

    private void ShowUI()
    {
        restoreChakraUI.SetActive(true);
    }
    
    private void Confirm()
    {
        var animator = character.GetComponent<Animator>();
        animator.SetInteger("Skill", 2);
        UnityEngine.Object.Destroy(restoreChakraUI);

        var time = animator.GetCurrentAnimatorClipInfo(0)[0].clip.length;
        
        RoundManager.GetInstance().Invoke(() => {
            animator.speed = 0;
            FXManager.GetInstance().Spawn("Chakra", animator.GetBoneTransform(HumanBodyBones.Chest).position, 3f);
        }, 0.5f);
        RoundManager.GetInstance().Invoke(() => {
            animator.speed = 1;
            animator.SetInteger("Skill", 0);
            var currentHp = character.GetComponent<CharacterStatus>().attributes.Find(d => d.eName == "hp").value;
            var mud = character.GetComponent<CharacterStatus>().attributes.Find(d => d.eName == "mud").value;
            var restoreValue = slider.value - character.GetComponent<CharacterStatus>().attributes.Find(d => d.eName == "mp").value;
            var hp = currentHp - mud * restoreValue;
            ChangeData.ChangeValue(character, "hp", (int)hp);
            ChangeData.ChangeValue(character, "mp", (int)slider.value);

            UIManager.GetInstance().FlyNum(character.GetComponent<CharacterStatus>().arrowPosition / 2 + character.position + Vector3.down * 0.2f, restoreValue.ToString(), UIManager.mpColor);

            var buff = new DataBuff(1, "def", -5);
            buff.Apply(character);
            character.GetComponent<CharacterStatus>().Buffs.Add(buff);
            
            character.GetComponent<Unit>().OnUnitEnd();   //真正的回合结束所应执行的逻辑。
            RoundManager.GetInstance().EndTurn();

            skillState = SkillState.confirm;

        }, 0.5f + 3.4f);
    }

    //AI
    //public void Confirm(int value)  //此value为改变后的结果
    //{
    //    var currentHp = character.GetComponent<CharacterStatus>().attributes.Find(d => d.eName == "hp").value;   //获取当前HP
    //    var mud = character.GetComponent<CharacterStatus>().attributes.Find(d => d.eName == "mud").value;        //获取mud属性
    //    var restoreValue = (float)value - character.GetComponent<CharacterStatus>().attributes.Find(d => d.eName == "mp").value;     //计算恢复的值
    //    var hp = currentHp - mud * restoreValue;    //计算损失的HP

    //    ChangeData.ChangeValue(character, "hp", (int)hp);   //执行改变
    //    ChangeData.ChangeValue(character, "mp", value);

    //    var buff = new DataBuff(1, "def", -5);
    //    buff.Apply(character);
    //    character.GetComponent<CharacterStatus>().Buffs.Add(buff);
    //    skillState = SkillState.confirm;

    //    character.GetComponent<Unit>().OnUnitEnd();   //真正的回合结束所应执行的逻辑。
    //    RoundManager.GetInstance().EndTurn();
    //}

    public override bool OnUpdate(Transform character)
    {
        switch (skillState)
        {
            case SkillState.init:
                if (Init(character))
                {
                    skillState = SkillState.waitForInput;
                }
                break;
            case SkillState.confirm:
                return true;
            case SkillState.reset:
                return true;
        }
        //锁死数值->不能向下调整。
        if (slider.value < character.GetComponent<CharacterStatus>().attributes.Find(d => d.eName == "mp").value + 1)
        {
            slider.value = character.GetComponent<CharacterStatus>().attributes.Find(d => d.eName == "mp").value + 1;
        }
        return false;
    }

    public override void Reset()
    {
        if (restoreChakraUI)
        {
            UnityEngine.Object.Destroy(restoreChakraUI);
        }
        base.Reset();
    }
}
