using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeastAttack : AttackSkill {

    public override void SetLevel(int level)
    {
        damageFactor = damageFactor + (level - 1) * 5;
    }

    public override void Confirm()
    {
        base.Confirm();
        animator.applyRootMotion = true;
        //Debug.Log(focus);
        animator.MatchTarget(focus, character.rotation, AvatarTarget.Root, new MatchTargetWeightMask(Vector3.forward, 0f), 0.21f, 0.37f);
    }

    public override void Effect()
    {
        base.Effect();
        RoundManager.GetInstance().Invoke(() => { render.SetActive(false); }, 1.35f);
        RoundManager.GetInstance().Invoke(() => { render.SetActive(true); }, 1.5f);
    }
}
