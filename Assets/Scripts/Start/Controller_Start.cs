using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Controller_Start : Singleton<Controller_Start>
{
    public ScreenFader screenFader;
    
    public void NextScene()
    {
        screenFader.FadeOut(() => {
            Global.GetInstance().NextScene();
        }, true);
    }
}

