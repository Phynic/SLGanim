using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AttackSkill : UnitSkill
{
    public int damageFactor;
    public int hit;
    public int finalFactor = 0;     //最终伤害加成
    protected int extraCrit = 5;
    protected int extraPounce = 5;
    private int pointerIterator = 0;
    public List<Transform> other = new List<Transform>();
    private GameObject expectationUI;
    private GameObject pointer;
    private List<KeyValuePair<CharacterStatus, string[]>> expectationList = new List<KeyValuePair<CharacterStatus, string[]>>();

    private List<List<Transform>> comboUnitsList = new List<List<Transform>>();
    private Dictionary<Transform, Vector3> comboUnitsOriginDirection = new Dictionary<Transform, Vector3>();
    private List<GameObject> arrowList = new List<GameObject>();
    EffectState effectState = EffectState.general;
    private enum EffectState
    {
        
        origin,
        general
    }

    public AttackSkill()
    {
        var attackSkillData = (AttackSkillData)skillData;
        damageFactor = attackSkillData.damageFactor;
        hit = attackSkillData.hit;
        extraCrit = attackSkillData.extraCrit;
        extraPounce = attackSkillData.extraPounce;
    }

    public override bool Init(Transform character)
    {
        expectationList.Clear();
        comboUnitsList.Clear();
        comboUnitsOriginDirection.Clear();
        arrowList.Clear();
        other.Clear();
        CreateUI();
        if (!base.Init(character))
            return false;
        return true;
    }

    private void CreateUI()
    {
        var go = (GameObject)Resources.Load("Prefabs/UI/ExpectationPanel");
        var go1 = (GameObject)Resources.Load("Prefabs/UI/Pointer");
        pointer = UnityEngine.Object.Instantiate(go1);
        expectationUI = UnityEngine.Object.Instantiate(go, GameObject.Find("Canvas").transform);
        expectationUI.transform.Find("Left").GetComponent<Button>().onClick.AddListener(PreviousUnit);
        expectationUI.transform.Find("Right").GetComponent<Button>().onClick.AddListener(NextUnit);
        pointer.SetActive(false);
        expectationUI.SetActive(false);
    }

    private void NextUnit()
    {
        pointerIterator = pointerIterator == expectationList.Count - 1 ? expectationList.Count - 1 : pointerIterator + 1;
        pointer.transform.SetParent(expectationList[pointerIterator].Key.transform);
        expectationUI.transform.Find("ExpectationTextLeft").GetComponent<Text>().text = expectationList[pointerIterator].Value[0];
        expectationUI.transform.Find("ExpectationTextRight").GetComponent<Text>().text = expectationList[pointerIterator].Value[1];
    }

    private void PreviousUnit()
    {
        pointerIterator = pointerIterator == 0 ? 0 : pointerIterator - 1;
        pointer.transform.SetParent(expectationList[pointerIterator].Key.transform);
        expectationUI.transform.Find("ExpectationTextLeft").GetComponent<Text>().text = expectationList[pointerIterator].Value[0];
        expectationUI.transform.Find("ExpectationTextRight").GetComponent<Text>().text = expectationList[pointerIterator].Value[1];
    }

    protected virtual void ShowUI()
    {
        foreach(var o in other)
        {
            var a = o.GetComponent<CharacterStatus>().attributes;
            var n = o.GetComponent<CharacterStatus>().roleCName;
            var atk = a.Find(d => d.eName == "atk").value.ToString();
            var def = a.Find(d => d.eName == "def").value.ToString();
            var dex = a.Find(d => d.eName == "dex").value.ToString();
            var currentHp = a.Find(d => d.eName == "hp").value.ToString();
            var currentMp = a.Find(d => d.eName == "mp").value.ToString();
            FinalDamageBuff finalDamageBuff = (FinalDamageBuff)character.GetComponent<Unit>().Buffs.Find(b => b.GetType() == typeof(FinalDamageBuff));
            var damageExpectation = DamageSystem.ExpectDamage(character, o, damageFactor, hit, hoverRange == 0, finalDamageBuff == null ? 0 : finalDamageBuff.Factor);
            var finalRate = DamageSystem.HitRateSystem(character, o, skillRate).ToString();

            if(originSkill != null && originSkill is AttackSkill)
            {
                var originAttackSkill = (AttackSkill)originSkill;
                damageExpectation += DamageSystem.ExpectDamage(character, o, originAttackSkill.damageFactor, originAttackSkill.hit, hoverRange == 0, finalDamageBuff == null ? 0 : finalDamageBuff.Factor);
                finalRate = DamageSystem.HitRateSystem(character, o, (skillRate * hit + originAttackSkill.skillRate * originAttackSkill.hit) / (hit + originAttackSkill.hit)).ToString();
            }
            
            var expectationTextLeft = "  " + n + "\n" + "  攻击力 ： " + atk + "\n" + "  防御力 ： " + def + "\n" + "  敏捷度 ： " + dex;
            var expectationTextRight = "  体力 ： " + currentHp + "\n" + "  查克拉 ： " + currentMp + "\n" +"  成功率 ： " + finalRate + "\n" + "  伤害期望 ： " + damageExpectation.ToString();
            expectationList.Add(new KeyValuePair<CharacterStatus, string[]>(o.GetComponent<CharacterStatus>(), new string[2] { expectationTextLeft, expectationTextRight }));

            //相同外观角色的合击逻辑
            var comboUnits = DamageSystem.ComboDetect(character, o);
            if (comboUnits.Count > 0)
            {
                foreach (var u in comboUnits)
                {
                    var go = (GameObject)Resources.Load("Prefabs/UI/Arrows");
                    var arrow = GameObject.Instantiate(go);

                    arrow.transform.position = u.GetComponent<CharacterStatus>().arrowPosition + u.position;
                    var arrows = arrow.GetComponentsInChildren<Transform>();
                    foreach (var d in arrows)
                    {
                        if(d != arrow.transform)
                        {
                            d.gameObject.SetActive(false);
                            
                            if (d.localPosition.normalized == (o.position - u.position).normalized)
                            {
                                d.gameObject.SetActive(true);
                                d.GetComponent<Renderer>().material.color = Color.red;
                            }
                        }
                    }
                    arrowList.Add(arrow);
                }
                comboUnitsList.Add(comboUnits);
            }
        }

        expectationUI.transform.Find("ExpectationTextLeft").GetComponent<Text>().text = expectationList[0].Value[0];
        expectationUI.transform.Find("ExpectationTextRight").GetComponent<Text>().text = expectationList[0].Value[1];
        pointer.transform.SetParent(expectationList[pointerIterator].Key.transform);
        pointer.transform.localPosition = expectationList[pointerIterator].Key.arrowPosition;
        pointer.SetActive(true);
        expectationUI.SetActive(true);
    }

    protected override void ShowConfirm()
    {
        base.ShowConfirm();
        
        ShowUI();
    }

    public override void SetLevel(int level)
    {
        throw new NotImplementedException();
    }

    protected override bool ApplyEffects()
    {
        switch (effectState)
        {
            case EffectState.origin:
                if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
                {
                    InitSkill();
                    effectState = EffectState.general;
                }
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

                if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
                {
                    if (comboUnitsList.Count > 0)
                    {
                        foreach (var comboUnits in comboUnitsList)
                        {
                            if (comboUnits.FindAll(u => u.forward != comboUnitsOriginDirection[u]).Count == 0)
                            {
                                character.GetComponent<CharacterAction>().SetSkill("ChooseDirection");
                                return true;
                            }
                        }
                    }
                    else
                    {
                        character.GetComponent<CharacterAction>().SetSkill("ChooseDirection");
                        return true;
                    }
                }
                break;
        }
        return false;
    }

    protected override void InitSkill()
    {
        base.InitSkill();
        foreach (var o in other)
        {
            if (comboSkill == null)
            {
                var comboUnits = DamageSystem.ComboDetect(character, o);
                if (comboUnits.Count > 0)
                {
                    foreach (var u in comboUnits)
                    {
                        var ninjaCombo = new NinjaCombo();
                        ninjaCombo.SetLevel(u.GetComponent<CharacterStatus>().skills["NinjaCombo"]);
                        comboUnitsOriginDirection.Add(u, u.forward);
                        u.forward = o.position - u.position;
                        u.GetComponent<Animator>().SetInteger("Skill", 1);
                    }
                }
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
            base.Effect();

            FinalDamageBuff finalDamageBuff = (FinalDamageBuff)character.GetComponent<Unit>().Buffs.Find(b => b.GetType() == typeof(FinalDamageBuff));

            foreach (var o in other)
            {
                //每Hit
                for (int i = 0; i < hit; i++)
                {
                    
                    if (!DamageSystem.Apply(character, o, damageFactor, skillRate, extraCrit, extraPounce, comboSkill == null && hoverRange == 0 || comboSkill != null && comboSkill.hoverRange == 0, finalDamageBuff == null ? 0 : finalDamageBuff.Factor))
                        break;
                }
                if(finalDamageBuff != null && finalDamageBuff.Duration < 0)
                {
                    finalDamageBuff.Undo(character);
                }
                //comboSkill是指组合技的第二个技能。
                if (comboSkill == null)
                {
                    var comboUnits = DamageSystem.ComboDetect(character, o);
                    if (comboUnits.Count > 0)
                    {
                        
                        foreach (var u in comboUnits)
                        {
                            FinalDamageBuff u_finalDamageBuff = (FinalDamageBuff)u.GetComponent<Unit>().Buffs.Find(b => b.GetType() == typeof(FinalDamageBuff));
                            var ninjaCombo = new NinjaCombo();
                            ninjaCombo.SetLevel(u.GetComponent<CharacterStatus>().skills["NinjaCombo"]);
                            DamageSystem.Apply(u, o, ninjaCombo.damageFactor, ninjaCombo.skillRate, ninjaCombo.extraCrit, ninjaCombo.extraPounce, ninjaCombo.hoverRange == 0, u_finalDamageBuff == null ? 0 : u_finalDamageBuff.Factor);
                            u.GetComponent<Animator>().SetInteger("Skill", 0);
                        }
                    }
                }
            }
        }
    }

    protected override void Confirm()
    {
        if (pointer)
            UnityEngine.Object.Destroy(pointer);
        if (expectationUI)
            UnityEngine.Object.Destroy(expectationUI);
        foreach(var arrow in arrowList)
        {
            if (arrow)
                UnityEngine.Object.Destroy(arrow);
        }

        if(originSkill != null)
        {
            if(originSkill is AttackSkill)
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
        if(pointer)
            UnityEngine.Object.Destroy(pointer);
        if (expectationUI)
            UnityEngine.Object.Destroy(expectationUI);
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
        if (expectationUI)
            UnityEngine.Object.Destroy(expectationUI);
        foreach (var arrow in arrowList)
        {
            if (arrow)
                UnityEngine.Object.Destroy(arrow);
        }
        arrowList.Clear();
    }

    public override bool Check()
    {
        other.Clear();
        
        var list = Detect.DetectObjects(hoverRange, focus);

        foreach (var l in list)
        {
            foreach (var u in l)
            {
                if (u.GetComponent<CharacterStatus>())
                {
                    if (character.GetComponent<CharacterStatus>().IsEnemy(u.GetComponent<CharacterStatus>()))
                    {
                        other.Add(u);
                    }
                }
            }
        }
        //other = other.Distinct().ToList();
        if (other.Count > 0)
        {
            return true;
        }
        return false;
    }
}
