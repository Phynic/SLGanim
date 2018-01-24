using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ShadowSimulation : AttackSkill {

    public int duration;
    
    //影子动画字典
    Dictionary<Material, TweenInfo> tweenDic = new Dictionary<Material, TweenInfo>();

    class TweenInfo
    {
        public string valueName;
        public float tweenValue = 0f;
        public TweenInfo(string valueName)
        {
            this.valueName = valueName;
        }
    }


    public override void SetLevel(int level)
    {
        duration = level + 2;
    }
    
    public override void Effect()
    {
        //施加禁止buff
        foreach (var o in other)
        {
            var banBuff = new BanBuff(duration);
            o.GetComponent<Unit>().Buffs.Add(banBuff);
            banBuff.Apply(o);
        }
        var buff = new BanBuff(duration);
        character.GetComponent<Unit>().Buffs.Add(buff);
        buff.Apply(character);
        

        base.Effect();

        animator.speed = 0;

        
        CreatePoint(character);

        
        foreach (var o in other)
        {
            //侧边
            if ((int)Vector3.Angle((o.position - character.position).normalized, character.forward) % 90 != 0) 
            {
                //确定两边
                if (Vector3.SignedAngle((o.position - character.position).normalized, character.forward, character.up) > 0)
                {
                    RoundManager.GetInstance().Invoke(() => { CreateMesh(character.position, o.position); }, 1f);
                    RoundManager.GetInstance().Invoke(() => { CreatePoint(o); }, 2f);
                }
                else
                {
                    RoundManager.GetInstance().Invoke(() => { CreateMesh(character.position, o.position); }, 1.5f);
                    RoundManager.GetInstance().Invoke(() => { CreatePoint(o); }, 2.5f);
                }
            }
            else
            {
                CreateLine(character.position, o.position);
                RoundManager.GetInstance().Invoke(() => { CreatePoint(o); }, 1f);
            }
        }
        
    }

    public override void GetHit()
    {
        
    }
    
    public override bool OnUpdate(Transform character)
    {
        foreach(var t in tweenDic)
        {
            ValueSync(t.Key, t.Value.valueName, t.Value.tweenValue);
        }

        return base.OnUpdate(character);
    }

    public override List<string> LogSkillEffect()
    {
        string title = "";
        string info = "";
        string durationInfo = duration.ToString();
        List<string> s = new List<string>
        {
            title,
            info,
            durationInfo
        };
        return s;
    }
    
    void CreateTween(Material m)
    {
        DOTween.To(() => tweenDic[m].tweenValue, x => tweenDic[m].tweenValue = x, 1f, 1f);
    }

    void ValueSync(Material m, string valueName, float tweenValue)
    {
        m.SetFloat(valueName, tweenValue);
    }

    //创建脚下阴影
    void CreatePoint(Transform parent)
    {
        var p = Resources.Load("Prefabs/Point");
        var point = GameObject.Instantiate(p, parent) as GameObject;
        point.name = "阴影";
        var meshes = parent.GetComponentsInChildren<SkinnedMeshRenderer>();

        //关闭原本阴影
        foreach (var m in meshes)
        {
            m.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        }

        tweenDic.Add(point.GetComponentInChildren<MeshRenderer>().material, new TweenInfo("_N_mask"));
        CreateTween(point.GetComponentInChildren<MeshRenderer>().material);
    }

    //创建直线阴影
    GameObject CreateLine(Vector3 from,Vector3 to)
    {
        to = to + (to - from).normalized * 0.2f;
        var l = Resources.Load("Prefabs/Line");
        var line = GameObject.Instantiate(l, from, character.rotation) as GameObject;
        line.transform.forward = (to - from).normalized;

        line.transform.localScale = new Vector3(1, 1, (to - from).magnitude);

        tweenDic.Add(line.GetComponentInChildren<MeshRenderer>().material, new TweenInfo("_TilingY"));
        CreateTween(line.GetComponentInChildren<MeshRenderer>().material);

        return line;
    }

    ////创建弯曲阴影
    //GameObject CreateMesh(Vector3 from, Vector3 to)
    //{
    //    var bezierMesh = new DrawMesh();
    //    var bezier = bezierMesh.DrawBezierMesh(from, to - character.right * (focus - character.position).magnitude / 3, to, 0.15f);

    //    tweenDic.Add(bezier.GetComponentInChildren<MeshRenderer>().material, new TweenInfo("_TilingY"));
    //    CreateTween(bezier.GetComponentInChildren<MeshRenderer>().material);

    //    return bezier;
    //}

    //创建弯曲阴影
    GameObject CreateMesh(Vector3 from, Vector3 to)
    {
        to = to + character.forward * 0.2f;
        var bezierMesh = new DrawMesh();
        var bezier = bezierMesh.DrawDoubleBezierMesh(from, from + character.forward * (to - from).magnitude * 1/6, to - character.forward * (to - from).magnitude * 1 / 6, to, 0.15f);

        tweenDic.Add(bezier.GetComponentInChildren<MeshRenderer>().material, new TweenInfo("_TilingY"));
        CreateTween(bezier.GetComponentInChildren<MeshRenderer>().material);

        return bezier;
    }

}
