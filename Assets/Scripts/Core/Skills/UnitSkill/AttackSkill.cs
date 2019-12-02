using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum EffectState
{
    origin,
    general
}

public class AttackSkill : UnitSkill
{
    public int finalFactor = 0;     //最终伤害加成
    protected int extraHit = 0;

    private int pointerIterator = 0;
    protected bool calculateDamage = true;
    protected float hitInterval = 0.2f;
    public bool skipDodge = false;
    public List<Transform> other = new List<Transform>();

    private GameObject pointer;

    private List<List<Transform>> comboUnitsList = new List<List<Transform>>();
    private Dictionary<Transform, Vector3> comboUnitsOriginDirection = new Dictionary<Transform, Vector3>();
    private List<GameObject> arrowList = new List<GameObject>();
    public EffectState effectState = EffectState.general;

    public override bool Init(Transform character)
    {
        AddPassiveSkillEffect();

        comboUnitsList.Clear();
        comboUnitsOriginDirection.Clear();
        arrowList.Clear();
        other.Clear();
        CreateUI();

        if (!base.Init(character))
            return false;
        return true;
    }

    private void AddPassiveSkillEffect()
    {
        if (RoundManager.GetInstance())
        {
            var currentUnit = RoundManager.GetInstance().CurrentUnit;
            if (currentUnit)
            {
                int lev;
                switch (skillInfo.skillClass)
                {
                    case SkillClass.ninjutsu:
                        if (RoundManager.GetInstance().CurrentUnit.GetComponent<Unit>().skills.TryGetValue(5001, out lev))
                        {
                            var factor = ((PassiveSkill)SkillManager.GetInstance().skillList.Find(s => s.SkillInfoID == 5001)).skillInfo.factor;
                            skillInfo.extraCrit += lev * factor;
                        }
                        break;
                    case SkillClass.taijutsu:
                        if (RoundManager.GetInstance().CurrentUnit.GetComponent<Unit>().skills.TryGetValue(5002, out lev))
                        {
                            var factor = ((PassiveSkill)SkillManager.GetInstance().skillList.Find(s => s.SkillInfoID == 5002)).skillInfo.factor;
                            skillInfo.extraPounce += lev * factor;
                        }
                        break;
                    case SkillClass.tool:
                        if (RoundManager.GetInstance().CurrentUnit.GetComponent<Unit>().skills.TryGetValue(5003, out lev))
                        {
                            var factor = ((PassiveSkill)SkillManager.GetInstance().skillList.Find(s => s.SkillInfoID == 5003)).skillInfo.factor;
                            extraHit = lev * factor;
                        }
                        break;
                    case SkillClass.other:
                        break;
                    default:
                        break;
                }
            }
        }
    }

    private void CreateUI()
    {
        var go = (GameObject)Resources.Load("Prefabs/UI/Pointer");
        pointer = UnityEngine.Object.Instantiate(go);
        pointer.SetActive(false);
    }

    private void NextUnit()
    {
        pointerIterator = pointerIterator == other.Count - 1 ? other.Count - 1 : pointerIterator + 1;
        pointer.transform.SetParent(other[pointerIterator]);
        Camera.main.GetComponent<RTSCamera>().FollowTarget(other[pointerIterator].position);
        ExpectationView.GetInstance().Refresh(pointerIterator);
    }

    private void PreviousUnit()
    {
        pointerIterator = pointerIterator == 0 ? 0 : pointerIterator - 1;
        pointer.transform.SetParent(other[pointerIterator]);
        Camera.main.GetComponent<RTSCamera>().FollowTarget(other[pointerIterator].position);
        ExpectationView.GetInstance().Refresh(pointerIterator);
    }

    protected virtual void ShowUI()
    {
        var expectations = new List<int>();
        var finalRates = new List<string>();
        
        foreach (var o in other)
        {
            FinalDamageBuff finalDamageBuff = (FinalDamageBuff)character.GetComponent<Unit>().Buffs.Find(b => b.GetType() == typeof(FinalDamageBuff));
            var expectation = DamageSystem.Expect(character, o, skillInfo.damage, skillInfo.hit, skillInfo.hoverRange == 0, finalDamageBuff == null ? 0 : finalDamageBuff.Factor);
            var finalRate = DamageSystem.HitRateSystem(character, o, skillInfo.skillRate).ToString();

            if (originSkill != null && originSkill is AttackSkill)
            {
                var originAttackSkill = (AttackSkill)originSkill;
                expectation += DamageSystem.Expect(character, o, originAttackSkill.skillInfo.damage, originAttackSkill.skillInfo.hit, skillInfo.hoverRange == 0, finalDamageBuff == null ? 0 : finalDamageBuff.Factor);
                finalRate = DamageSystem.HitRateSystem(character, o, (skillInfo.skillRate * skillInfo.hit + originAttackSkill.skillInfo.skillRate * originAttackSkill.skillInfo.hit) / (skillInfo.hit + originAttackSkill.skillInfo.hit)).ToString();
            }

            expectations.Add(expectation);
            finalRates.Add(finalRate);
        }
        ExpectationView.GetInstance().Open(other, expectations, finalRates, PreviousUnit, NextUnit);

        pointer.transform.SetParent(other[pointerIterator]);
        pointer.transform.localPosition = other[pointerIterator].GetComponent<Unit>().arrowPosition;
        pointer.SetActive(true);
    }

    protected override void ShowConfirm()
    {
        base.ShowConfirm();

        ShowUI();
    }

    public override List<string> LogSkillEffect()
    {
        string title = "攻击力";
        string info = skillInfo.damage + "×" + skillInfo.hit;
        List<string> s = new List<string>
        {
            title,
            info
        };
        return s;
    }

    public virtual void GetHit()
    {
        if (calculateDamage)
        {
            //寻找是否有最终伤害buff
            FinalDamageBuff finalDamageBuff = (FinalDamageBuff)character.GetComponent<Unit>().Buffs.Find(b => b.GetType() == typeof(FinalDamageBuff));

            foreach (var o in other)
            {
                //<伤害序列，伤害结果>
                List<int> damageList = new List<int>();

                if (DamageSystem.ExtraHitSystem(extraHit))
                {
                    DebugLogPanel.GetInstance().Log("速击！" + "（" + character.GetComponent<Unit>().roleCName + " -> " + o.GetComponent<Unit>().roleCName + "）");
                    skillInfo.hit++;
                }

                //每Hit
                for (int i = 0; i < skillInfo.hit; i++)
                {
                    int d;
                    var doNextHit = DamageSystem.ApplyDamage(character, o, this, comboSkill == null && skillInfo.hoverRange == 0 || comboSkill != null && comboSkill.skillInfo.hoverRange == 0, finalDamageBuff == null ? 0 : finalDamageBuff.Factor, out d);
                    damageList.Add(d);
                    if (!doNextHit)
                    {
                        break;
                    }
                }

                //如果有最终伤害Buff，且Duration小于0，手动撤销影响。（此处应为倍化术）
                if (finalDamageBuff != null && finalDamageBuff.Duration < 0)
                {
                    finalDamageBuff.Undo(character);
                }

                for (int i = 0; i < damageList.Count; i++)
                {
                    bool donePost = false;
                    //异步时，要把迭代器传进去。
                    Utils_Coroutine.GetInstance().Invoke(j =>
                    {
                        if (o)
                        {
                            //飘字
                            if (skillInfo.damage > 0)
                            {
                                if (damageList[j] > 0)
                                {
                                    //受击动作
                                    HitEffect(o);

                                    UIManager.GetInstance().FlyNum(o.GetComponent<Unit>().arrowPosition / 2 + o.position, damageList[j].ToString(), Color.white, true);
                                }
                                else if (damageList[j] == 0)
                                {
                                    UIManager.GetInstance().FlyNum(o.GetComponent<Unit>().arrowPosition / 2 + o.position, damageList[j].ToString(), Color.white, false);
                                }
                                else if (damageList[j] < 0)
                                {
                                    //受击动作
                                    HitEffect(o);
                                }
                            }
                            else
                                UIManager.GetInstance().FlyNum(o.GetComponent<Unit>().arrowPosition / 2 + o.position, Mathf.Abs(damageList[j]).ToString(), Utils_Color.hpColor, false);
                            if (!donePost)
                            {
                                PostEffect(o);
                                donePost = true;
                            }
                        }
                    }, hitInterval * i, i);
                }
            }
        }
    }

    protected virtual void HitEffect(Transform o)
    {
        if (o.GetComponent<Animator>())
        {
            FXManager.GetInstance().HitPointSpawn(o.GetComponent<Animator>().GetBoneTransform(HumanBodyBones.Chest).position, Quaternion.identity, null, 1);
            o.GetComponent<Animator>().SetFloat("HitAngle", Vector3.SignedAngle(o.position - character.position, -o.forward, Vector3.up));
            //o.GetComponent<Animator>().Play("GetHit", 0, j == 0 ? 0 : 0.2f);
            //直接从0.2f开始放受击动画。
            o.GetComponent<Animator>().Play("GetHit", 0, 0.2f);
        }
        else
        {
            FXManager.GetInstance().HitPointSpawn(o.position + Vector3.up * 0.7f, Quaternion.identity, null, 1);
        }
    }

    protected override void InitSkill()
    {
        base.InitSkill();
        //连续技第二个
        if (originSkill != null && comboSkill == null)
        {

            int i = 0;
            foreach (var o in other)
            {
                if (o != null)
                {
                    var buff = o.GetComponent<Unit>().Buffs.Find(b => b.GetType() == typeof(DodgeBuff));
                    if (buff != null)
                    {
                        var b = (DodgeBuff)buff;
                        if (!b.done)
                        {
                            i++;
                        }
                    }
                    else
                    {
                        i++;
                    }
                }
            }
            if (i > 0)
            {
                animator.SetInteger("Skill", skillInfo.animID);
            }
            else
            {

                character.GetComponent<CharacterAction>().SetSkill("ChooseDirection");
                animator.applyRootMotion = false;
                skillState = SkillState.reset;
                return;
            }

        }
    }

    public override void Effect()
    {
        if (effectState == EffectState.origin)
        {
            originSkill.Effect();
        }
        else
        {
            //结算消耗以及动作归位。
            base.Effect();
        }
    }

    protected virtual void PostEffect(Transform o)
    {

    }

    protected override bool ApplyEffects()
    {
        switch (effectState)
        {
            case EffectState.origin:

                break;
            case EffectState.general:
                foreach (var comboUnits in comboUnitsList)
                {
                    foreach (var u in comboUnits)
                    {
                        if (u.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Idle"))
                        {
                            u.forward = comboUnitsOriginDirection[u];
                        }
                    }
                }

                if (complete)
                {
                    if (comboUnitsList.Count > 0)
                    {
                        foreach (var comboUnits in comboUnitsList)
                        {
                            if (comboUnits.FindAll(u => u.forward != comboUnitsOriginDirection[u]).Count == 0)
                            {
                                character.GetComponent<CharacterAction>().SetSkill("ChooseDirection");
                                animator.applyRootMotion = false;
                                return true;
                            }
                        }
                    }
                    else
                    {
                        character.GetComponent<CharacterAction>().SetSkill("ChooseDirection");
                        animator.applyRootMotion = false;
                        return true;
                    }
                }
                break;
        }
        return false;
    }

    public override void Confirm()
    {
        if (pointer)
            UnityEngine.Object.Destroy(pointer);
        ExpectationView.TryClose();
        foreach (var arrow in arrowList)
        {
            if (arrow)
                UnityEngine.Object.Destroy(arrow);
        }

        if (originSkill != null)
        {
            if (originSkill is AttackSkill)
            {
                var originAttackSkill = (AttackSkill)originSkill;
                originAttackSkill.other.Add(pointer.transform.parent);
            }
        }
        if (originSkill != null)
            effectState = EffectState.origin;
        arrowList.Clear();
        base.Confirm();
    }

    protected override void ResetSelf()
    {
        base.ResetSelf();
        if (pointer)
            UnityEngine.Object.Destroy(pointer);
        ExpectationView.TryClose();
        foreach (var arrow in arrowList)
        {
            if (arrow)
                UnityEngine.Object.Destroy(arrow);
        }
        arrowList.Clear();
    }

    public override void Reset()
    {
        base.Reset();
        if (pointer)
            UnityEngine.Object.Destroy(pointer);
        ExpectationView.TryClose();
        foreach (var arrow in arrowList)
        {
            if (arrow)
                UnityEngine.Object.Destroy(arrow);
        }
        arrowList.Clear();
    }

    //检测范围内目标，符合条件的可被添加至other容器。AttackSkill中默认选中敌人，有特殊需求请覆盖。
    public override bool Check()
    {
        other.Clear();
        List<List<Transform>> list;
        if (customizedHoverRangeList.Count > 0)
        {
            list = Detect.DetectObjects(customizedHoverRangeList);
        }
        else
        {
            List<Vector3> hover = new List<Vector3>();
            foreach (var item in range.hoverRangeList)
            {
                hover.Add(item.transform.position);
            }
            list = Detect.DetectObjects(hover);
        }


        foreach (var l in list)
        {
            foreach (var u in l)
            {
                if (u.GetComponent<Unit>())
                {
                    if (character.GetComponent<Unit>().IsEnemy(u.GetComponent<Unit>()))
                    {
                        other.Add(u);
                    }
                }
            }
        }

        if (other.Count > 0)
        {
            return true;
        }
        return false;
    }

    public override void Complete()
    {
        if (effectState == EffectState.origin)
        {
            InitSkill();
            effectState = EffectState.general;
        }
        else
        {
            base.Complete();
        }
    }
}
