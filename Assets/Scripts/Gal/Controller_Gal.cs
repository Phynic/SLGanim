using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class Controller_Gal : MonoBehaviour {
    public Image[] sprites;
    public ScreenFader screenFader;
    float comicStep = 3f;

    private void Start()
    {
        GameController.GetInstance().Invoke(() => {
            StartCoroutine(ComicToon());
        }, 2);
    }

    IEnumerator ComicToon()
    {
        foreach (var s in sprites)
        {
            s.transform.DOMove(new Vector3(Screen.width / 2, Screen.height / 2, 0), 1);
            GameController.GetInstance().Invoke(() => {
                s.transform.DOMove(new Vector3(0, Screen.height, 0), 1);
            }, 2f);

            s.DOFade(1, 1);
            GameController.GetInstance().Invoke(() => {
                s.DOFade(0, 1);
            }, 2f);
            yield return new WaitForSeconds(comicStep);
        }
    }


    public void NextScene()
    {
        screenFader.FadeOut(() => {
            SceneManager.LoadScene("Main");
        }, true);
    }
}
