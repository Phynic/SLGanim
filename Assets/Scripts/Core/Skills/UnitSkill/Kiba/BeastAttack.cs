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
    public override void SetLevel(int level)
    {
        damageFactor = damageFactor + (level - 1) * 5;
		aliesObstruct = true;//友军遮挡
    }

    public override void Effect()
    {
        base.Effect();
    }

	protected override void InitSkill()
	{
		base.InitSkill();
		Camera.main.GetComponent<RTSCamera> ().FollowTarget (character.position);
		//跳过去并停在被攻击对象的前一格
		RoundManager.GetInstance ().Invoke (() => {
			if (!animator.applyRootMotion) {
				animator.applyRootMotion = true;
			}
			RoundManager.GetInstance ().Invoke (() => {
				RoundManager.GetInstance ().Invoke (() => {
					complete = true;
				}, 0.65f);//改了下delay的参数，使其在站起身后才出现ChooseDirection
			}, hit * 0.3f);//改了下delay的参数，使其在站起身后才出现ChooseDirection
		}, 0.1f);
	}

	private void MatchPoint(Vector3 destination)
	{
		//MatchTargetWeightMask中的positionXYZWeight应该是localPosition，所以forward即可，因为人物会转向。
		animator.MatchTarget(destination, character.rotation, AvatarTarget.Root, new MatchTargetWeightMask(Vector3.forward, 0f), 0.33f, 0.55f);
	}

	protected override bool ApplyEffects(){
		float distance = ((focus - character.forward) - character.position).magnitude;

		if (!animator.isMatchingTarget) {
			if (distance > 0.95f) {  
				MatchPoint (focus - character.forward);
			}
		}

		if (distance <= 0.35f)
		{
			animator.InterruptMatchTarget(false);
			character.Translate(((focus - character.forward) - character.position), Space.World);
			animator.applyRootMotion = false;
		}
		if (complete)
		{
			character.GetComponent<CharacterAction>().SetSkill("ChooseDirection");
			return true;
		}
		return false;
	}
}
