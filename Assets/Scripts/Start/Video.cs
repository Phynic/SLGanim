using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class Video : MonoBehaviour {
    public Transform mainMenu;
    public bool playOnStart = true;
    VideoPlayer vp;
	// Use this for initialization
	private void Start ()
    {
        vp = GetComponent<VideoPlayer>();
        vp.loopPointReached += FadeIn;
        if (playOnStart)
        {
            vp.transform.SetAsLastSibling();
            vp.Prepare();
            vp.prepareCompleted += s => { vp.Play(); };
        }
        else
        {
            Destroy(vp.gameObject);
            mainMenu.gameObject.SetActive(true);
            var screenFader = GameObject.Find("Canvas").GetComponentInChildren<ScreenFader>();
            screenFader.waitForEvent = false;
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
