using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;

//技能是HumanPlayer的入口，而AIPlayer直接调用各个分组件进行技能实现。
public abstract class Skill
{
    public Transform character;
    public SkillInfo skillInfo;
    public string CName { get { return skillInfo.cName; } }
    public string EName { get { return skillInfo.eName; } }
    public int SkillInfoID { get { return skillInfo.ID; } }
    public bool isAI
    {
        get
        {
            var player = RoundManager.GetInstance().Players.Find(p => p.playerNumber == SkillManager.GetInstance().skillQueue.Peek().Value.GetComponent<Unit>().playerNumber);

            if (player is AIPlayer && ((AIPlayer)player).AIControl)
                return true;
            else
                return false;
        }
        private set { }
    }

    //结束输入，但技能效果并未完结。
    public bool done = false;

    public enum SkillState
    {
        init,
        waitForInput,   //每帧执行。
        confirm,        //单次执行后跳转。
        applyEffect,    //每帧执行。
        reset
    }
    public SkillState skillState = SkillState.init;

    public Skill()
    {
        var skillID = SkillInfoDictionary.GetParamList().Find(i => i.eName == GetType().ToString()).ID;
        skillInfo = SkillInfoDictionary.GetNewParam(skillID);
    }

    public abstract bool Init(Transform character);     //初始化技能
    public abstract bool OnUpdate(Transform character); //每帧执行函数

    public virtual void SetLevel(int level)
    {
        skillInfo.factor = skillInfo.factor + (level - 1) * skillInfo.growFactor;
    }

    public virtual void Reset()
    {
        //回退至上一个技能。
        if (SkillManager.GetInstance().skillQueue.Count == 1)
        {
            character.GetComponent<Unit>().action.Pop();
            character.GetComponent<CharacterAction>().SetSkill(character.GetComponent<Unit>().action.Pop().EName);
            skillState = SkillState.reset;
        }
        else
        {
            Debug.LogWarning("队列长度 ： " + SkillManager.GetInstance().skillQueue.Count);
            foreach (var a in SkillManager.GetInstance().skillQueue)
            {
                Debug.LogWarning(a.Key.CName);
            }
        }
    }

    public abstract bool Check();   //检查技能执行条件，由技能内部决定何时调用
}

public abstract class NewSkill
{
    public Transform character;
    public SkillInfo skillInfo;
    public string CName { get { return skillInfo.cName; } }
    public string EName { get { return skillInfo.eName; } }
    public int SkillInfoID { get { return skillInfo.ID; } }
    //输入焦点
    public Vector3 focus;

    protected GameObject render;
    protected SkillRange range;
    protected Animator animator;
    protected List<Vector3> customizedRangeList = new List<Vector3>();
    protected List<Vector3> customizedHoverRangeList = new List<Vector3>();
    //友军阻挡技能范围
    protected bool aliesObstruct = false;
    protected bool enablePathFinding = false;
    protected bool rotateToPathDirection = true;

    private bool inputComplete;
    private WaitUntil waitInput;
    protected SkillManager skillManager;
    //初始化技能
    public virtual void Init(int skillID, Transform character)
    {
        this.character = character;
        render = character.Find("Render").gameObject;
        animator = character.GetComponent<Animator>();

        skillManager = SkillManager.GetInstance();
        skillInfo = SkillInfoDictionary.GetNewParam(skillID);

        inputComplete = false;
        waitInput = new WaitUntil(() => inputComplete == true);
    }
    protected void RangeInit()
    {
        if (skillInfo.skillRange > 0)
        {
            range = new SkillRange();
            switch (skillInfo.rangeType)
            {
                case RangeType.common:
                    range.CreateSkillRange(skillInfo.skillRange, character);
                    break;
                case RangeType.straight:
                    range.CreateStraightSkillRange(skillInfo.skillRange, character, aliesObstruct);
                    break;
                case RangeType.other:
                    range.CreateCustomizedRange(customizedRangeList, customizedHoverRangeList, enablePathFinding, character);
                    break;
            }

            focus = new Vector3(-1, -1, -1);

            foreach (var f in BattleFieldManager.GetInstance().floors)
            {
                if (f.Value.activeInHierarchy)
                {
                    var player = RoundManager.GetInstance().Players.Find(p => p.playerNumber == SkillManager.GetInstance().skillQueue.Peek().Key.character.GetComponent<Unit>().playerNumber);

                    if (player is HumanPlayer || (player is AIPlayer && ((AIPlayer)player).AIControl == false))
                    {
                        f.Value.GetComponent<Floor>().FloorClicked += Confirm;
                        f.Value.GetComponent<Floor>().FloorExited += DeleteHoverRange;
                        f.Value.GetComponent<Floor>().FloorHovered += Focus;
                    }
                }
            }
            //角色加入忽略层，方便选取
            RoundManager.GetInstance().Units.ForEach(u => u.gameObject.layer = 2);

            range.RecoverColor();
        }
        else
        {
            Focus(character.gameObject);
            ShowConfirm();
        }
    }

    protected virtual void ShowConfirm()
    {
        ConfirmView.GetInstance().Open(ResetSelf, Confirm);
    }
    protected virtual void Confirm(GameObject sender)
    {
        inputComplete = true;
    }

    //confirmUI的回调函数
    public virtual void Confirm()
    {
        //角色取出忽略层
        RoundManager.GetInstance().Units.ForEach(u => u.gameObject.layer = 0);
        DebugLogPanel.GetInstance().Log(character.GetComponent<Unit>().roleCName + " 使用了 " + CName);
    }

    private void DeleteHoverRange(GameObject sender)
    {
        range.DeleteHoverRange();
        range.RecoverColor();
    }

    private void Focus(GameObject sender)
    {
        focus = sender.transform.position;
        if (skillInfo.skillRange > 0)
            range.ExcuteChangeColorAndRotate(skillInfo.hoverRange, skillInfo.skillRange, focus, rotateToPathDirection);
    }

    public virtual IEnumerator Excute()
    {

        yield return waitInput;

        yield return skillManager.StartCoroutine(Perform());
    }

    //视觉演绎
    protected abstract IEnumerator Perform();

    protected virtual void ResetSelf()
    {
        RoundManager.GetInstance().Units.ForEach(u => u.gameObject.layer = 0);
        if (range != null)
            range.Reset();
        foreach (var f in BattleFieldManager.GetInstance().floors)
        {
            f.Value.GetComponent<Floor>().FloorClicked -= Confirm;
            f.Value.GetComponent<Floor>().FloorExited -= DeleteHoverRange;
            f.Value.GetComponent<Floor>().FloorHovered -= Focus;
        }
    }

    public virtual void Reset()
    {
        //回退至上一个技能。
        if (SkillManager.GetInstance().skillQueue.Count == 1)
        {
            character.GetComponent<Unit>().action.Pop();
            character.GetComponent<CharacterAction>().SetSkill(character.GetComponent<Unit>().action.Pop().EName);
        }
        else
        {
            Debug.LogWarning("队列长度 ： " + SkillManager.GetInstance().skillQueue.Count);
            foreach (var a in SkillManager.GetInstance().skillQueue)
            {
                Debug.LogWarning(a.Key.CName);
            }
        }
    }

    public virtual bool Filter(Skill sender)
    {
        return CheckCost(sender);
    }

    protected virtual bool CheckCost(Skill sender)
    {
        var currentHP = sender.character.GetComponent<Unit>().attributes.Find(d => d.eName == "hp").Value;
        var currentMP = sender.character.GetComponent<Unit>().attributes.Find(d => d.eName == "mp").Value;
        if (sender is UnitSkill)
        {
            if (((UnitSkill)sender).skillInfo.costMP + skillInfo.costMP <= currentMP)
            {
                if (((UnitSkill)sender).skillInfo.costHP + skillInfo.costHP <= currentHP)
                {
                    return true;
                }
                else
                {
                    //DebugLogPanel.GetInstance().Log("体力不足！");
                    //Debug.Log("体力不足！");
                }
            }
            else
            {
                //DebugLogPanel.GetInstance().Log("查克拉不足！");
                //Debug.Log("查克拉不足！");
            }
        }
        else
        {
            if (skillInfo.costMP <= currentMP)
            {
                if (skillInfo.costHP <= currentHP)
                {
                    return true;
                }
                else
                {
                    //DebugLogPanel.GetInstance().Log("体力不足！");
                    //Debug.Log("体力不足！");
                }
            }
            else
            {
                //DebugLogPanel.GetInstance().Log("查克拉不足！");
                //Debug.Log("查克拉不足！");
            }
        }

        return false;
    }

    protected virtual void Cost()
    {
        var hpAttribute = character.GetComponent<Unit>().attributes.Find(d => d.eName == "hp");
        var mpAttribute = character.GetComponent<Unit>().attributes.Find(d => d.eName == "mp");
        var currentHP = hpAttribute.Value;
        var currentMP = mpAttribute.Value;

        var hp = currentHP - skillInfo.costHP;
        var mp = currentMP - skillInfo.costMP;
        hpAttribute.ChangeValueTo(hp);
        mpAttribute.ChangeValueTo(mp);
        if (skillInfo.costHP > 0)
            UIManager.GetInstance().FlyNum(character.GetComponent<Unit>().arrowPosition / 2 + character.position + Vector3.down * 0.2f, "-" + skillInfo.costHP, Utils_Color.hpColor);
        if (skillInfo.costMP > 0)
            UIManager.GetInstance().FlyNum(character.GetComponent<Unit>().arrowPosition / 2 + character.position + Vector3.down * 0.4f, "-" + skillInfo.costMP, Utils_Color.mpColor);
    }
}
