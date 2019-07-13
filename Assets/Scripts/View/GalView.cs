using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Table;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GalView : ViewBase<GalView>
{
    private Image backgroundImage;
    private Image leftImage;
    private Image rightImage;
    private Text galSpeaker;
    private Text galText;
    private Button nextButton;
    private Button skipButton;
    private GalSet galSet;
    private List<GalCon> galCons = new List<GalCon>();
    private bool next = false;
    private bool finish;
    private List<Sprite> characterImgs = new List<Sprite>();
    private float fadeTime;

    public override void Open(UnityAction onInit = null)
    {
        if (!isInit)
        {
            backgroundImage = transform.Find("Background").GetComponent<Image>();
            leftImage = transform.Find("Left/Image").GetComponent<Image>();
            rightImage = transform.Find("Right/Image").GetComponent<Image>();
            galSpeaker = transform.Find("GalFrame/Speaker").GetComponent<Text>();
            galText = transform.Find("GalFrame/Text").GetComponent<Text>();
            skipButton = transform.Find("Skip").GetComponent<Button>();
            nextButton = GetComponent<Button>();
            skipButton.onClick.AddListener(Skip);
            nextButton.onClick.AddListener(Next);
            fadeTime = GameController.GetInstance().fadeTime;
            var cImgs = Resources.LoadAll("Textures/Gal/Characters", typeof(Sprite));
            foreach (var cImg in cImgs)
            {
                characterImgs.Add((Sprite)cImg);
            }
        }
        base.Open(onInit);
    }

    public override void Refresh()
    {
        StopAllCoroutines();
        MaskView.GetInstance().Open();
        leftImage.color = Utils_Color.empty;
        rightImage.color = Utils_Color.empty;
        galSpeaker.text = "";
        galText.text = "";
        galSet = null;
        galCons.Clear();
        try
        {
            galSet = GalSetDictionary.GetParam(GameController.GetInstance().GalIndex);
            GameController.GetInstance().GalIndex++;
            var bImg = Resources.Load("Textures/Gal/Background/" + galSet.bcImg, typeof(Sprite));
            backgroundImage.sprite = (Sprite)bImg;
            galCons = GalConDictionary.GetparamList().FindAll(galCon => galSet.startStop.Length == 2 && galCon.ID >= galSet.startStop[0] && galCon.ID <= galSet.startStop[1]);
        }
        catch
        {
            Debug.Log("无对话内容");
        }

        finish = false;

        Utils_Coroutine.GetInstance().Invoke(() =>
        {
            StartCoroutine(PlayGal());
        }, fadeTime);
        
    }

    //优先播旁白
    public IEnumerator PlayVoiceOver()
    {
        var maskView = MaskView.GetInstance();
        var next = maskView.gameObject.AddComponent<Button>();
        next.onClick.AddListener(Next);
        var text = maskView.transform.Find("Text").GetComponent<Text>();
        for (int i = 0; i < galSet.voiceOver.Length; i++)
        {
            text.text = "";
            var textTween = text.DOText("　　" + galSet.voiceOver[i], galSet.voiceOver[i].Length * 0.1f);
            textTween.SetEase(Ease.Linear);
            yield return StartCoroutine(WaitNext(textTween));
        }
        var textFadeTween = text.DOFade(0, fadeTime);
        textFadeTween.SetEase(Ease.InQuad);
        yield return new WaitForSeconds(fadeTime);  //wait fade
        if (galCons.Count == 0)
            GameController.GetInstance().NextScene(galSet.next);
    }

    public IEnumerator PlayGal()
    {
        if (galSet.voiceOver.Length > 0)
            yield return StartCoroutine(PlayVoiceOver());
        MaskView.GetInstance().FadeIn();
        yield return new WaitForSeconds(fadeTime);  //wait fade
        skipButton.gameObject.SetActive(true);
        yield return new WaitForSeconds(fadeTime);  //wait fade
        Image last = null;
        for (int i = 0; i < galCons.Count; i++)
        {
            Image img;
            if (galCons[i].position == "Left")
            {
                img = leftImage;
                last = leftImage;
            }
            else if (galCons[i].position == "Right")
            {
                img = rightImage;
                last = rightImage;
            }
            else
            {
                img = last;
                img.DOFade(0, fadeTime);
                continue;
            }

            if (!img.sprite || img.sprite.name != galCons[i].speaker.ToLower())
            {
                img.sprite = characterImgs.Find(image => image.name == galCons[i].speaker.ToLower());
                img.SetNativeSize();
                img.color = new Color(1, 1, 1, 0);
                img.DOFade(1, fadeTime);
            }

            var textTween = Talk(galCons[i].speaker, galCons[i].content);
            if (i == 0)
                yield return new WaitForSeconds(fadeTime);
            yield return StartCoroutine(WaitNext(textTween));
        }

        GameController.GetInstance().NextScene(galSet.next);
    }

    public Tweener Talk(string speaker, string content)
    {
        var cName = GameController.GetInstance().nameDic[speaker];
        galSpeaker.text = cName.Substring(cName.IndexOf(" ") + 1) + "：";
        galText.text = "";
        var textT = galText.DOText("　　" + content, content.Length * 0.1f);
        textT.SetEase(Ease.Linear);
        return textT;
    }

    IEnumerator WaitNext(Tweener textTween)
    {
        while (true)
        {
            if (next && !finish)
            {
                next = false;
                if (textTween.IsPlaying())
                    textTween.Complete();
                else
                    break;
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void Skip()
    {
        finish = true;
        skipButton.gameObject.SetActive(false);
        GameController.GetInstance().NextScene(galSet.next);
    }

    public void Next()
    {
        next = true;
    }
}