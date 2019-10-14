using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SaveLoadView : ViewBase<SaveLoadView>
{
    private GameObject _Button;
    List<GameObject> allButtons = new List<GameObject>();
    List<Save> saves = new List<Save>();
    private Transform contentTrans;
    private Button closeButton;
    
    public void Open(bool save, UnityAction onInit = null)
    {
        if (!isInit)
        {
            contentTrans = transform.Find("List/Viewport/Content");
            closeButton = transform.Find("Close").GetComponent<Button>();
            closeButton.onClick.AddListener(Close);
            if (save)
            {
                CreateSaveList();
            }
            else
            {
                CreateLoadList();
            }
        }
        base.Open(onInit);
    }

    public void CreateSaveList()
    {
        allButtons.Clear();
        saves = Utils_Save.LoadSaveList();
        if (saves.Count < Global.maxSaveCount)
        {
            var newSave = new Save();
            newSave.saveName = "新存档";
            saves.Add(newSave);
        }

        CreateButtons(saves);
        contentTrans.GetComponent<RectTransform>().sizeDelta = new Vector2(GetComponent<RectTransform>().sizeDelta.x, allButtons[0].GetComponent<RectTransform>().sizeDelta.y * allButtons.Count + 5 * (allButtons.Count - 1));

        for (int i = 0; i < allButtons.Count; i++)
        {
            allButtons[i].GetComponent<Button>().onClick.AddListener(OnSaveButtonClick);
            allButtons[i].transform.localPosition = new Vector3(0, -(int)(i * (allButtons[i].GetComponent<RectTransform>().sizeDelta.y + 5)), 0);
        }
    }

    public void CreateLoadList()
    {
        allButtons.Clear();
        saves = Utils_Save.LoadSaveList();
        if(saves.Count == 0)
        {
            return;
        }
        CreateButtons(saves);

        contentTrans.GetComponent<RectTransform>().sizeDelta = new Vector2(GetComponent<RectTransform>().sizeDelta.x, allButtons[0].GetComponent<RectTransform>().sizeDelta.y * allButtons.Count + 5 * (allButtons.Count - 1));

        for (int i = 0; i < allButtons.Count; i++)
        {
            allButtons[i].GetComponent<Button>().onClick.AddListener(OnLoadButtonClick);
            allButtons[i].transform.localPosition = new Vector3(0, -(int)(i * (allButtons[i].GetComponent<RectTransform>().sizeDelta.y + 5)), 0);
        }
    }

    public void CreateButtons(List<Save> saves)
    {
        for (int i = 0; i < saves.Count; i++)
        {
            _Button = (GameObject)Resources.Load("Prefabs/UI/SaveButton");
            GameObject button = GameObject.Instantiate(_Button, contentTrans);
            button.name = GameManager.IndexToString(i);

            //button.GetComponentInChildren<Text>().alignment = TextAnchor.MiddleLeft;
            //button.GetComponentInChildren<Text>().text = tempSkill.CName;
            //button.GetComponentInChildren<Text>().resizeTextForBestFit = false;
            //button.GetComponentInChildren<Text>().fontSize = 45;
            //button.GetComponentInChildren<Text>().GetComponent<RectTransform>().sizeDelta = new Vector2(-30, 0);

            button.transform.Find("SaveName").GetComponent<Text>().text = saves[i].saveName;
            if (saves[i].saveDate != null && saves[i].saveDate != "")
                button.transform.Find("SaveTime").GetComponent<Text>().text = Utils_Time.StampToDateTime(saves[i].saveDate);
            else
                button.transform.Find("SaveTime").GetComponent<Text>().text = "";
            //button.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 72);
            button.GetComponent<RectTransform>().pivot = new Vector2(0f, 1f);
            button.GetComponent<RectTransform>().anchorMin = new Vector2(0, 1);
            button.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);
            allButtons.Add(button);
        }
    }

    private void OnLoadButtonClick()
    {
        var btn = EventSystem.current.currentSelectedGameObject;
        Utils_Save.Load(saves[int.Parse(btn.name)]);
        Close();
    }

    private void OnSaveButtonClick()
    {
        var btn = EventSystem.current.currentSelectedGameObject;
        Utils_Save.Save(btn.name);
        Close();
    }

    public override void Close()
    {
        for (int i = 0; i < allButtons.Count; i++)
        {
            Destroy(allButtons[i]);
        }
        allButtons.Clear();
        base.Close();
    }
}
