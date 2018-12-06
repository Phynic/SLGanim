using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class Video : MonoBehaviour {
    public Transform mainMenu;
    VideoPlayer vp;
	// Use this for initialization
	private void Start ()
    {
        vp = GetComponent<VideoPlayer>();
        vp.loopPointReached += FadeIn;
    }
	
    private void FadeIn(VideoPlayer source)
    {
        transform.root.Find("ScreenFader").GetComponent<ScreenFader>().FadeIn();
        Destroy(gameObject);
        mainMenu.gameObject.SetActive(true);
    }
}
