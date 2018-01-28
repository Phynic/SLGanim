using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeatBulletTank : AttackSkill {
    public override void SetLevel(int level)
    {
        damageFactor = 35 + level * 5;
    }
    public override bool Filter(Skill sender)
    {
        if (sender.EName == "Intumescence")
        {
            return base.Filter(sender);
        }
        return false;
    }

    protected override void InitSkill()
    {
        base.InitSkill();
        character.Find("Render").gameObject.SetActive(false);
        
        FXManager.GetInstance().Spawn("Intumescence", character, 3f);


        var go = Resources.Load("Prefabs/Character/MeatBulletTank");
        var meatBulletTank = GameObject.Instantiate(go, character.position, character.rotation);
    }
}
