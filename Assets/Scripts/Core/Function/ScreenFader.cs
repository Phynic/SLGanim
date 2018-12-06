using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class ScreenFader : MonoBehaviour {
    public Image fadeImage;
    public float timeBeforeFade = 0.3f;
    public bool waitForEvent = false;
    float fadeTime = 0.5f;
	// Use this for initialization
	void Start () {
        fadeImage.color = new Color(0, 0, 0, 1);
        Global.GetInstance().screenFader = this;
        transform.SetAsLastSibling();
        if (!waitForEvent)
        {
            GameController.GetInstance().Invoke(() =>
            {
                FadeIn();
            }, timeBeforeFade);
        }
    }
	
    public void FadeIn()
    {
        var tween = fadeImage.DOColor(new Color(0, 0, 0, 0), fadeTime);
        tween.SetEase(Ease.InQuad);
    }

    public void FadeOut(bool setAsLastSibling)
    {
        if(setAsLastSibling)
            transform.SetAsLastSibling();
        fadeImage.DOColor(new Color(0, 0, 0, 1), fadeTime);
    }

    public void FadeOut(Action onComplete, bool setAsLastSibling)
    {
        if (setAsLastSibling)
            transform.SetAsLastSibling();
        fadeImage.DOColor(new Color(0, 0, 0, 1), fadeTime).onComplete = () =>
        {
            onComplete.Invoke();
        };
    }
}
