using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.IO;

public class GalManager : Singleton<GalManager> {
    public Transform background;
    public Transform left;
    public Transform right;
    public Transform galFrame;
    public Gal gal;

    private bool next = false;

    private void Start()
    {
        try
        {
            StartCoroutine(XMLManager.LoadSync<Gal>(Application.streamingAssetsPath + "/XML/gal.xml", result => gal = result));
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
                foreach (var galCon in gal.galCons)
                {
                    if (cImg.name == galCon.speaker.ToLower())
                        galCon.characterImage = (Sprite)cImg;
                }
            }

            StartCoroutine(PlayGal());
        }, 1f);
    }
    
    public IEnumerator PlayGal()
    {
        for (int i = 0; i < gal.galCons.Count; i++)
        {
            Image img;
            if (gal.galCons[i].position == "Left")
                img = left.Find("Image").GetComponent<Image>();
            else
                img = right.Find("Image").GetComponent<Image>();
            if (img.sprite != gal.galCons[i].characterImage)
            {
                img.sprite = gal.galCons[i].characterImage;
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

    public void Next()
    {
        next = true;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Next();
        }
    }
}

[System.Serializable]
public class Gal
{
    public string bcImg;
    public string nextScene;
    public List<GalCon> galCons = new List<GalCon>();
}

[System.Serializable]
public class GalCon
{
    public string speaker;
    public string position;
    public string content;

    public Sprite characterImage;

    public GalCon() { }
    
    public GalCon(string speaker, string position, string content)
    {
        this.speaker = speaker;
        this.position = position;
        this.content = content;
    }
}
