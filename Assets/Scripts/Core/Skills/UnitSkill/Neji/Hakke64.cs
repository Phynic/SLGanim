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

    public override void Effect()
    {
        base.Effect();
    }

    protected override void InitSkill()
    {
        base.InitSkill();

        var go1 = Resources.Load("Prefabs/Skills/Hakke/HakkeCamera") as GameObject;
        var go2 = Resources.Load("Prefabs/Skills/Hakke/HakkeRT") as GameObject;

        var hakkeCamera = GameObject.Instantiate(go1, GameObject.Find("Main Camera").transform);
        var hakkeRT = GameObject.Instantiate(go2, GameObject.Find("Canvas").transform);

        var hakke64 = FXManager.GetInstance().Spawn("Hakke64", character, 14f);
        
        GameObject.Find("Canvas").transform.Find("ScreenFader").GetComponent<ScreenFader>().FadeOut(false);
        GameObject.Find("Directional Light").layer = LayerMask.NameToLayer("Hakke");
        hakkeRT.transform.SetAsLastSibling();
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
                    CreateTween(mat, "_TintColor", new Color(1, 1, 1, 0), 1.5f);
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
                            item.gameObject.layer = LayerMask.NameToLayer("Default");
                        }
                    }

                    GameObject.Find("Directional Light").layer = LayerMask.NameToLayer("Default");
                    GameObject.Destroy(hakkeCamera);
                    GameObject.Destroy(hakkeRT);
                    GameObject.Find("Canvas").transform.Find("ScreenFader").GetComponent<ScreenFader>().FadeIn();
                    Complete();
                }, 1.5f);
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
