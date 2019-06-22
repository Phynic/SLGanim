using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class ScreenFader : MonoBehaviour
{
    public Image fadeImage;
    public float timeBeforeFade = 0.3f;
    public bool waitForEvent = false;
    float fadeTime = 0.5f;
    // Use this for initialization
    void Start()
    {
        fadeImage.color = new Color(0, 0, 0, 1);
        Global.GetInstance().screenFader = this;
        transform.SetAsLastSibling();
        if (!waitForEvent)
        {
            Utils_Coroutine.GetInstance().Invoke(() =>
            {
                FadeIn();
            }, timeBeforeFade);
        }
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
