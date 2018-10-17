using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BeastCombo : AttackSkill {
	FXManager fx;
    Transform partner;
	/// <summary>
	/// 使用说明：1、gamedata.xml 应该要修改，这个我不会改，我修改了<skillRange><hoverRange>，但没看到效果，所以暂时搁置了，先改脚本
	/// 		  2、FX Manager中Curve1，曲线为直线，右端点为（0.5，1.5）
	/// 		  3、待后续讨论添加
	/// 未完成内容：1、直线攻击范围，技能覆盖范围...
	///		2、特效大小调整
	///		3、效果细调
	/// </summary>
    public override bool Filter(Skill sender)
    {
        int i = 0;
        var list = Detect.DetectObjects(Range.CreateRange(1, sender.character.position));
		//牙通牙判断,牙或赤丸是否在相邻1格内
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
		//牙判断，i+1，赤丸判断，i+1，所以i == 2
        if(i == 2)
        {
            return base.Filter(sender);
        }
        return false;
    }

    public override void SetLevel(int level)
    {
        damageFactor = damageFactor + (level - 1) * 5;
        aliesObstruct = true;
    }
    
	protected override void InitSkill()
	{
		base.InitSkill();
		fx = FXManager.GetInstance();

        //获取partner
        var list = Detect.DetectObjects(Range.CreateRange(1, character.position));
        //牙通牙判断,牙或赤丸是否在相邻1格内
        foreach (var l in list)
        {
            foreach (var u in l)
            {
                if (u.GetComponent<CharacterStatus>())
                {
                    if (!character.GetComponent<CharacterStatus>().IsEnemy(u.GetComponent<CharacterStatus>()))
                    {
                        if (u.GetComponent<CharacterStatus>().roleEName == "Kiba")
                        {
                            if (u != character)
                                partner = u;
                        }
                    }
                }
            }
        }

        Camera.main.GetComponent<RTSCamera>().FollowTarget(focus);
        
        partner.GetComponent<Animator>().SetInteger("Skill", animID);
        
		

	}

    public override void Effect()
    {
        fx.Spawn("Smoke", partner.position, 4f);
        fx.Spawn("Smoke", character.position, 4f);
        animator.speed = 0;
        partner.GetComponent<Animator>().speed = 0;
        partner.Find("Render").gameObject.SetActive(false);
        render.gameObject.SetActive(false);

        var bcBody = fx.Spawn("BeastComboBody", character, 1f);
        var bcAnimator = bcBody.GetComponent<Animator>();
        bcAnimator.speed = 0;
        
        GameController.GetInstance().Invoke(() => {
            bcAnimator.speed = 1;
        }, 0.5f);

        GameController.GetInstance().Invoke(() => {
            var bc = fx.Spawn("BeastCombo", character, 1.5f);
            bc.GetChild(0).GetComponent<Animation>().Play();
            float time = 0.4f;

            var t = bc.DOMove(focus - bc.forward * 1.2f, time);
            t.onComplete = () =>
            {
                base.Effect();
                GetHitSelf();
                //Camera.main.GetComponent<RTSCamera>().FollowTarget(focus);
            };
            t.SetEase(fx.curve1);

            GameController.GetInstance().Invoke(() => {
                var smoke = fx.Spawn("Smoke", bc.position, 4f);
                smoke.localScale = new Vector3(2, 2, 2);
                GameController.GetInstance().Invoke(() => {
                    smoke.localScale = new Vector3(1, 1, 1);
                }, 4f);
                GameController.GetInstance().Invoke(() => {
                    Camera.main.GetComponent<RTSCamera>().FollowTarget(character.position);
                    fx.Spawn("Smoke", partner.position, 4f);
                    fx.Spawn("Smoke", character.position, 4f);
                    partner.Find("Render").gameObject.SetActive(true);
                    render.gameObject.SetActive(true);
                    animator.speed = 1;
                    partner.GetComponent<Animator>().SetInteger("Skill", 0);
                    partner.GetComponent<Animator>().speed = 1;
                }, 0.5f);
            }, 1.5f);

        }, 0.5f + 0.5f);
        //base.Effect();
    }

    public override void GetHit()
    {
        //覆盖掉，防止调用。
    }

    public void GetHitSelf()
    {
        foreach (var o in other)
        {
            for (int i = 0; i < hit; i++)
            {
                GameController.GetInstance().Invoke(() => {
                    if (o)
                    {
                        if (o.GetComponent<Animator>())
                        {
                            FXManager.GetInstance().HitPointSpawn(o.GetComponent<Animator>().GetBoneTransform(HumanBodyBones.Chest).position, Quaternion.identity, null, 1);
                            o.GetComponent<Animator>().SetFloat("HitAngle", Vector3.SignedAngle(o.position - character.position, -o.forward, Vector3.up));
                            o.GetComponent<Animator>().Play("GetHit", 0, i == 0 ? 0 : 0.2f);
                        }
                        else
                        {
                            FXManager.GetInstance().HitPointSpawn(o.position + Vector3.up * 0.7f, Quaternion.identity, null, 1);
                        }
                    }
                }, 0.2f * i);
            }
        }
    }
}
