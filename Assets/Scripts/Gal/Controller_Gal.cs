using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class Controller_Gal : MonoBehaviour
{

    public MaskView screenFader;

    private void Awake()
    {
        //instance = this;
        screenFader.enabled = false;
    }

    public void NextScene(string nextScene)
    {
        GameController.GetInstance().NextScene(nextScene);
    }
}