using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Hakke64 : AttackSkill {



    public override void SetLevel(int level)
    {
        
    }

    public override void GetHit()
    {
        //base.GetHit();
    }

    protected override void InitSkill()
    {
        base.InitSkill();
        var hakke64 = FXManager.GetInstance().Spawn("Hakke64", character, 14f);
        //int originCullingMask = Camera.main.cullingMask;
        //Camera.main.cullingMask = 0;
        //GameObject.Find("Canvas").GetComponent<Canvas>().enabled = false;
        GameObject.Find("Canvas").transform.Find("ScreenFader").GetComponent<ScreenFader>().FadeOut(false);
        GameObject.Find("Directional Light").layer = LayerMask.NameToLayer("Hakke");
        GameObject.Find("Canvas").transform.Find("Hakke").SetAsLastSibling();
        foreach (var item in character.GetComponentsInChildren<Transform>())
        {
            item.gameObject.layer = LayerMask.NameToLayer("Hakke");
        }

        foreach (var o in other)
        {
            foreach (var item in o.GetComponentsInChildren<Transform>())
            {
                item.gameObject.layer = LayerMask.NameToLayer("Hakke");
            }
        }

        GameController.GetInstance().Invoke(() => {
            
            foreach (var item in hakke64.GetComponentsInChildren<MeshRenderer>())
            {
                var mat = item.material;
                CreateTween(mat, "_TintColor", new Color(1, 1, 1, 1), 3f);
            }

            GameController.GetInstance().Invoke(() =>
            {
                foreach (var item in hakke64.GetComponentsInChildren<MeshRenderer>())
                {
                    var mat = item.material;
                    CreateTween(mat, "_TintColor", new Color(1, 1, 1, 0), 3f);
                }
                
                GameController.GetInstance().Invoke(() =>
                {
                    foreach (var item in character.GetComponentsInChildren<Transform>())
                    {
                        item.gameObject.layer = LayerMask.NameToLayer("Default");
                    }

                    foreach (var o in other)
                    {
                        foreach (var item in o.GetComponentsInChildren<Transform>())
                        {
                            item.gameObject.layer = LayerMask.NameToLayer("Hakke");
                        }
                    }

                    GameObject.Find("Directional Light").layer = LayerMask.NameToLayer("Default");
                    
                }, 3f);
                GameObject.Find("Canvas").transform.Find("ScreenFader").GetComponent<ScreenFader>().FadeIn();
            }, 11f);
        }, 0.8f);

    }

    private void CreateTween(Material mat, string valueName, Color endValue, float duration)
    {
        var myValue = mat.GetColor(valueName);
        DOTween.To(() => myValue, x => myValue = x, endValue, duration).OnUpdate(() =>
        {
            mat.SetColor(valueName, myValue);
        });
    }
}
