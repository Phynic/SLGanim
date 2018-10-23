using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class Controller_Gal : Singleton<Controller_Gal> {

    public ScreenFader screenFader;


    private void Start()
    {
        
    }

    public void NextScene(string nextScene)
    {
        screenFader.FadeOut(() => {
            Global.GetInstance().NextScene(nextScene);

        }, true);
    }
}