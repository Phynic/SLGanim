using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// Beast attack.cs的使用方法：
/// 1、请将xml里这个技能的animID修改，我改了4；
/// 2、在kiba的<skill>Animator中添加一个新动画，即，把裁剪过且添加了GetHit帧事件的kibaBeastAttack.anim拉进去，条件为 Skill Equals 4
/// </summary>

public class BeastAttack : AttackSkill {
    FXManager fx;
    public override void SetLevel(int level)
    {
        damageFactor = damageFactor + (level - 1) * 5;
		aliesObstruct = true;//友军遮挡
    }
    
	protected override void InitSkill()
	{
		base.InitSkill ();
        fx = FXManager.GetInstance();
        var destination = focus - (focus - character.position).normalized;
        Camera.main.GetComponent<RTSCamera> ().FollowTarget (character.position);
        //跳过去并停在被攻击对象的前一格

        var t = character.DOMove(destination, animator.GetCurrentAnimatorStateInfo(0).length * 0.22f);
        t.SetEase(fx.curve1);
        
        RoundManager.GetInstance().Invoke(() => {
            character.position = destination;

        }, 2f);
    }
    
}
