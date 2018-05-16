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
    
	protected override void InitSkill()
	{
		base.InitSkill ();
		Camera.main.GetComponent<RTSCamera> ().FollowTarget (character.position);
		//跳过去并停在被攻击对象的前一格
		RoundManager.GetInstance ().Invoke (() => {
			if (!animator.applyRootMotion) {
				animator.applyRootMotion = true;//√上applyRootMotion，未测试是否存在bug，也暂时未发现使用该判定条件目前有何问题，只是单纯觉得可能不够严谨（心理作用）？
			}
			RoundManager.GetInstance ().Invoke (() => {
				float time = 0.8f;
				float distance = (focus - character.position).magnitude;

				//Debug.Log("距离是："+distance);
				if (!animator.isMatchingTarget) {
					//distance的数值会有波动，发现其实也不用Mathf.RoundToInt（）取整并比较1了（强迫症请无视），直接用>0.95f比较也可以用，暂未发现有不妥的bug出现
					if (distance > 0.95f) {  
						MatchPoint (focus - character.forward);
					}
				}
				if (distance <= 0.45f) {
					animator.InterruptMatchTarget (false);
					character.Translate (focus - character.position, Space.World);
					animator.applyRootMotion = false;
				}
				RoundManager.GetInstance ().Invoke (() => {
					//Effect ();//我的施工现场，调整效果的时机的地方，暂时留着空位，无需要就删了吧
					//GetHit ();
					RoundManager.GetInstance ().Invoke (() => {
						RoundManager.GetInstance ().Invoke (() => {
							complete = true;
						}, 0.5f);
					}, hit * 0.2f);
				}, time);
			}, 0.1f);//我比一般的其他技能加快了这里，从1f—>0.1f，因为延时长了，animator.InterruptMatchTarget (false)会出问题
		}, 0.1f);
	}

	private void MatchPoint(Vector3 destination)
	{
		//MatchTargetWeightMask中的positionXYZWeight应该是localPosition，所以forward即可，因为人物会转向。
		animator.MatchTarget(destination, character.rotation, AvatarTarget.Root, new MatchTargetWeightMask(Vector3.forward, 0f), 0.33f, 0.55f);
	}
}
