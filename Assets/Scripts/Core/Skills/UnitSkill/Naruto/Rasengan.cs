using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rasengan : AttackSkill {

    public override void SetLevel(int level)
    {
        base.SetLevel(level);
        hitInterval = 0.15f;
    }
    
    protected override void InitSkill()
    {
        base.InitSkill();
        var list = Detect.DetectObject<CharacterStatus>(character.position + character.right - character.forward);
        if(list.Count == 1)
        {
            var comboCS = list[0];
            if (comboCS != null && comboCS.roleEName == character.GetComponent<CharacterStatus>().roleEName)
            {
                comboCS.transform.forward = character.forward;
                comboCS.GetComponent<Animator>().SetInteger("Skill", 9);
                GameController.GetInstance().Invoke(() => { comboCS.GetComponent<Animator>().SetInteger("Skill", 0); }, 0.1f);
                damage *= 2;
            }
        }
        Camera.main.GetComponent<RTSCamera>().FollowTarget(character.position);
        GameController.GetInstance().Invoke(() => {
            FXManager.GetInstance().Spawn("Rasengan", character.GetComponent<Animator>().GetBoneTransform(HumanBodyBones.RightHand), 3.2f);
        },0.5f);
        
    }
}
