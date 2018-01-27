using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//倍化术
public class Intumescence : UnitSkill
{
    int factor;
    FinalDamageBuff buff;
    public override bool Init(Transform character)
    {
        //SetLevel在base.Init中执行，所以先执行再添加Buff。
        if (base.Init(character))
        {
            buff = new FinalDamageBuff(-1, factor);
            character.GetComponent<CharacterStatus>().Buffs.Add(buff);
            return true;
        }
        else
        {
            return false;
        }
    }

    public override void SetLevel(int level)
    {
        factor = 20 + level * 10;
    }

    public override List<string> LogSkillEffect()
    {
        string title = "最终伤害";
        string info = "+ " + factor + "%";

        List<string> s = new List<string>
        {
            title,
            info
        };

        return s;
    }
    
    public override void Effect()
    {
        base.Effect();
        DebugLogPanel.GetInstance().Log("最终伤害 + " + factor.ToString() + "%");

        var go = Resources.Load("Prefabs/Character/MeatBulletTank");
        //var ass = AssetBundle.LoadFromFile(Application.dataPath + "/Resources/Prefabs/Character/MeatBulletTank");
        
        character.Find("Render").gameObject.SetActive(false);
        var meatBulletTank = GameObject.Instantiate(go, character.position, character.rotation);
    }

    protected override void ResetSelf()
    {
        if (character.GetComponent<CharacterStatus>().Buffs.Contains(buff))
        {
            buff.Undo(character);
        }
    }

    public override void Reset()
    {
        ResetSelf();
        base.Reset();
    }
}
