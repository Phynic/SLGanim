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

        //animator.speed = 0;

        var p = Resources.Load("Prefabs/Point");
        var point = GameObject.Instantiate(p, character) as GameObject;
        point.name = "阴影";
        Sequence s = DOTween.Sequence();
        var meshes = character.GetComponentsInChildren<SkinnedMeshRenderer>();

        

        foreach (var m in meshes)
        {
            m.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        }
        foreach (var o in other)
        {
            CreateLine(character.position, o.position,s);
            var enemyPoint = GameObject.Instantiate(p, o.position, o.rotation) as GameObject;
            enemyPoint.name = "阴影";
            var eMeshes = o.GetComponentsInChildren<SkinnedMeshRenderer>();
            foreach (var m in eMeshes)
            {
                m.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            }
        }
        
    }

    void CreateLine(Vector3 from,Vector3 to, Sequence s)
    {
        if((to - from).normalized != character.forward)
        {
            from = character.position + (focus - character.position) / 2;
            
            var bezierMesh = new DrawMesh();
            var bezier = bezierMesh.DrawBezierMesh(character, from, to - character.forward , to, 0.5f, character.right, character.forward,focus);
            
        }
        else
        {
            var l = Resources.Load("Prefabs/Line");
            var line = GameObject.Instantiate(l, from, Quaternion.Euler(0, Vector3.SignedAngle(character.forward, (to - from).normalized, Vector3.up), 0)) as GameObject;
            //line.transform.DOScaleZ((to - from).magnitude, 2f);

            line.transform.localScale = new Vector3(1, 1, (to - from).magnitude);
        }
        
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

    
}
