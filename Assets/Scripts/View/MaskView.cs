using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.Events;

public class MaskView : ViewBase<MaskView>
{
    public override void Refresh()
    {
        transform.SetAsLastSibling();
    }

    /// <summary>
    /// 进入场景，变透明
    /// </summary>
    public void FadeIn()
    {
        var fadeImage = GetComponent<Image>();
        fadeImage.color = new Color(0, 0, 0, 1);
        transform.SetAsLastSibling();
        var tween = fadeImage.DOColor(new Color(0, 0, 0, 0), GameController.GetInstance().fadeTime);
        tween.onComplete = () => { Close(); };
        tween.SetEase(Ease.InQuad);
        base.Open();
    }

    /// <summary>
    /// 淡出场景，变黑
    /// </summary>
    /// <param name="setAsLastSibling"></param>
    /// <param name="onComplete"></param>
    public void FadeOut(bool setAsLastSibling = true, TweenCallback onComplete = null)
    {
        var fadeImage = GetComponent<Image>();
        fadeImage.color = new Color(0, 0, 0, 0);
        if (setAsLastSibling)
            transform.SetAsLastSibling();
        fadeImage.DOColor(new Color(0, 0, 0, 1), GameController.GetInstance().fadeTime).onComplete = onComplete;
        base.Open();
    }
}
