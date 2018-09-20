using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class Controller_Gal : MonoBehaviour {

    public ScreenFader screenFader;


    private void Start()
    {
        
    }

    public void NextScene()
    {
        screenFader.FadeOut(() => {
            SceneManager.LoadScene("Main");
        }, true);
    }
}