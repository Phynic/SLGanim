using UnityEngine;
using DG.Tweening;

public class MeatBulletTank : AttackSkill {
    FXManager fx;
    
    public override void SetLevel(int level)
    {
        base.SetLevel(level);
        skillRange = factor;
        aliesObstruct = true;
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
        animator.speed = 0;
        fx = FXManager.GetInstance();
        var render = character.Find("Render").gameObject;
        render.SetActive(false);

        Camera.main.GetComponent<RTSCamera>().FollowTarget(character.position);
        fx.Spawn("Intumescence", character, 3f);
        var mbtFXBody = fx.Spawn("MeatBulletTankBody", character, 10f);
        
        Utils_Coroutine.GetInstance().Invoke(() => {
            
            var mbtFX = fx.Spawn("MeatBulletTank", character, 10f);

            mbtFXBody.SetParent(mbtFX);
            //肉弹运动时间。
            float time = 0.8f;
            var t = mbtFX.DOMove(focus - mbtFX.forward, time);
            t.SetEase(fx.curve0);
            Camera.main.GetComponent<RTSCamera>().FollowTarget(focus - mbtFX.forward);

            Utils_Coroutine.GetInstance().Invoke(() => {
                Effect();
                GetHit();
                
                Utils_Coroutine.GetInstance().Invoke(() => {
                    mbtFX.gameObject.SetActive(false);
                    var temp = character.position;
                    character.position = mbtFX.position;
                    render.SetActive(true);
                    animator.speed = 1;
                    var tween = character.DOMove(temp, 0.5f);
                    fx.Spawn("Smoke", character.position, 4f);
                    tween.SetEase(Ease.OutQuint);

                    Utils_Coroutine.GetInstance().Invoke(() => {
                        complete = true;
                    }, 0.5f);

                }, hit * 0.2f);
            }, time);

        }, 1f);
    }

}
