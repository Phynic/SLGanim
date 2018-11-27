using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Video;
using System;

public class Controller_Start : Singleton<Controller_Start>
{
    public ScreenFader screenFader;
    public Image bcColor;
    public Image bcTexture;
    public Image art;
    float loopTime = 15;
    float doColorTime = 2;
    int lastRandom = 0;
    Vector3 originPosition;

    public void NextScene(string sceneName)
    {
        screenFader.FadeOut(() => {
            Global.GetInstance().NextScene(sceneName);
        }, true);
    }

    public void TestScene()
    {
        Global.GetInstance().BattleIndex = 0;
        NextScene("Battle");
    }

    private void Start()
    {
        var vp = screenFader.GetComponentInChildren<VideoPlayer>();
        vp.Prepare();
        vp.prepareCompleted += s => { vp.Play(); };
        originPosition = art.transform.position;
        StartCoroutine(ArtLoop());
    }

    IEnumerator ArtLoop()
    {
        var random = UnityEngine.Random.Range(1, 26);

        if (lastRandom > 0)
        {
            while(random == lastRandom)
            {
                random = UnityEngine.Random.Range(1, 26);
            }
        }

        lastRandom = random;

        string path = "Textures/Start/";

        if (random < 10)
            path += "0";
        path += random.ToString();
        var tex = Resources.Load(path) as Texture2D;
        Resources.UnloadUnusedAssets();

        art.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        art.SetNativeSize();
        var color = ComputeMainColor(art.sprite.texture);

        bcColor.DOColor(color, doColorTime).SetEase(Ease.Linear);
        bcTexture.DOColor(new Color(color.r, color.g, color.b, bcTexture.color.a), doColorTime).SetEase(Ease.Linear);
        yield return new WaitForSeconds(doColorTime);

        art.DOFade(1, 3).SetEase(Ease.Linear);
        art.transform.DOMove(new Vector3(Screen.width / 2 - 150, Screen.height / 2, 0), loopTime).SetEase(Ease.Linear);
        yield return new WaitForSeconds(loopTime - 1);

        art.DOFade(0, 1).SetEase(Ease.Linear);
        yield return new WaitForSeconds(1);

        art.transform.position = originPosition;
        
        yield return StartCoroutine(ArtLoop());
    }

    Color ComputeMainColor(Texture2D img)
    {
        float r = 0;
        float g = 0;
        float b = 0;
        
        int width = img.width;
        int height = img.height;
        Color[] colors = new Color[width * height];

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                colors[i * height + j] = img.GetPixel(i, j);
                r += colors[i * height + j].r;
                g += colors[i * height + j].g;
                b += colors[i * height + j].b;
            }
        }

        r /= width * height;
        g /= width * height;
        b /= width * height;
        
        var result = new Color(r, g, b);
        result -= Color.gray * 0.2f;
        return result;
    }
}

