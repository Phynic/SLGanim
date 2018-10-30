using UnityEngine;
using DG.Tweening;

public class MeatBulletTank : AttackSkill {
    FXManager fx;
    public override void SetLevel(int level)
    {
        skillRange = skillRange + (level - 1) * (int)growFactor;
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

    public override void GetHit()
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
                }, 0.33f * i);
            }
        }
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
        
        GameController.GetInstance().Invoke(() => {
            
            var mbtFX = fx.Spawn("MeatBulletTank", character, 10f);

            mbtFXBody.SetParent(mbtFX);
            //肉弹运动时间。
            float time = 0.8f;
            var t = mbtFX.DOMove(focus - mbtFX.forward, time);
            t.SetEase(fx.curve0);
            Camera.main.GetComponent<RTSCamera>().FollowTarget(focus - mbtFX.forward);

            GameController.GetInstance().Invoke(() => {
                Effect();
                GetHit();
                
                GameController.GetInstance().Invoke(() => {
                    mbtFX.gameObject.SetActive(false);
                    var temp = character.position;
                    character.position = mbtFX.position;
                    render.SetActive(true);
                    animator.speed = 1;
                    var tween = character.DOMove(temp, 0.5f);
                    fx.Spawn("Smoke", character.position, 4f);
                    tween.SetEase(Ease.OutQuint);

                    GameController.GetInstance().Invoke(() => {
                        complete = true;
                    }, 0.5f);

                }, hit * 0.2f);
            }, time);

        }, 1f);
    }

}
