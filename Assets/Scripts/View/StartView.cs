using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Video;

public class StartView : ViewBase<StartView>
{
    public Image bcColorImage;
    public Image bcTextureImage;
    public Image artImage;
    public Transform mainMenu;
    private Button startButton;
    private Button testButton;
    private Button optionsButton;
    private Button exitButton;
    float loopTime = 15;
    float doColorTime = 2;
    int lastRandom = 0;
    Vector3 originPosition;

    public override void Open(UnityAction onInit = null)
    {
        if (!isInit)
        {
            MaskView.GetInstance().FadeIn();
            bcColorImage = transform.Find("Back_Color").GetComponent<Image>();
            bcTextureImage = transform.Find("Back_Texture").GetComponent<Image>();
            artImage = transform.Find("Art").GetComponent<Image>();
            mainMenu = transform.Find("MainMenu");

            startButton = transform.Find("MainMenu/Start").GetComponent<Button>();
            testButton = transform.Find("MainMenu/Test").GetComponent<Button>();
            optionsButton = transform.Find("MainMenu/Options").GetComponent<Button>();
            exitButton = transform.Find("MainMenu/Exit").GetComponent<Button>();

            startButton.onClick.AddListener(NewGame);
            testButton.onClick.AddListener(Test);
            //optionsButton
            exitButton.onClick.AddListener(GameController.GetInstance().Exit);

            originPosition = artImage.transform.localPosition;
            StartCoroutine(ArtLoop());
        }
        base.Open(onInit);
    }

    IEnumerator ArtLoop()
    {
        var random = UnityEngine.Random.Range(1, 26);

        if (lastRandom > 0)
        {
            while (random == lastRandom)
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

        artImage.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        artImage.SetNativeSize();
        var color = ComputeMainColor(artImage.sprite.texture);

        bcColorImage.DOColor(color, doColorTime).SetEase(Ease.Linear);
        bcTextureImage.DOColor(new Color(color.r, color.g, color.b, bcTextureImage.color.a), doColorTime).SetEase(Ease.Linear);
        yield return new WaitForSeconds(doColorTime);

        artImage.DOFade(1, 3).SetEase(Ease.Linear);
        artImage.transform.DOMove(new Vector3(Screen.width / 2 - 100, Screen.height / 2 + 25, 0), loopTime).SetEase(Ease.Linear);
        yield return new WaitForSeconds(loopTime - 1);

        artImage.DOFade(0, 1).SetEase(Ease.Linear);
        yield return new WaitForSeconds(1);

        artImage.transform.localPosition = originPosition;

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

    public void Test()
    {
        StartCoroutine(LoadTest());
    }

    public void NewGame()
    {
        StartCoroutine(LoadNewGame());
    }

    public IEnumerator LoadTest()
    {
        GameController.GetInstance().GalIndex = 0;
        GameController.GetInstance().BattleIndex = 0;
        yield return StartCoroutine(LoadPreset());
        GameController.GetInstance().NextScene("Battle");
    }

    public IEnumerator LoadNewGame()
    {
        GameController.GetInstance().GalIndex = 0;
        GameController.GetInstance().BattleIndex = 1;
        yield return StartCoroutine(LoadPreset());
        GameController.GetInstance().NextScene("Gal");
    }

    public IEnumerator LoadPreset()
    {
        yield return StartCoroutine(XMLManager.LoadAsync<CharacterDataBase>(Application.streamingAssetsPath + "/XML/Preset/characterData.xml", result => GameController.GetInstance().characterDB = result));
        yield return StartCoroutine(XMLManager.LoadAsync<PlayerDataBase>(Application.streamingAssetsPath + "/XML/Preset/playerData.xml", result => GameController.GetInstance().playerDB = result));
    }

    public override void Close()
    {
        //MaskView.GetInstance().FadeOut();
        base.Close();
    }
}
