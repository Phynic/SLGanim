using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeastCombo : AttackSkill {

    public override bool Filter(Skill sender)
    {
        int i = 0;
        var list = Detect.DetectObjects(1, sender.character.position);
        foreach (var l in list)
        {
            foreach (var u in l)
            {
                if (u.GetComponent<CharacterStatus>())
                {
                    if (!sender.character.GetComponent<CharacterStatus>().IsEnemy(u.GetComponent<CharacterStatus>()))
                    {
                        if(u.GetComponent<CharacterStatus>().roleEName == "Kiba")
                        {
                            i++;
                        }
                    }
                }
            }
        }
        if(i == 2)
        {
            return base.Filter(sender);
        }
        return false;
    }

    public override void SetLevel(int level)
    {
        damageFactor = 20 + level * 5;
    }

    protected override void Confirm()
    {
        base.Confirm();
        //animator.applyRootMotion = true;
        //Debug.Log(focus);
        //animator.MatchTarget(focus, character.rotation, AvatarTarget.Root, new MatchTargetWeightMask(Vector3.forward, 0f), 0.21f, 0.37f);
    }
}
