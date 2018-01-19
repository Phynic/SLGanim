using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ShadowSimulation : AttackSkill {
    public int duration;
    float tilingY1 = 0f;
    float tilingY2 = 0f;
    float tilingY3 = 0f;
    List<Material> straightShadow = new List<Material>();
    List<Material> bendShadow = new List<Material>();
    public override void SetLevel(int level)
    {
        duration = level + 2;
    }
    
    public override void Effect()
    {
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

        var p = Resources.Load("Prefabs/Point");
        var point = GameObject.Instantiate(p, character) as GameObject;
        point.name = "阴影";
        var meshes = character.GetComponentsInChildren<SkinnedMeshRenderer>();

        

        foreach (var m in meshes)
        {
            m.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        }
        foreach (var o in other)
        {
            

            GameObject go;
            
            //侧边
            if ((o.position - character.position).normalized != character.forward)
            {
                //确定两边
                if (Vector3.SignedAngle((o.position - character.position).normalized, character.forward, character.up) > 0)
                {
                    RoundManager.GetInstance().Invoke(() => {
                        go = CreateMesh(character.position, o.position);
                        DOTween.To(() => tilingY2, x => tilingY2 = x, 1f, 2f);
                        bendShadow.Add(go.GetComponentInChildren<MeshRenderer>().material);
                    }, 1f);

                    RoundManager.GetInstance().Invoke(() => {
                        var enemyPoint = GameObject.Instantiate(p, o.position, o.rotation) as GameObject;
                        enemyPoint.name = "阴影";
                        var eMeshes = o.GetComponentsInChildren<SkinnedMeshRenderer>();
                        foreach (var m in eMeshes)
                        {
                            m.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                        }
                    }, 3f);

                }
                else
                {
                    RoundManager.GetInstance().Invoke(() => {
                        go = CreateMesh(character.position, o.position);
                        DOTween.To(() => tilingY3, x => tilingY3 = x, 1f, 1.5f);
                        bendShadow.Add(go.GetComponentInChildren<MeshRenderer>().material);
                    }, 1.5f);

                    RoundManager.GetInstance().Invoke(() => {
                        var enemyPoint = GameObject.Instantiate(p, o.position, o.rotation) as GameObject;
                        enemyPoint.name = "阴影";
                        var eMeshes = o.GetComponentsInChildren<SkinnedMeshRenderer>();
                        foreach (var m in eMeshes)
                        {
                            m.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                        }
                    }, 3f);
                }
                
            }
            else
            {
                go = CreateLine(character.position, o.position);
                DOTween.To(() => tilingY1, x => tilingY1 = x, 1f, 2f);
                straightShadow.Add(go.GetComponentInChildren<MeshRenderer>().material);

                RoundManager.GetInstance().Invoke(() => {
                    var enemyPoint = GameObject.Instantiate(p, o.position, o.rotation) as GameObject;
                    enemyPoint.name = "阴影";
                    var eMeshes = o.GetComponentsInChildren<SkinnedMeshRenderer>();
                    foreach (var m in eMeshes)
                    {
                        m.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                    }
                }, 2f);
            }
        }
        
    }

    public override void GetHit()
    {
        
    }

    public override bool OnUpdate(Transform character)
    {
        if (straightShadow.Count > 0)
        {
            foreach (var s in straightShadow)
            {
                s.SetFloat("_TilingY", tilingY1);
            }
        }
        if (bendShadow.Count > 0)
        {
            bendShadow[0].SetFloat("_TilingY", tilingY2);
            
        }
        if(bendShadow.Count > 1)
        {
            bendShadow[1].SetFloat("_TilingY", tilingY3);
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

    GameObject CreateLine(Vector3 from,Vector3 to)
    {
        var l = Resources.Load("Prefabs/Line");
        var line = GameObject.Instantiate(l, from, Quaternion.Euler(0, Vector3.SignedAngle(character.forward, (to - from).normalized, Vector3.up), 0)) as GameObject;
        
        line.transform.localScale = new Vector3(1, 1, (to - from).magnitude);
        return line;
    }

    GameObject CreateMesh(Vector3 from, Vector3 to)
    {
        var bezierMesh = new DrawMesh();
        var bezier = bezierMesh.DrawBezierMesh(from, to - character.right * (focus - character.position).magnitude / 3, to, 0.15f);
        return bezier;
    }
    
}
