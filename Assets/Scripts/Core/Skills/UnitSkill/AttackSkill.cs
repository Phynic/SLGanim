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
    public int hit;
    public int damage;
    public int finalFactor = 0;     //最终伤害加成
    public int extraCrit = 0;
    public int extraPounce = 0;
    protected int extraHit = 0;

    private int pointerIterator = 0;
    protected bool calculateDamage = true;
    protected float hitInterval = 0.2f;
    public bool skipDodge = false;
    public List<Transform> other = new List<Transform>();
    private GameObject expectationUI;
    private GameObject pointer;
    private List<KeyValuePair<CharacterStatus, string[]>> expectationList = new List<KeyValuePair<CharacterStatus, string[]>>();

    private List<List<Transform>> comboUnitsList = new List<List<Transform>>();
    private Dictionary<Transform, Vector3> comboUnitsOriginDirection = new Dictionary<Transform, Vector3>();
    private List<GameObject> arrowList = new List<GameObject>();
    public EffectState effectState = EffectState.general;


    public AttackSkill()
    {
        damage = skillInfo.damage;
        hit = skillInfo.hit;
        extraCrit = skillInfo.extraCrit;
        extraPounce = skillInfo.extraPounce;
        if (RoundManager.GetInstance())
        {
            var currentUnit = RoundManager.GetInstance().CurrentUnit;
            if (currentUnit)
            {
                int lev;
                switch (skillClass)
                {
                    case SkillClass.ninjutsu:
                        if (RoundManager.GetInstance().CurrentUnit.GetComponent<CharacterStatus>().skills.TryGetValue("QuickCharge", out lev))
                        {
                            var factor = ((PassiveSkill)SkillManager.GetInstance().skillList.Find(s => s.EName == "QuickCharge")).skillInfo.factor;
                            extraCrit += lev * factor;
                        }
                        break;
                    case SkillClass.taijutsu:
                        if (RoundManager.GetInstance().CurrentUnit.GetComponent<CharacterStatus>().skills.TryGetValue("Pounce", out lev))
                        {
                            var factor = ((PassiveSkill)SkillManager.GetInstance().skillList.Find(s => s.EName == "Pounce")).skillInfo.factor;
                            extraPounce += lev * factor;
                        }
                        break;
                    case SkillClass.tool:
                        if (RoundManager.GetInstance().CurrentUnit.GetComponent<CharacterStatus>().skills.TryGetValue("ThrowingPractice", out lev))
                        {
                            var factor = ((PassiveSkill)SkillManager.GetInstance().skillList.Find(s => s.EName == "ThrowingPractice")).skillInfo.factor;
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
        Camera.main.GetComponent<RTSCamera>().FollowTarget(expectationList[pointerIterator].Key.transform.position);
        RefreshExpectionData(pointerIterator);
    }

    private void PreviousUnit()
    {
        pointerIterator = pointerIterator == 0 ? 0 : pointerIterator - 1;
        pointer.transform.SetParent(expectationList[pointerIterator].Key.transform);
        Camera.main.GetComponent<RTSCamera>().FollowTarget(expectationList[pointerIterator].Key.transform.position);
        RefreshExpectionData(pointerIterator);
    }

    protected virtual void ShowUI()
    {
        if (other.Count <= 1)
        {
            expectationUI.transform.Find("Left").gameObject.SetActive(false);
            expectationUI.transform.Find("Right").gameObject.SetActive(false);
        }
        foreach (var o in other)
        {
            var a = o.GetComponent<CharacterStatus>().attributes;
            var atk = a.Find(d => d.eName == "atk").value.ToString();
            var def = a.Find(d => d.eName == "def").value.ToString();
            var dex = a.Find(d => d.eName == "dex").value.ToString();
            var currentHp = a.Find(d => d.eName == "hp").value.ToString();
            var currentMp = a.Find(d => d.eName == "mp").value.ToString();
            FinalDamageBuff finalDamageBuff = (FinalDamageBuff)character.GetComponent<Unit>().Buffs.Find(b => b.GetType() == typeof(FinalDamageBuff));
            var expectation = DamageSystem.Expect(character, o, damage, hit, hoverRange == 0, finalDamageBuff == null ? 0 : finalDamageBuff.Factor);
            var finalRate = DamageSystem.HitRateSystem(character, o, skillRate).ToString();

            if (originSkill != null && originSkill is AttackSkill)
            {
                var originAttackSkill = (AttackSkill)originSkill;
                expectation += DamageSystem.Expect(character, o, originAttackSkill.damage, originAttackSkill.hit, hoverRange == 0, finalDamageBuff == null ? 0 : finalDamageBuff.Factor);
                finalRate = DamageSystem.HitRateSystem(character, o, (skillRate * hit + originAttackSkill.skillRate * originAttackSkill.hit) / (hit + originAttackSkill.hit)).ToString();
            }

            string roleName = o.GetComponent<CharacterStatus>().roleCName.Replace(" ", "");
            string roleIdentity = o.GetComponent<CharacterStatus>().IsEnemy(character.GetComponent<CharacterStatus>()) ? "" : o.GetComponent<CharacterStatus>().identity;
            string roleState = o.GetComponent<Unit>().UnitEnd ? "结束" : "待机";
            string hp = currentHp;
            string mp = currentMp;
            string hpMax = a.Find(d => d.eName == "hp").valueMax.ToString();
            string mpMax = a.Find(d => d.eName == "mp").valueMax.ToString();
            string effectTitle = expectation > 0 ? "损伤" : "恢复";
            string effectInfo = Mathf.Abs(expectation).ToString();
            string rateInfo = finalRate + "%";
            string atkInfo = atk;
            string defInfo = def;
            string dexInfo = dex;

            expectationList.Add(new KeyValuePair<CharacterStatus, string[]>(o.GetComponent<CharacterStatus>(),
                new string[13] {
                    roleName,
                    roleIdentity,
                    roleState,
                    hp,
                    mp,
                    hpMax,
                    mpMax,
                    effectInfo,
                    rateInfo,
                    atkInfo,
                    defInfo,
                    dexInfo,
                    effectTitle
                }));
        }

        RefreshExpectionData(0);
        pointer.transform.SetParent(expectationList[pointerIterator].Key.transform);
        pointer.transform.localPosition = expectationList[pointerIterator].Key.arrowPosition;
        pointer.SetActive(true);
        expectationUI.SetActive(true);
    }

    void RefreshExpectionData(int iter)
    {
        expectationUI.transform.Find("Content").Find("RoleName").GetComponent<Text>().text = expectationList[iter].Value[0];
        expectationUI.transform.Find("Content").Find("RoleIdentity").GetComponent<Text>().text = expectationList[iter].Value[1];
        expectationUI.transform.Find("Content").Find("RoleState").GetComponent<Text>().text = expectationList[iter].Value[2];

        expectationUI.transform.Find("Content").Find("Info").GetComponent<Text>().text = expectationList[iter].Value[3] + "\n" + expectationList[iter].Value[4];

        expectationUI.transform.Find("Content").Find("Health").GetComponent<Slider>().maxValue = int.Parse(expectationList[iter].Value[5]);
        expectationUI.transform.Find("Content").Find("Health").GetComponent<Slider>().value = int.Parse(expectationList[iter].Value[3]);
        expectationUI.transform.Find("Content").Find("Chakra").GetComponent<Slider>().maxValue = int.Parse(expectationList[iter].Value[6]);
        expectationUI.transform.Find("Content").Find("Chakra").GetComponent<Slider>().value = int.Parse(expectationList[iter].Value[4]);

        expectationUI.transform.Find("Content").Find("EffectTitle").GetComponent<Text>().text = expectationList[iter].Value[12];
        expectationUI.transform.Find("Content").Find("EffectInfo").GetComponent<Text>().text = expectationList[iter].Value[7];
        expectationUI.transform.Find("Content").Find("RateInfo").GetComponent<Text>().text = expectationList[iter].Value[8];
        expectationUI.transform.Find("Content").Find("AtkInfo").GetComponent<Text>().text = expectationList[iter].Value[9];
        expectationUI.transform.Find("Content").Find("DefInfo").GetComponent<Text>().text = expectationList[iter].Value[10];
        expectationUI.transform.Find("Content").Find("DexInfo").GetComponent<Text>().text = expectationList[iter].Value[11];
    }

    protected override void ShowConfirm()
    {
        base.ShowConfirm();

        ShowUI();
    }

    public override List<string> LogSkillEffect()
    {
        string title = "攻击力";
        string info = damage + "×" + hit;
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
                    DebugLogPanel.GetInstance().Log("速击！" + "（" + character.GetComponent<CharacterStatus>().roleCName + " -> " + o.GetComponent<CharacterStatus>().roleCName + "）");
                    hit++;
                }

                //每Hit
                for (int i = 0; i < hit; i++)
                {
                    int d;
                    var doNextHit = DamageSystem.ApplyDamage(character, o, this, comboSkill == null && hoverRange == 0 || comboSkill != null && comboSkill.hoverRange == 0, finalDamageBuff == null ? 0 : finalDamageBuff.Factor, out d);
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
                            if (damage > 0)
                            {
                                if (damageList[j] > 0)
                                {
                                    //受击动作
                                    HitEffect(o);

                                    UIManager.GetInstance().FlyNum(o.GetComponent<CharacterStatus>().arrowPosition / 2 + o.position, damageList[j].ToString(), Color.white, true);
                                }
                                else if (damageList[j] == 0)
                                {
                                    UIManager.GetInstance().FlyNum(o.GetComponent<CharacterStatus>().arrowPosition / 2 + o.position, damageList[j].ToString(), Color.white, false);
                                }
                                else if (damageList[j] < 0)
                                {
                                    //受击动作
                                    HitEffect(o);
                                }
                            }
                            else
                                UIManager.GetInstance().FlyNum(o.GetComponent<CharacterStatus>().arrowPosition / 2 + o.position, Mathf.Abs(damageList[j]).ToString(), UIManager.hpColor, false);
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
                    var buff = o.GetComponent<CharacterStatus>().Buffs.Find(b => b.GetType() == typeof(DodgeBuff));
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
                animator.SetInteger("Skill", animID);
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
        if (expectationUI)
            UnityEngine.Object.Destroy(expectationUI);
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
                if (u.GetComponent<CharacterStatus>())
                {
                    if (character.GetComponent<CharacterStatus>().IsEnemy(u.GetComponent<CharacterStatus>()))
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
