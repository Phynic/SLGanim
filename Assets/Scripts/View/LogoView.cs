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
            vp.loopPointReached += source => { Close(); };
            vp.transform.SetAsLastSibling();
            vp.Prepare();
            vp.prepareCompleted += source => { vp.Play(); };
        }
        base.Open(onInit);
    }

    public override void Close()
    {
        StartView.GetInstance().Open();
        base.Close();
    }
}
