using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public abstract class UnitSkill : Skill {
    //public int level;
    public int costMP;
    public int costHP;
    public string description;
    protected int skillRange;
    public int hoverRange;
    protected int skillRate;
    protected SkillRange range;
    public Vector3 focus;
    private bool final;
    private GameObject confirmUI;
    protected bool rotateToPathDirection = true;
    protected Animator animator;
    protected GameObject comboJudgeUI;
    protected GameObject comboSelectUI;
    protected int animID;
    //originSkill是指组合技的第一个技能。
    public UnitSkill originSkill = null;
    //comboSkill是指组合技的第二个技能。
    public UnitSkill comboSkill  = null;
    protected GameObject render;

    public ComboType comboType;
    [Serializable]
    public enum ComboType
    {
        cannot,
        can,
        must
    }

    public SkillType skillType;
    [Serializable]
    public enum SkillType
    {
        attack,
        effect,
        dodge,
        other
    }

    public RangeType rangeType = RangeType.common;
    [Serializable]
    public enum RangeType
    {
        common,
        straight
    }

    public UnitSkill()
    {
        var unitSkillData = (UnitSkillData)skillData;
        costMP = unitSkillData.costMP;
        costHP = unitSkillData.costHP;
        description = unitSkillData.description;
        skillRange = unitSkillData.skillRange;
        hoverRange = unitSkillData.hoverRange;
        skillRate = unitSkillData.skillRate;
        comboType = unitSkillData.comboType;
        skillType = unitSkillData.skillType;
        rangeType = unitSkillData.rangeType;
        animID = unitSkillData.animID;
    }

    public abstract void SetLevel(int level);

    public override bool Init(Transform character)
    {
        this.character = character;
        render = character.Find("Render").gameObject;
        if(skillType != SkillType.dodge)
        {
            if (!CheckCost())
            {
                Reset();
                return false;
            }
        }
        SetLevel(character.GetComponent<CharacterStatus>().skills[_eName]);
        animator = character.GetComponent<Animator>();
        if (originSkill == null)
        {
            GameObject go;
            switch (comboType)
            {
                case ComboType.cannot:
                    RangeInit();
                    break;
                case ComboType.can:
                    //继续使用连续攻击进行攻击吗？
                    go = (GameObject)Resources.Load("Prefabs/UI/Judge");
                    comboJudgeUI = UnityEngine.Object.Instantiate(go, GameObject.Find("Canvas").transform);
                    comboJudgeUI.name = "comboConfirmPanel";
                    comboJudgeUI.transform.Find("Text").GetComponent<Text>().text = "继续使用连续攻击进行攻击吗？";
                    comboJudgeUI.transform.Find("No").GetComponent<Button>().onClick.AddListener(RangeInit);
                    comboJudgeUI.transform.Find("Yes").GetComponent<Button>().onClick.AddListener(CreateComboSelectUI);
                    break;
                case ComboType.must:
                    //请选择要使用连续攻击的攻击类术·忍具。
                    go = (GameObject)Resources.Load("Prefabs/UI/Judge");
                    comboJudgeUI = UnityEngine.Object.Instantiate(go, GameObject.Find("Canvas").transform);
                    comboJudgeUI.name = "comboConfirmPanel";
                    comboJudgeUI.transform.Find("Text").GetComponent<Text>().text = "请选择要使用连续攻击的攻击类术·忍具。";
                    GameObject.Destroy(comboJudgeUI.transform.Find("No").gameObject);
                    var button = comboJudgeUI.transform.Find("Yes");
                    button.GetComponent<Button>().onClick.AddListener(CreateComboSelectUI);
                    button.localPosition = new Vector3(0, button.localPosition.y, button.localPosition.z);
                    break;
            }
        }
        else
        {
            originSkill.comboSkill = this;
            RangeInit();
        }
        return true;
    }

    protected void CreateComboSelectUI()
    {
        if (comboJudgeUI)
            GameObject.Destroy(comboJudgeUI);
        var go = (GameObject)Resources.Load("Prefabs/UI/SkillOrToolList");
        var b = (GameObject)Resources.Load("Prefabs/UI/Button");
        GameObject button;
        comboSelectUI = UnityEngine.Object.Instantiate(go, GameObject.Find("Canvas").transform);
        List<GameObject> temp = new List<GameObject>();
        var unitSkillsData = character.GetComponent<CharacterStatus>().skills;
        foreach (var skill in unitSkillsData)
        {
            var tempSkill = (UnitSkill)SkillManager.GetInstance().skillList.Find(s => s.EName == skill.Key);
            if (tempSkill != null)
            {
                if (tempSkill.skillType == SkillType.attack)
                {
                    button = GameObject.Instantiate(b, comboSelectUI.transform);
                    button.GetComponentInChildren<Text>().alignment = TextAnchor.MiddleLeft;
                    button.GetComponentInChildren<Text>().text = " " + tempSkill.CName;
                    button.name = skill.Key;
                    button.GetComponent<Button>().onClick.AddListener(OnButtonClick);
                    button.GetComponent<RectTransform>().sizeDelta = new Vector2(860, 60);
                    button.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 1f);
                    temp.Add(button);
                }
            }
        }

        comboSelectUI.GetComponent<RectTransform>().sizeDelta = new Vector2(comboSelectUI.GetComponent<RectTransform>().sizeDelta.x, temp[0].GetComponent<RectTransform>().sizeDelta.y * (1.2f * (temp.Count - 1) + 3));

        //设置按钮位置
        for (int i = 0; i < temp.Count; i++)
        {
            temp[i].transform.localPosition = new Vector3(0, -temp[i].GetComponent<RectTransform>().sizeDelta.y - (int)(i * temp[i].GetComponent<RectTransform>().sizeDelta.y * 1.2f), 0);
        }
        
        comboSelectUI.transform.Find("Return").GetComponent<Button>().onClick.AddListener(Reset);
    }

    protected void OnButtonClick()
    {
        var btn = EventSystem.current.currentSelectedGameObject;

        if (character.GetComponent<CharacterAction>().SetSkill(btn.name))
        {
            var skill = character.GetComponent<Unit>().action.Peek() as UnitSkill;
            skill.originSkill = this;
            if (comboSelectUI)
                UnityEngine.Object.Destroy(comboSelectUI);
            skillState = SkillState.reset;
        }
        else
        {
            Debug.Log("SetSkill FALSE");
        }
    }
    
    protected void RangeInit()
    {
        if (comboJudgeUI)
            GameObject.Destroy(comboJudgeUI);
        if(skillRange > 0)
        {
            range = new SkillRange();
            switch (rangeType)
            {
                case RangeType.common:
                    range.CreateSkillRange(skillRange, character);
                    break;
                case RangeType.straight:
                    range.CreateStraightSkillRange(skillRange, character);
                    break;
            }

            focus = new Vector3(-1, -1, -1);
            final = false;
            foreach (var f in BattleFieldManager.GetInstance().floors)
            {
                if (f.Value.activeInHierarchy)
                {
                    f.Value.GetComponent<Floor>().FloorClicked += Confirm;
                    f.Value.GetComponent<Floor>().FloorExited += DeleteHoverRange;
                    f.Value.GetComponent<Floor>().FloorHovered += Focus;
                }
            }
            //角色加入忽略层，方便选取
            UnitManager.GetInstance().units.ForEach(u => u.gameObject.layer = 2);

            range.RecoverColor();
        }
        else
        {
            Focus(this, null);
            Confirm();
        }
    }

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
            case SkillState.waitForInput:
                if (final)
                {
                    //角色取出忽略层
                    UnitManager.GetInstance().units.ForEach(u => u.gameObject.layer = 0);
                    
                    if (Check())
                    {
                        foreach (var f in BattleFieldManager.GetInstance().floors)
                        {
                            if (f.Value.activeInHierarchy)
                            {
                                f.Value.GetComponent<Floor>().FloorClicked -= Confirm;
                                f.Value.GetComponent<Floor>().FloorExited -= DeleteHoverRange;
                                f.Value.GetComponent<Floor>().FloorHovered -= Focus;
                            }
                        }
                        ShowConfirm();
                    }
                    UnitManager.GetInstance().units.ForEach(u => u.gameObject.layer = 2);
                    final = false;
                }
                break;
            case SkillState.confirm:
                
                if (originSkill != null)
                {
                    originSkill.InitSkill();
                }
                else
                {
                    InitSkill();
                }

                skillState = SkillState.applyEffect;
                
                break;
            case SkillState.applyEffect:
                if (ApplyEffects())
                {
                    animator.SetInteger("Skill", 0);
                    return true;
                }
                    
                break;
            case SkillState.reset:
                return true;
        }
        return false;
    }

    private void DeleteHoverRange(object sender, EventArgs e)
    {
        range.DeleteHoverRange();
        range.RecoverColor();
    }

    private void Focus(object sender, EventArgs e)
    {
        var go = sender as GameObject;
        focus = go.transform.position;
        if (originSkill != null)
            originSkill.focus = focus;
        range.ExcuteChangeColorAndRotate(hoverRange, skillRange, focus, rotateToPathDirection);
    }

    //AI
    public void Focus(Floor floor)
    {
        focus = floor.transform.position;
        range.ExcuteChangeColorAndRotate(hoverRange, skillRange, focus, rotateToPathDirection);
    }

    protected virtual void ShowConfirm()
    {
        var go = (GameObject)Resources.Load("Prefabs/UI/Confirm");
        confirmUI = UnityEngine.Object.Instantiate(go, GameObject.Find("Canvas").transform);
        confirmUI.transform.Find("Return").GetComponent<Button>().onClick.AddListener(ResetSelf);
        confirmUI.transform.Find("Confirm").GetComponent<Button>().onClick.AddListener(Confirm);
    }

    protected virtual void Confirm(object sender, EventArgs e)
    {
        final = true;
    }

    //confirmUI的回调函数
    protected virtual void Confirm()
    {
        if(confirmUI)
            UnityEngine.Object.Destroy(confirmUI);
        //角色取出忽略层
        UnitManager.GetInstance().units.ForEach(u => u.gameObject.layer = 0);
        skillState = SkillState.confirm;
    }

    protected virtual void InitSkill()
    {
        if(originSkill == null && comboSkill != null)
        {
            comboSkill.range.DeleteHoverRange();
            comboSkill.range.Delete();
            comboSkill.character.GetComponent<Unit>().action.Clear();  //禁止回退
            comboSkill.animator.SetInteger("Skill", animID);
        }
        if(originSkill == null && comboSkill == null)
        {
            range.DeleteHoverRange();
            range.Delete();
            character.GetComponent<Unit>().action.Clear();  //禁止回退
            animator.SetInteger("Skill", animID);
        }
        if(originSkill != null && comboSkill == null)
        {
            animator.SetInteger("Skill", animID);
        }
    }

    //ApplyEffects是一个时间段，这个时间段用来进行技能展示。
    protected abstract bool ApplyEffects();
    //Effect是一个时间点，由动画事件调用。
    public virtual void Effect()
    {
        Cost();
        animator.SetInteger("Skill", 0);
    }

    protected virtual void ResetSelf()
    {
        if (comboJudgeUI)
            GameObject.Destroy(comboJudgeUI);
        if (comboSelectUI)
            GameObject.Destroy(comboSelectUI);
        if (confirmUI)
            UnityEngine.Object.Destroy(confirmUI);
        UnitManager.GetInstance().units.ForEach(u => u.gameObject.layer = 0);
        range.Reset();
        
        foreach (var f in BattleFieldManager.GetInstance().floors)
        {
            f.Value.GetComponent<Floor>().FloorClicked -= Confirm;
            f.Value.GetComponent<Floor>().FloorExited -= DeleteHoverRange;
            f.Value.GetComponent<Floor>().FloorHovered -= Focus;
        }

        //character.GetComponent<CharacterAction>().SetSkill(character.GetComponent<Unit>().action.Pop().EName);
        skillState = SkillState.init;
    }

    //与ResetSelf的区别：Reset在Skill层对技能进行出列入列，而ResetSelf仅用于类似替身术时候的自身重置。
    public override void Reset()
    {
        //按照顺序，逆序消除影响。因为每次会Init()，所以不必都Reset。
        if (originSkill != null)
            originSkill.ResetSelf();
        if (range != null)
            range.Reset();
        
        foreach (var f in BattleFieldManager.GetInstance().floors)
        {
            f.Value.GetComponent<Floor>().FloorClicked -= Confirm;
            f.Value.GetComponent<Floor>().FloorExited -= DeleteHoverRange;
            f.Value.GetComponent<Floor>().FloorHovered -= Focus;
        }
        
        //角色取出忽略层
        UnitManager.GetInstance().units.ForEach(u => u.gameObject.layer = 0);
        if(comboJudgeUI)
            GameObject.Destroy(comboJudgeUI);
        if (comboSelectUI)
            GameObject.Destroy(comboSelectUI);
        if (confirmUI)
            UnityEngine.Object.Destroy(confirmUI);
        if(originSkill != null)
            character.GetComponent<Unit>().action.Pop();
        base.Reset();
    }

    public override bool Check()
    {
        return true;
    }
    
    protected virtual bool CheckCost()
    {
        var currentHP = character.GetComponent<CharacterStatus>().attributes.Find(d => d.eName == "hp").value;
        var currentMP = character.GetComponent<CharacterStatus>().attributes.Find(d => d.eName == "mp").value;
        if(originSkill == null)
        {
            if (costMP <= currentMP)
            {
                if (costHP <= currentHP)
                {
                    return true;
                }
                else
                {
                    DebugLogPanel.GetInstance().Log("体力不足！");
                }
            }
            else
            {
                DebugLogPanel.GetInstance().Log("查克拉不足！");
                Debug.Log("查克拉不足！");
            }
        }
        else
        {
            if (originSkill.costMP + costMP <= currentMP)
            {
                if (originSkill.costHP + costHP <= currentHP)
                {
                    return true;
                }
                else
                {
                    DebugLogPanel.GetInstance().Log("体力不足！");
                }
            }
            else
            {
                DebugLogPanel.GetInstance().Log("查克拉不足！");
                Debug.Log("查克拉不足！");
            }
        }
        
        return false;
    }

    protected virtual void Cost()
    {
        var currentHP = character.GetComponent<CharacterStatus>().attributes.Find(d => d.eName == "hp").value;
        var currentMP = character.GetComponent<CharacterStatus>().attributes.Find(d => d.eName == "mp").value;

        var hp = currentHP - costHP;
        var mp = currentMP - costMP;

        ChangeData.ChangeValue(character, "hp", hp);
        ChangeData.ChangeValue(character, "mp", mp);
    }
}
