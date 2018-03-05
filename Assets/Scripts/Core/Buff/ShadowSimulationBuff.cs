using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ShadowSimulationBuff : BanBuff {
    Transform point;
    Transform line;


    public ShadowSimulationBuff(int duration, Transform point) : base(duration)
    {
        this.point = point;
    }

    public ShadowSimulationBuff(int duration, Transform point, Transform line) : base(duration)
    {
        this.point = point;
        this.line = line;
    }

    public override void Undo(Transform character)
    {
        base.Undo(character);

        var meshes = character.GetComponentsInChildren<SkinnedMeshRenderer>();
        
        var time = line ? 0f : 2f;

        RoundManager.GetInstance().Invoke(() => {
            //开启原本阴影
            foreach (var m in meshes)
            {
                m.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
            }
            var mat = point.GetComponentInChildren<MeshRenderer>().material;
            CreateTween(mat, "_N_mask", 0f, 1f);
        }, time);
        
        RoundManager.GetInstance().Invoke(() => { GameObject.Destroy(point.gameObject); }, time + 1f);
        RoundManager.GetInstance().Invoke(() => {
            if (line)
            {
                var mat = line.GetComponentInChildren<MeshRenderer>().material;
                CreateTween(mat, "_TilingY", 0f, 1f);
                RoundManager.GetInstance().Invoke(() => { GameObject.Destroy(line.gameObject); }, 1f);
            }
        }, 1f);
        
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
