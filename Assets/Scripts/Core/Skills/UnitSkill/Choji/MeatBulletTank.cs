using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeatBulletTank : AttackSkill {
    float speed = 3f;

    GameObject meatBulletTankFX = null;

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
        var meatBulletTank = GameObject.Instantiate(go, character.position, character.rotation, character) as GameObject;

        meatBulletTankFX = FXManager.GetInstance().Spawn("MeatBulletTank", meatBulletTank.transform, 10f).gameObject;
    }
    
    protected override bool ApplyEffects()
    {
        if (meatBulletTankFX && (character.position - focus).magnitude > 0.7)
            character.position += character.forward * speed * Time.deltaTime;
        return base.ApplyEffects();
    }
}
