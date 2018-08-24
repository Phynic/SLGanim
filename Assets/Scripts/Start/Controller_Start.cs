using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Controller_Start : MonoBehaviour {
    public ScreenFader screenFader;

    public void NextScene()
    {
        screenFader.FadeOut(() => {
            SceneManager.LoadScene("Main");
        }, true);
    }
}
