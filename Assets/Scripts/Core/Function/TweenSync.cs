using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TweenInfo
{
    public string valueName;
    public float tweenValue = 0f;
    public TweenInfo(string valueName)
    {
        this.valueName = valueName;
    }
}

public class TweenSync : MonoBehaviour {

    private static TweenSync instance;

    public static TweenSync GetInstance()
    {
        return instance;
    }

    //动画字典
    Dictionary<Material, TweenInfo> tweenDic = new Dictionary<Material, TweenInfo>();

    private void Add(Material m, TweenInfo tweenInfo)
    {
        enabled = true;
        tweenDic.Add(m, tweenInfo);
    }

    public void Remove(Material key, float time)
    {
        RoundManager.GetInstance().Invoke(() => { tweenDic.Remove(key); }, time);
    }

    private void ValueSync(Material m, string valueName, float tweenValue)
    {
        m.SetFloat(valueName, tweenValue);
    }

    public void CreateTween(Material m, float targetValue, TweenInfo tweenInfo)
    {
        if(!tweenDic.ContainsKey(m))
            Add(m, tweenInfo);
        DOTween.To(() => tweenDic[m].tweenValue, x => tweenDic[m].tweenValue = x, targetValue, 1f);
        Remove(m, 3f);
    }

    private void Awake()
    {
        instance = this;
    }

    private void Update () {
        if(tweenDic.Count == 0)
        {
            enabled = false;
        }
        foreach (var t in tweenDic)
        {
            ValueSync(t.Key, t.Value.valueName, t.Value.tweenValue);
        }
    }
}
