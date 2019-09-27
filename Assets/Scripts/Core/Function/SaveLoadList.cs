using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SaveLoadList : MonoBehaviour {
    private GameObject _Button;
    List<GameObject> allButtons = new List<GameObject>();
    
    public void CreateSaveList()
    {
        allButtons.Clear();
        var saves = new List<Save>();

        if (saves.Count < GameManager.GetInstance().maxSaveCount)
        {
            var newSave = new Save();
            newSave.saveName = "新存档";
            CreateButton(newSave);
        }
        
        foreach (var save in saves)
        {
            CreateButton(save);
        }

        GetComponent<RectTransform>().sizeDelta = new Vector2(GetComponent<RectTransform>().sizeDelta.x, allButtons[0].GetComponent<RectTransform>().sizeDelta.y * allButtons.Count +  5 *(allButtons.Count - 1));
        
        for (int i = 0; i < allButtons.Count; i++)
        {
            allButtons[i].GetComponent<Button>().onClick.AddListener(OnSaveButtonClick);
            allButtons[i].transform.localPosition = new Vector3(0, -(int)(i * (allButtons[i].GetComponent<RectTransform>().sizeDelta.y + 5)), 0);
        }
    }

    public void CreateLoadList()
    {
        allButtons.Clear();
        var saves = new List<Save>();

        foreach (var save in saves)
        {
            CreateButton(save);
        }

        GetComponent<RectTransform>().sizeDelta = new Vector2(GetComponent<RectTransform>().sizeDelta.x, allButtons[0].GetComponent<RectTransform>().sizeDelta.y * allButtons.Count + 5 * (allButtons.Count - 1));

        for (int i = 0; i < allButtons.Count; i++)
        {
            allButtons[i].GetComponent<Button>().onClick.AddListener(OnLoadButtonClick);
            allButtons[i].transform.localPosition = new Vector3(0, -(int)(i * (allButtons[i].GetComponent<RectTransform>().sizeDelta.y + 5)), 0);
        }
    }

    public void CreateButton(Save save)
    {
        _Button = (GameObject)Resources.Load("Prefabs/UI/SaveButton");
        GameObject button = GameObject.Instantiate(_Button, transform);
        //button.name = GameController.GetInstance().IndexToString(save.ID);

        //button.GetComponentInChildren<Text>().alignment = TextAnchor.MiddleLeft;
        //button.GetComponentInChildren<Text>().text = tempSkill.CName;
        //button.GetComponentInChildren<Text>().resizeTextForBestFit = false;
        //button.GetComponentInChildren<Text>().fontSize = 45;
        //button.GetComponentInChildren<Text>().GetComponent<RectTransform>().sizeDelta = new Vector2(-30, 0);

        button.transform.Find("SaveName").GetComponent<Text>().text = save.saveName;
        if (save.SaveDate != "")
            button.transform.Find("SaveTime").GetComponent<Text>().text = Utils_Time.StampToDateTime(save.SaveDate);
        else
            button.transform.Find("SaveTime").GetComponent<Text>().text = "";
        //button.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 72);
        button.GetComponent<RectTransform>().pivot = new Vector2(0f, 1f);
        button.GetComponent<RectTransform>().anchorMin = new Vector2(0, 1);
        button.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);
        allButtons.Add(button);
        transform.parent.parent.parent.gameObject.SetActive(true);
    }

    public void Clear()
    {
        for (int i = 0; i < allButtons.Count; i++)
        {
            Destroy(allButtons[i]);
        }
        allButtons.Clear();
        transform.parent.parent.parent.gameObject.SetActive(false);
    }

    private void OnLoadButtonClick()
    {
        var btn = EventSystem.current.currentSelectedGameObject;
        Utils_Save.Load(btn.name);
        Clear();
    }

    private void OnSaveButtonClick()
    {
        var btn = EventSystem.current.currentSelectedGameObject;
        Utils_Save.Save(btn.name);
        Clear();
    }
}
