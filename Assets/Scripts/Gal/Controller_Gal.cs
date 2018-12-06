using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class Controller_Gal : Singleton<Controller_Gal> {

    public ScreenFader screenFader;
    
    private void Awake()
    {
        instance = this;
        screenFader.enabled = false;
    }

    public void NextScene(string nextScene)
    {
        Global.GetInstance().NextScene(nextScene);
    }
}