using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeatBulletTank : AttackSkill {
    //float speed = 3f;   

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
        FXManager.GetInstance().Spawn("MeatBulletTankBody", character, 10f);
        FXManager.GetInstance().Spawn("MeatBulletTank", character, 10f);
        
        //肉弹战车特效部分有自动延迟，中间有一帧SetActive，后续去掉延迟并修正。
        
    }
    
    protected override bool ApplyEffects()
    {
        //if (meatBulletTankFX && (character.position - focus).magnitude > 0.7)
        //    character.position += character.forward * speed * Time.deltaTime;
        return base.ApplyEffects();
    }
}
