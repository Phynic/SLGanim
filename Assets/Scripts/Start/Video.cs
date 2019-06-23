using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class Video : MonoBehaviour {
    public bool playOnStart = true;
    VideoPlayer vp;
	// Use this for initialization
	private void Start ()
    {
        vp = GetComponent<VideoPlayer>();
        
        if (playOnStart)
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
	
    private void FadeIn(VideoPlayer source)
    {
        var screenFader = transform.root.Find("ScreenFader").GetComponent<ScreenFader>();
        Destroy(gameObject);
        StartView.GetInstance().Open();
        screenFader.transform.SetAsLastSibling();
        Utils_Coroutine.GetInstance().Invoke(screenFader.FadeIn, 1);
    }
}
