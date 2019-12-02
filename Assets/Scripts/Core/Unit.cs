using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

/// <summary>
/// Base class for all units in the game.
/// </summary>
public abstract class Unit : Touchable
{
    //通过OnUnitEnd()改变
    public bool UnitEnd { get; private set; }

    //[HideInInspector]
    public List<SLG.Attribute> attributes = new List<SLG.Attribute>();

    public event UnityAction<Unit> UnitClicked;
    public event UnityAction<Unit> UnitSelected;
    public event EventHandler UnitDeselected;

#if (UNITY_STANDALONE || UNITY_EDITOR)
    public event EventHandler UnitHighlighted;
    public event EventHandler UnitDehighlighted;
#endif

    public event EventHandler UnitDestroyed;
    public event EventHandler UnitEnded;

    //[HideInInspector]
    public Renderer[] rend;

    
    public List<IBuff> Buffs { get; private set; }

    public Stack<Skill> action = new Stack<Skill>();

    /// <summary>
    /// Indicates the player that the unit belongs to. Should correspoond with PlayerNumber variable on Player script.
    /// </summary>
    public int playerNumber;

    /// <summary>
    /// Method is called at the start of each turn.
    /// </summary>
    public virtual void OnRoundStart()
    {
        if (Buffs.Find(b => b is BanBuff) == null)
            UnitEnd = false;
    }

    /// <summary>
    /// Method is called at the end of each turn.
    /// </summary>
    public virtual void OnRoundEnd()
    {

    }

    //OnTurnStart在不管敌方还是我方Turn开始的时候都会调用。
    public virtual void OnTurnStart()
    {
        if (Buffs.Find(b => b is BanBuff) == null)
            Gray(false);
    }

    public void Gray(bool on)
    {
        if (on)
        {
            for (int i = 0; i < rend.Length; i++)
            {
                if (rend[i].material.shader.name == "Shader/ToonOutLine")
                    rend[i].material.SetFloat("_Gray", 1f);
            }
        }
        else
        {
            for (int i = 0; i < rend.Length; i++)
            {
                if (rend[i].material.shader.name == "Shader/ToonOutLine")
                    rend[i].material.SetFloat("_Gray", 0f);
            }
        }

    }

    /// <summary>
    /// 把buff结算挪至这里，仍需检验与OnTurnStart的区别以及合理性。
    /// </summary>
    public virtual void OnTurnEnd()
    {
        //应该在TurnStart，才能保证在轮到自己的时候，buff已经做过结算。buff类内的Duration == 0表示持续至下一个Turn开始（敌方的）。
        Buffs.FindAll(b => b.Duration == 0).ForEach(b => { b.Undo(transform); });
        Buffs.RemoveAll(b => b.Duration == 0);
        Buffs.ForEach(b => { b.Duration--; });

        if (Buffs.Find(b => b is BanBuff) == null)
        {
            Gray(false);
        }
    }

    public virtual void OnUnitEnd()
    {
        UnitEnd = true;
        if (UnitEnded != null)
            UnitEnded.Invoke(this, null);
        Gray(true);
        action.Clear();
    }


#if (UNITY_STANDALONE || UNITY_EDITOR)
    protected virtual void OnMouseDown()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (UnitClicked != null)
                UnitClicked.Invoke(this);
        }
    }

    protected virtual void OnMouseEnter()
    {
        if (UnitHighlighted != null)
            UnitHighlighted.Invoke(this, new EventArgs());
    }

    protected virtual void OnMouseExit()
    {
        if (UnitDehighlighted != null)
            UnitDehighlighted.Invoke(this, new EventArgs());
    }
#endif

#if (!UNITY_EDITOR && (UNITY_IOS || UNITY_ANDROID))

    public override void OnTouchUp()
    {
        if (marked)
        {
            if (UnitClicked != null)
                UnitClicked.Invoke(this, new EventArgs());
        }
        base.OnTouchUp();
    }
    
#endif


    /// <summary>
    /// Method is called when unit is selected.
    /// </summary>
    public virtual void OnUnitSelected()
    {
        if (UnitSelected != null)
            UnitSelected.Invoke(this);
    }
    /// <summary>
    /// Method is called when unit is deselected.
    /// </summary>
    public virtual void OnUnitDeselected()
    {
        if (UnitDeselected != null)
            UnitDeselected.Invoke(this, new EventArgs());
        for (int i = 0; i < rend.Length; i++)
        {
            if (rend[i].material.shader.name == "Shader/ToonOutLine")
                rend[i].material.SetColor("_OutLineColor", new Color(0, 0, 0, 1));
        }
    }

    private void Start()
    {
        //等待mesh合并
        Utils_Coroutine.GetInstance().Invoke(() =>
        {
            rend = GetComponentsInChildren<Renderer>();
            UnitEnd = false;
        }, 0.1f);
    }






    /// <summary>
    /// CharacterStatus
    /// </summary>
    public int CharacterInfoID { get; private set; }
    public CharacterData CharacterData { get; private set; }
    public string roleEName;        //人物名称。      
    public string roleCName;
    public CharacterIdentity characterIdentity = CharacterIdentity.noumenon;
    public string identity = "本体";
    public string state;
    public int killExp = 100;       //击杀经验
    public int bonusExp = 0;        //人头奖励
    public enum CharacterIdentity
    {
        noumenon,               //本体
        clone,                  //分身
        advanceClone,           //高级分身
        beastClone,             //赤丸
        obstacle                //障碍
    }

    //战斗场景中只读使用。
    /// <summary>
    /// <uniqueID, ItemRecord>
    /// </summary>
    public Dictionary<int, ItemRecord> items = new Dictionary<int, ItemRecord>();
    public Dictionary<int, int> skills; //忍术列表<忍术ID，技能等级>

    public Vector3 arrowPosition = new Vector3(0, 1.1f, 0);
    public List<Skill> firstAction;                 //第一次行动列表
    public List<Skill> secondAction;                //第二次行动列表

    public void Init(int characterInfoID)
    {
        CharacterInfoID = characterInfoID;
        var characterInfo = CharacterInfoDictionary.GetParam(characterInfoID);
        roleEName = characterInfo.roleEName;
        roleCName = characterInfo.roleCName;
        arrowPosition = characterInfo.arrowPosition;
        Init();
    }

    public void Init()
    {
        Buffs = new List<IBuff>();

        var characterData = Global.characterDataList.Find(d => d.characterInfoID == CharacterInfoID);

        //序列化和反序列化进行深度复制。
        MemoryStream stream = new MemoryStream();
        BinaryFormatter formatter = new BinaryFormatter();
        formatter.Serialize(stream, characterData.attributes);
        stream.Position = 0;
        attributes = formatter.Deserialize(stream) as List<SLG.Attribute>;
    }

    public void Init(CharacterData characterData)
    {
        CharacterData = characterData;
        CharacterInfoID = characterData.characterInfoID;
        var characterInfo = CharacterInfoDictionary.GetParam(CharacterInfoID);
        roleEName = characterInfo.roleEName;
        roleCName = characterInfo.roleCName;
        arrowPosition = characterInfo.arrowPosition;

        Buffs = new List<IBuff>();

        //序列化和反序列化进行深度复制。
        MemoryStream stream = new MemoryStream();
        BinaryFormatter formatter = new BinaryFormatter();
        formatter.Serialize(stream, characterData.attributes);
        stream.Position = 0;
        attributes = formatter.Deserialize(stream) as List<SLG.Attribute>;

        SetIdentity(characterInfo.initialIdentity);
    }

    public void Init(Unit noumenon)
    {
        Buffs = new List<IBuff>();

        MemoryStream stream = new MemoryStream();
        BinaryFormatter formatter = new BinaryFormatter();
        formatter.Serialize(stream, noumenon.GetComponent<Unit>().attributes);
        stream.Position = 0;
        attributes = formatter.Deserialize(stream) as List<SLG.Attribute>;

    }

    public bool IsEnemy(Unit unit)
    {
        return playerNumber != unit.playerNumber;
    }

    public void SetNoumenon()
    {
        characterIdentity = CharacterIdentity.noumenon;
        identity = "本体";
        rend = GetComponentsInChildren<Renderer>();
        firstAction = new List<Skill>();
        secondAction = new List<Skill>();
        skills = new Dictionary<int, int>();

        firstAction.Add(SkillManager.GetInstance().skillList.Find(s => s.EName == "Move"));
        firstAction.Add(SkillManager.GetInstance().skillList.Find(s => s.EName == "SkillOrToolList"));
        firstAction.Add(SkillManager.GetInstance().skillList.Find(s => s.EName == "RestoreChakra"));
        firstAction.Add(SkillManager.GetInstance().skillList.Find(s => s.EName == "Rest"));
        firstAction.Add(SkillManager.GetInstance().skillList.Find(s => s.EName == "EndRound"));

        secondAction.Add(SkillManager.GetInstance().skillList.Find(s => s.EName == "SkillOrToolList"));
        secondAction.Add(SkillManager.GetInstance().skillList.Find(s => s.EName == "EndRound"));


        var characterData = Global.characterDataList.Find(d => d.characterInfoID == CharacterInfoID);



        foreach (var data in characterData.skills)
        {
            skills.Add(data.skillInfoID, data.level);
        }

        foreach (var item in characterData.items)
        {
            items.Add(item.uniqueID, item);
        }
    }

    public void SetIdentity(int identityID)
    {
        var identityInfo = IdentityInfoDictionary.GetParam(identityID);

        firstAction = new List<Skill>();
        secondAction = new List<Skill>();

        //第一行动
        foreach (var skillID in identityInfo.firstAction)
        {
            firstAction.Add(SkillManager.GetInstance().skillList.Find(s => s.SkillInfoID == skillID));
        }

        //第二行动
        foreach (var skillID in identityInfo.secondAction)
        {
            secondAction.Add(SkillManager.GetInstance().skillList.Find(s => s.SkillInfoID == skillID));
        }
    }

    public void SetObstacle()
    {
        characterIdentity = CharacterIdentity.obstacle;
        identity = "障碍";
        rend = GetComponentsInChildren<Renderer>();
        firstAction = new List<Skill>();
        secondAction = new List<Skill>();
        skills = new Dictionary<int, int>();
    }

    public void SetClone(Unit noumenon)
    {
        Init(noumenon);
        identity = "分身";

        characterIdentity = CharacterIdentity.clone;
        firstAction = new List<Skill>();
        secondAction = new List<Skill>();
        skills = new Dictionary<int, int>();

        firstAction.Add(SkillManager.GetInstance().skillList.Find(s => s.EName == "Move"));
        firstAction.Add(SkillManager.GetInstance().skillList.Find(s => s.EName == "EndRound"));

    }

    public void SetAdvancedClone(Unit noumenon)
    {
        Init(noumenon);

        characterIdentity = CharacterIdentity.advanceClone;
        firstAction = new List<Skill>();
        secondAction = new List<Skill>();
        skills = new Dictionary<int, int>();

        firstAction.Add(SkillManager.GetInstance().skillList.Find(s => s.EName == "Move"));
        firstAction.Add(SkillManager.GetInstance().skillList.Find(s => s.EName == "SkillOrToolList"));
        firstAction.Add(SkillManager.GetInstance().skillList.Find(s => s.EName == "EndRound"));

        secondAction.Add(SkillManager.GetInstance().skillList.Find(s => s.EName == "SkillOrToolList"));
        secondAction.Add(SkillManager.GetInstance().skillList.Find(s => s.EName == "EndRound"));

        var characterRecord = Global.characterDataList.Find(d => d.characterInfoID == CharacterInfoID);

        foreach (var data in characterRecord.skills)
        {
            skills.Add(data.skillInfoID, data.level);
        }
    }

    public void SetBeastClone(Unit noumenon)
    {
        Init(noumenon);

        characterIdentity = CharacterIdentity.beastClone;
        firstAction = new List<Skill>();
        secondAction = new List<Skill>();
        skills = new Dictionary<int, int>();

        firstAction.Add(SkillManager.GetInstance().skillList.Find(s => s.EName == "Move"));
        firstAction.Add(SkillManager.GetInstance().skillList.Find(s => s.EName == "SkillOrToolList"));
        firstAction.Add(SkillManager.GetInstance().skillList.Find(s => s.EName == "EndRound"));

        secondAction.Add(SkillManager.GetInstance().skillList.Find(s => s.EName == "SkillOrToolList"));
        secondAction.Add(SkillManager.GetInstance().skillList.Find(s => s.EName == "EndRound"));

        var characterData = Global.characterDataList.Find(d => d.characterInfoID == CharacterInfoID);

        foreach (var data in characterData.skills)
        {
            skills.Add(data.skillInfoID, data.level);
        }
    }


    public virtual void OnDestroyed(object sender, EventArgs e)
    {
        Debug.Log("由于" + ((Unit)sender).transform.name + "受伤，" + transform.name + "退出战斗");
        RoundManager.GetInstance().Units.Remove(this);
        Destroy(gameObject);
    }

    /// <summary>
    /// Method is called when units HP drops below 1.
    /// </summary>
    public void OnDestroyed()
    {
        RoundManager.GetInstance().Units.Remove(this);
        if (UnitDestroyed != null)
            UnitDestroyed.Invoke(this, null);
        Debug.Log(transform.name + " is Dead!");

        switch (characterIdentity)
        {
            case CharacterIdentity.noumenon:
                GetComponent<Animator>().SetBool("Dead", true);
                Destroy(gameObject, 4f);
                break;
            case CharacterIdentity.clone:
                Destroy(gameObject);
                break;
            case CharacterIdentity.advanceClone:
                Destroy(gameObject);
                break;
            case CharacterIdentity.beastClone:
                GetComponent<Animator>().SetBool("Dead", true);
                Destroy(gameObject, 4f);
                break;
            default:
                break;
        }
    }
}
