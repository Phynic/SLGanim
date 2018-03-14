using UnityEngine;
using DG.Tweening;

public class MeatBulletTank : AttackSkill {
    FXManager fx;
    public override void SetLevel(int level)
    {
        damageFactor = 35 + level * 5;
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
        fx = FXManager.GetInstance();
        var render = character.Find("Render").gameObject;
        render.SetActive(false);

        Camera.main.GetComponent<RTSCamera>().FollowTarget(character.position);
        fx.Spawn("Intumescence", character, 3f);
        var mbtFXBody = fx.Spawn("MeatBulletTankBody", character, 10f);
        
        RoundManager.GetInstance().Invoke(() => {
            
            var mbtFX = fx.Spawn("MeatBulletTank", character, 10f);

            mbtFXBody.SetParent(mbtFX);
            //肉弹运动时间。
            float time = 0.5f;
            var t = mbtFX.DOMove(focus - mbtFX.forward, time);
            t.SetEase(fx.curve0);
            Camera.main.GetComponent<RTSCamera>().FollowTarget(focus - mbtFX.forward);

            RoundManager.GetInstance().Invoke(() => {
                Effect();
                GetHit();
                
                RoundManager.GetInstance().Invoke(() => {
                    mbtFX.gameObject.SetActive(false);
                    var temp = character.position;
                    character.position = mbtFX.position;
                    render.SetActive(true);
                    animator.speed = 1;
                    var tween = character.DOMove(temp, 0.5f);
                    fx.Spawn("Smoke", character.position, 4f);
                    tween.SetEase(Ease.OutQuint);
                }, hit * 0.2f);
            }, time);

        }, 1f);
    }

}
