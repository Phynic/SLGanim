using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.Events;

public class MaskView : ViewBase<MaskView>
{
    public Image fadeImage;
    public float timeBeforeFade = 0.3f;
    public bool waitForEvent = false;
    float fadeTime = 0.5f;
    
    public override void Open(UnityAction onInit = null)
    {
        if (!isInit)
        {
            fadeImage.color = new Color(0, 0, 0, 1);
            transform.SetAsLastSibling();
            if (!waitForEvent)
            {
                Utils_Coroutine.GetInstance().Invoke(() =>
                {
                    FadeIn();
                }, timeBeforeFade);
            }
        }
        base.Open(onInit);
    }

    //进入场景
    public void FadeIn()
    {
        transform.SetAsLastSibling();
        var tween = fadeImage.DOColor(new Color(0, 0, 0, 0), fadeTime);
        tween.onComplete = () =>
        {
            fadeImage.raycastTarget = false;
        };
        tween.SetEase(Ease.InQuad);
    }

    //淡出场景
    public void FadeOut(bool setAsLastSibling, TweenCallback onComplete = null)
    {
        if (setAsLastSibling)
            transform.SetAsLastSibling();
        fadeImage.raycastTarget = true;
        fadeImage.DOColor(new Color(0, 0, 0, 1), fadeTime).onComplete = onComplete;
    }
}
