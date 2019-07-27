using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BeastCombo : AttackSkill {
	FXManager fx;
    Transform partner;
	
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
        base.SetLevel(level);
        damage = skillInfo.factor;
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
        
        partner.GetComponent<Animator>().SetInteger("Skill", skillInfo.animID);
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
        
        Utils_Coroutine.GetInstance().Invoke(() => {
            bcAnimator.speed = 1;
        }, 0.5f);

        Utils_Coroutine.GetInstance().Invoke(() => {
            var bc = fx.Spawn("BeastCombo", character, 1.5f);
            bc.GetChild(0).GetComponent<Animation>().Play();
            float time = 0.4f;

            var t = bc.DOMove(focus - bc.forward * 1.2f, time);
            t.onComplete = () =>
            {
                base.Effect();
                base.GetHit();
                //Camera.main.GetComponent<RTSCamera>().FollowTarget(focus);
            };
            t.SetEase(fx.curve1);

            Utils_Coroutine.GetInstance().Invoke(() => {
                var smoke = fx.Spawn("Smoke", bc.position, 4f);
                smoke.localScale = new Vector3(2, 2, 2);
                Utils_Coroutine.GetInstance().Invoke(() => {
                    smoke.localScale = new Vector3(1, 1, 1);
                }, 4f);
                Utils_Coroutine.GetInstance().Invoke(() => {
                    //Camera.main.GetComponent<RTSCamera>().FollowTarget(character.position);
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
        //覆盖掉，防止defaultSkill动作调用。
    }
}
