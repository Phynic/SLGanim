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

        Camera.main.GetComponent<RTSCamera>().FollowTarget(character.position);
        
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

        RoundManager.GetInstance().Invoke(() => {
            bcAnimator.speed = 1;
        }, 0.5f);

        RoundManager.GetInstance().Invoke(() => {
            var bc = fx.Spawn("BeastCombo", character, 5);
            bc.GetChild(0).GetComponent<Animation>().Play();
            float time = 0.4f;
            var t = bc.DOMove(focus - bc.forward * 1.2f, time);
            t.SetEase(fx.curve1);

        }, 0.5f + 0.5f);
        //base.Effect();
    }

    public override void GetHit()
    {
        
    }
}
