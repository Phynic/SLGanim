using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ShadowSimulation : AttackSkill {

    public int duration;
    
    public override void SetLevel(int level)
    {
        duration = level + 2;
    }
    
    public override void Effect()
    {
        //不飘字
        calculateDamage = false;
        
        base.Effect();

        animator.speed = 0;

        int missCount = 0;

        float timeline = 0;

        for(int i = 0; i < other.Count; i++)
        {
            var o = other[i];
            
            //侧边
            if ((int)Vector3.Angle((o.position - character.position).normalized, character.forward) % 90 != 0)
            {
                timeline = timeline + 0.3f;
                //确定两边
                GameObject go = null;
                RoundManager.GetInstance().Invoke(() => { go = CreateMesh(character.position, o.position); }, timeline);
                if (DamageSystem.Miss(character, o, skillRate))
                {
                    RoundManager.GetInstance().Invoke(() => {
                        var mat = go.GetComponentInChildren<MeshRenderer>().material;
                        CreateTween(mat, "_TilingY", 0f, 1f);

                        DebugLogPanel.GetInstance().Log("Miss");
                        
                        RoundManager.GetInstance().Invoke(() => { GameObject.Destroy(go); }, 1f);
                        }, timeline + 1);
                    missCount++;
                }
                else
                {
                    RoundManager.GetInstance().Invoke(() => {
                        var point = CreatePoint(o);
                        var shadowSimulationBuff = new ShadowSimulationBuff(duration, point.transform, go.transform);
                        o.GetComponent<Unit>().Buffs.Add(shadowSimulationBuff);
                        shadowSimulationBuff.Apply(o);
                    }, timeline + 1);
                    
                }
            }
            else
            {
                GameObject go = CreateLine(character.position, o.position);

                if (DamageSystem.Miss(character, o, skillRate))
                {
                    RoundManager.GetInstance().Invoke(() => {
                        var mat = go.GetComponentInChildren<MeshRenderer>().material;
                        CreateTween(mat, "_TilingY", 0f, 1f);

                        DebugLogPanel.GetInstance().Log("Miss");
                        RoundManager.GetInstance().Invoke(() => { GameObject.Destroy(go); }, 1f);
                    }, 1);
                    missCount++;
                }
                else
                {
                    RoundManager.GetInstance().Invoke(() => {
                        var point = CreatePoint(o);
                        var shadowSimulationBuff = new ShadowSimulationBuff(duration, point.transform, go.transform);
                        o.GetComponent<Unit>().Buffs.Add(shadowSimulationBuff);
                        shadowSimulationBuff.Apply(o);
                    }, 1);
                    
                }
            }
        }
        
        var selfPoint = CreatePoint(character);
        if(missCount == other.Count)
        {
            RoundManager.GetInstance().Invoke(() => {
                var meshes = character.GetComponentsInChildren<SkinnedMeshRenderer>();

                //开启原本阴影
                foreach (var m in meshes)
                {
                    m.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                }

                var mat = selfPoint.GetComponentInChildren<MeshRenderer>().material;
                
                CreateTween(mat, "_N_mask", 0f, 1f);

            }, 2f);
        }
        else
        {
            //给自身施加禁止buff
            var buff = new ShadowSimulationBuff(duration, selfPoint.transform);
            character.GetComponent<Unit>().Buffs.Add(buff);
            buff.Apply(character);
        }
        
        RoundManager.GetInstance().Invoke(() => { animator.speed = 1; }, timeline + 2);

    }

    public override void GetHit()
    {
        
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

    //创建脚下阴影
    GameObject CreatePoint(Transform parent)
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

        var mat = point.GetComponentInChildren<MeshRenderer>().material;
        
        CreateTween(mat, "_N_mask", 1f, 1f);

        return point;
    }

    //创建直线阴影
    GameObject CreateLine(Vector3 from,Vector3 to)
    {
        //适配末梢位置在脚下
        to = to + (to - from).normalized * 0.08f * (to - from).magnitude;

        var l = Resources.Load("Prefabs/Line");
        var line = GameObject.Instantiate(l, from, character.rotation) as GameObject;

        

        line.transform.forward = (to - from).normalized;

        line.transform.localScale = new Vector3(1, 1, (to - from).magnitude);

        var mat = line.GetComponentInChildren<MeshRenderer>().material;
        
        CreateTween(mat, "_TilingY", 1f, 1f);

        return line;
    }

    ////创建弯曲阴影
    ////一条贝塞尔
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
        //适配末梢位置在脚下
        to = to + (to - from).normalized * 0.04f * (to - from).magnitude;

        var bezierMesh = new DrawMesh();
        var p11 = from + character.forward * (to - from).magnitude * 1 / 6;
        var p12 = to - character.forward * (to - from).magnitude * 1 / 6;
        var bezier = bezierMesh.DrawDoubleBezierMesh(from, p11, p12, to, 0.15f);

        //物理忽略
        bezier.layer = 2;

        var mat = bezier.GetComponentInChildren<MeshRenderer>().material;
        CreateTween(mat, "_TilingY", 1f, 1f);
        
        return bezier;
    }

    private void CreateTween(Material mat, string valueName, float endValue, float duration)
    {
        float myValue = mat.GetFloat(valueName);
        DOTween.To(() => myValue, x => myValue = x, endValue, duration).OnUpdate(() =>
        {
            mat.SetFloat(valueName, myValue);
        });
    }
}
