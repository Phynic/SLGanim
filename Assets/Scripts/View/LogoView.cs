using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Video;

public class LogoView : ViewBase<LogoView>
{
    private VideoPlayer vp;

    public override void Open(UnityAction onInit = null)
    {
        if (!isInit)
        {
            vp = GetComponent<VideoPlayer>();

            if (GameController.GetInstance().playLogo)
            {
                vp.loopPointReached += FadeIn;
                vp.transform.SetAsLastSibling();
                vp.Prepare();
                vp.prepareCompleted += s => { vp.Play(); };
            }
            else
            {
                FadeIn(vp);
            }
        }
        base.Open(onInit);
    }

    private void FadeIn(VideoPlayer source)
    {
        var screenFader = transform.root.Find("ScreenFader").GetComponent<MaskView>();
        Destroy(gameObject);
        StartView.GetInstance().Open();
        screenFader.transform.SetAsLastSibling();
        Utils_Coroutine.GetInstance().Invoke(screenFader.FadeIn, 1);
    }
}
