using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class Controller_Gal : MonoBehaviour {
    public Image[] sprites;
    public ScreenFader screenFader;

    private void Start()
    {
        GameController.GetInstance().Invoke(() => {
            ComicToon();
        }, 2);
    }

    public void ComicToon()
    {
        foreach (var s in sprites)
        {
            s.transform.DOMove(new Vector3(Screen.width / 2, Screen.height / 2, 0), 1);
            s.DOFade(1, 1);
            
        }
    }


    public void NextScene()
    {
        screenFader.FadeOut(() => {
            SceneManager.LoadScene("Main");
        }, true);
    }
}
