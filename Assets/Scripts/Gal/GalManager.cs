using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using System.IO;

public class GalManager : Singleton<GalManager> {
    public Transform background;
    public Transform left;
    public Transform right;
    public Transform galFrame;
    public Gal gal;

    private bool next = false;
    private List<Sprite> characterImgs = new List<Sprite>();

    private void Start()
    {
        try
        {
            StartCoroutine(XMLManager.LoadSync<Gal>(Application.streamingAssetsPath + "/XML/Core/Gal/gal_" + Global.GetInstance().GalIndex.ToString() + ".xml", result => {
                gal = result;
                Global.GetInstance().GalIndex++;
                var bImg = Resources.Load("Textures/Gal/Background/" + gal.bcImg, typeof(Sprite));
                background.GetComponent<Image>().sprite = (Sprite)bImg;
            }));
        }
        catch
        {
            Debug.Log("本场景无对话内容。");
        }
        
        var cImgs = Resources.LoadAll("Textures/Gal/Characters", typeof(Sprite));
        
        GameController.GetInstance().Invoke(() =>
        {
            foreach (var cImg in cImgs)
            {
                characterImgs.Add((Sprite)cImg);
            }

            StartCoroutine(PlayGal());
        }, 1f);
    }
    
    public IEnumerator PlayVoiceOver()
    {
        var text = Controller_Gal.GetInstance().screenFader.transform.Find("Text").GetComponent<Text>();
        for (int i = 0; i < gal.voiceOver.Count; i++)
        {
            text.text = "";
            var textTween = text.DOText("　　" + gal.voiceOver[i], gal.voiceOver[i].Length * 0.1f);
            textTween.SetEase(Ease.Linear);
            yield return StartCoroutine(WaitNext(textTween));
        }
        var textFadeTween = text.DOFade(0, 0.5f);
        textFadeTween.SetEase(Ease.InQuad);
    }

    public IEnumerator PlayGal()
    {
        if (gal.voiceOver.Count > 0)
            yield return StartCoroutine(PlayVoiceOver());
        yield return new WaitForSeconds(0.5f);  //wait fade
        Controller_Gal.GetInstance().screenFader.enabled = true;
        yield return new WaitForSeconds(0.5f);  //wait fade
        Transform last = null;
        for (int i = 0; i < gal.galCons.Count; i++)
        {
            Image img;
            if (gal.galCons[i].position == "Left")
            {
                img = left.Find("Image").GetComponent<Image>();
                last = left;
            }
            else if (gal.galCons[i].position == "Right")
            {
                img = right.Find("Image").GetComponent<Image>();
                last = right;
            }
            else
            {
                img = last.Find("Image").GetComponent<Image>();
                img.DOFade(0, 0.5f);
                continue;
            }

            if (!img.sprite || img.sprite.name != gal.galCons[i].speaker.ToLower())
            {
                img.sprite = characterImgs.Find(image => image.name == gal.galCons[i].speaker.ToLower());
                img.SetNativeSize();
                img.color = new Color(1, 1, 1, 0);
                img.DOFade(1, 0.5f);
            }
            
            var textTween = Talk(gal.galCons[i].speaker, gal.galCons[i].content);
            if (i == 0)
                yield return new WaitForSeconds(0.5f);
            yield return StartCoroutine(WaitNext(textTween));
        }

        Controller_Gal.GetInstance().NextScene(gal.nextScene);
    }

    public Tweener Talk(string speaker, string content)
    {
        var cName = Global.GetInstance().characterDB.characterDataList.Find(c => c.roleEName == speaker).roleCName;
        galFrame.Find("Speaker").GetComponent<Text>().text = cName.Substring(cName.IndexOf(" ") + 1) + "：";
        galFrame.Find("Text").GetComponent<Text>().text = "";
        var textT = galFrame.Find("Text").GetComponent<Text>().DOText("　　" + content, content.Length * 0.1f);
        textT.SetEase(Ease.Linear);
        return textT;
    }
    
    IEnumerator WaitNext(Tweener textTween)
    {
        while (true)
        {
            if (next)
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
        Controller_Gal.GetInstance().NextScene(gal.nextScene);
    }

    public void Next()
    {
        next = true;
    }

    private void Update()
    {
#if (UNITY_STANDALONE || UNITY_EDITOR)
        if (Input.GetMouseButtonDown(0))
        {
            if(!EventSystem.current.currentSelectedGameObject)
            {
                Next();
            }
            else
            {
                if (EventSystem.current.currentSelectedGameObject.name != "Skip")
                {
                    Next();
                }
            }
        }

#elif (!UNITY_EDITOR && (UNITY_IOS || UNITY_ANDROID))
        if (Input.touchCount > 0)
        {
            if(!EventSystem.current.currentSelectedGameObject)
            {
                Next();
            }
            else
            {
                if (EventSystem.current.currentSelectedGameObject.name != "Skip")
                {
                    Next();
                }
            }
        }
#endif
    }
}

[System.Serializable]
public class Gal
{
    public string bcImg;
    public string nextScene;
    public List<string> voiceOver;
    public List<GalCon> galCons = new List<GalCon>();
}

[System.Serializable]
public class GalCon
{
    public string speaker;
    public string position;
    public string content;
    
    public GalCon() { }
    
    public GalCon(string speaker, string position, string content)
    {
        this.speaker = speaker;
        this.position = position;
        this.content = content;
    }
}
