using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public enum ComboType
{
    empty,
    cannot,
    can,
    must
}

public enum SkillType
{
    empty,
    attack,
    effect,
    defence,
    dodge,
}

public enum SkillClass
{
    empty,
    ninjutsu,
    taijutsu,
    passive,
    tool,
    other,
}

public enum RangeType
{
    empty,
    common,
    straight,
    other
}

//角色可使用的主动技能类。
public abstract class UnitSkill : Skill
{
    //输入焦点
    public Vector3 focus;
    //originSkill是指组合技的第一个技能。
    public UnitSkill originSkill = null;
    //comboSkill是指组合技的第二个技能。
    public UnitSkill comboSkill = null;

    protected SkillRange range;
    protected Animator animator;
    protected GameObject comboSelectUI;
    protected GameObject render;
    public bool complete = false;
    protected bool rotateToPathDirection = true;
    protected bool aliesObstruct = false;   //友军阻挡技能范围
    protected List<Vector3> customizedRangeList = new List<Vector3>();
    protected List<Vector3> customizedHoverRangeList = new List<Vector3>();
    protected bool enablePathFinding = false;
    //输入最终确定。
    private bool final;

    private Dictionary<GameObject, ItemRecord> buttonRecord = new Dictionary<GameObject, ItemRecord>();

    public override bool Init(Transform character)
    {
        this.character = character;
        render = character.Find("Render").gameObject;
        if (skillInfo.skillType != SkillType.dodge)
        {
            //if (!CheckCost())
            //{
            //    Reset();
            //    return false;
            //}
        }

        //此处设定的是深度复制的技能实例。

        if (!(this is INinjaTool))
        {
            SetLevel(character.GetComponent<Unit>().skills[skillInfo.ID]);
        }

        animator = character.GetComponent<Animator>();


        if (originSkill == null)
        {
            switch (skillInfo.comboType)
            {
                case ComboType.cannot:
                    RangeInit();
                    break;
                case ComboType.can:
                    //继续使用连续攻击进行攻击吗？
                    ConfirmView.GetInstance().Open("继续使用连续攻击 进行攻击吗？", RangeInit, CreateComboSelectUI);
                    break;
                case ComboType.must:
                    //请选择要使用连续攻击的攻击类术·忍具。
                    ConfirmView.GetInstance().Open("请选择要使用连续攻击的 攻击类术·忍具。", "", "确定", null, CreateComboSelectUI);
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
        List<GameObject> allButtons;
        Func<UnitSkill, bool> comboFilter = ComboFilter;
        comboSelectUI = UIManager.GetInstance().CreateButtonList(character, this, out allButtons, ref buttonRecord, comboFilter);
        foreach (var button in allButtons)
        {
            button.GetComponent<Button>().onClick.AddListener(OnButtonClick);
        }
        comboSelectUI.transform.Find("Return").GetComponent<Button>().onClick.AddListener(Reset);
        comboSelectUI.SetActive(true);
    }

    protected virtual bool ComboFilter(UnitSkill unitSkill)
    {
        return unitSkill.skillInfo.skillType == SkillType.attack;
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

            character.GetComponent<CharacterAction>().SetItem(btn.name, buttonRecord[btn]);
            var skill = character.GetComponent<Unit>().action.Peek() as UnitSkill;
            skill.originSkill = this;
            if (comboSelectUI)
                UnityEngine.Object.Destroy(comboSelectUI);
            skillState = SkillState.reset;
        }
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
            final = false;
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
                    RoundManager.GetInstance().Units.ForEach(u => u.gameObject.layer = 0);

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
                        var player = RoundManager.GetInstance().Players.Find(p => p.playerNumber == SkillManager.GetInstance().skillQueue.Peek().Key.character.GetComponent<Unit>().playerNumber);
                        if (player is HumanPlayer || (player is AIPlayer && ((AIPlayer)player).AIControl == false))
                        {
                            ShowConfirm();
                        }
                    }
                    RoundManager.GetInstance().Units.ForEach(u => u.gameObject.layer = 2);
                    final = false;
                }
                break;
            case SkillState.confirm:

                if (originSkill != null)
                {
                    //有连续技。连续技第二个技能的InitSkill在AttackSkill的ApplyEffects中进行处理。
                    originSkill.InitSkill();
                }
                else
                {
                    //无连续技
                    InitSkill();
                }
                if (this is INinjaTool && !(this is Substitute))
                    ((INinjaTool)this).RemoveSelf(character);
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

    private void DeleteHoverRange(GameObject sender)
    {
        range.DeleteHoverRange();
        range.RecoverColor();
    }

    private void Focus(GameObject sender)
    {
        focus = sender.transform.position;
        if (originSkill != null)
            originSkill.focus = focus;
        if (skillInfo.skillRange > 0)
            range.ExcuteChangeColorAndRotate(skillInfo.hoverRange, skillInfo.skillRange, focus, rotateToPathDirection);
    }

    /// <summary>
    /// AI
    /// </summary>
    /// <param name="floorPosition"></param>
    public void Focus(Vector3 floorPosition)
    {
        focus = floorPosition;
        range.ExcuteChangeColorAndRotate(skillInfo.hoverRange, skillInfo.skillRange, focus, rotateToPathDirection);
        final = true;
    }

    protected virtual void ShowConfirm()
    {
        ConfirmView.GetInstance().Open(ResetSelf, Confirm);
    }

    protected virtual void Confirm(GameObject sender)
    {
        final = true;
    }

    //confirmUI的回调函数
    public virtual void Confirm()
    {
        done = true;
        //角色取出忽略层
        RoundManager.GetInstance().Units.ForEach(u => u.gameObject.layer = 0);
        skillState = SkillState.confirm;
        if (originSkill != null)
            DebugLogPanel.GetInstance().Log(character.GetComponent<Unit>().roleCName + " 使用了 " + originSkill.CName + " + " + CName);
        else
            DebugLogPanel.GetInstance().Log(character.GetComponent<Unit>().roleCName + " 使用了 " + CName);
    }

    protected virtual void InitSkill()
    {
        //连续技第一个。第二个在AttackSkill中。
        if (originSkill == null && comboSkill != null)
        {
            comboSkill.range.DeleteHoverRange();
            comboSkill.range.Delete();
            comboSkill.character.GetComponent<Unit>().action.Clear();  //禁止回退
            comboSkill.animator.SetInteger("Skill", skillInfo.animID);
        }
        if (originSkill == null && comboSkill == null)
        {

            if (range != null)
            {
                range.DeleteHoverRange();
                range.Delete();
            }
            character.GetComponent<Unit>().action.Clear();  //禁止回退
            animator.SetInteger("Skill", skillInfo.animID);
        }

    }

    /// <summary>
    /// 用来向技能面板输出本技能的效果和数值。长度为2或3。0位为Title，1位为Info,3位为DurationInfo。
    /// </summary>
    /// <returns></returns>
    public virtual List<string> LogSkillEffect()
    {
        string title = "";
        string info = "";
        List<string> s = new List<string>
        {
            title,
            info
        };
        return s;
    }

    /// <summary>
    /// ApplyEffects是一个时间段，这个时间段用来进行技能展示。
    /// 基类为默认实现，并且在OnUpdate中仍然进行了动画归位。（可能重复）
    /// </summary>
    /// <returns></returns>
    protected virtual bool ApplyEffects()
    {
        if (complete)
        {
            character.GetComponent<CharacterAction>().SetSkill("ChooseDirection");
            return true;
        }
        return false;
    }

    /// <summary>
    /// Effect是一个时间点，由动画事件调用。
    /// 必须调用基类以完成消耗扣除和动画归位。
    /// </summary>
    public virtual void Effect()
    {
        Cost();
        animator.SetInteger("Skill", 0);
    }

    protected virtual void ResetSelf()
    {
        if (comboSelectUI)
            GameObject.Destroy(comboSelectUI);
        ConfirmView.TryClose();
        RoundManager.GetInstance().Units.ForEach(u => u.gameObject.layer = 0);
        if (range != null)
            range.Reset();
        foreach (var f in BattleFieldManager.GetInstance().floors)
        {
            f.Value.GetComponent<Floor>().FloorClicked -= Confirm;
            f.Value.GetComponent<Floor>().FloorExited -= DeleteHoverRange;
            f.Value.GetComponent<Floor>().FloorHovered -= Focus;
        }


        skillState = SkillState.init;
    }

    /// <summary>
    /// 与ResetSelf的区别：Reset在Skill层对技能进行出列入列，而ResetSelf仅用于类似替身术时候的自身重置。
    /// </summary>
    public override void Reset()
    {

        //按照顺序，逆序消除影响。因为每次会Init()，所以不必都Reset。

        if (range != null)
            range.Reset();

        foreach (var f in BattleFieldManager.GetInstance().floors)
        {
            f.Value.GetComponent<Floor>().FloorClicked -= Confirm;
            f.Value.GetComponent<Floor>().FloorExited -= DeleteHoverRange;
            f.Value.GetComponent<Floor>().FloorHovered -= Focus;
        }

        //角色取出忽略层
        RoundManager.GetInstance().Units.ForEach(u => u.gameObject.layer = 0);
        if (comboSelectUI)
            GameObject.Destroy(comboSelectUI);
        ConfirmView.TryClose();
        if (originSkill != null)
        {
            originSkill.ResetSelf();
            character.GetComponent<Unit>().action.Pop();

        }

        base.Reset();
    }

    public override bool Check()
    {
        return true;
    }

    /// <summary>
    /// 技能列表中的条件过滤。
    /// 请通过sender来获取character，因为在这时Skill本身的character还未初始化。
    /// 请勿通过此方法赋值类中的变量，因为此方法先行于其他方法，它们往往不属于同一个实例。
    /// </summary>
    /// <param name="sender"></param>
    /// <returns></returns>
    public virtual bool Filter(Skill sender)
    {
        return CheckCost(sender.character, sender);
    }

    protected virtual bool CheckCost(Transform character, Skill sender)
    {
        var currentHP = character.GetComponent<Unit>().attributes.Find(d => d.eName == "hp").Value;
        var currentMP = character.GetComponent<Unit>().attributes.Find(d => d.eName == "mp").Value;
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

    public virtual void Complete()
    {
        complete = true;
    }
}
