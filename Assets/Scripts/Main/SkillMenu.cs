using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillMenu : MonoBehaviour {

    public Transform character;

    private GameObject _Button;
    private GameObject _SkillButtonImages;
    private List<Sprite> imagesList = new List<Sprite>();

    private void Start()
    {
        _Button = (GameObject)Resources.Load("Prefabs/UI/Button");
        _SkillButtonImages = (GameObject)Resources.Load("Prefabs/UI/SkillButtonImages");

        var images = Resources.LoadAll("Textures/SkillButtonImages", typeof(Sprite));

        foreach (var i in images)
        {
            imagesList.Add((Sprite)i);
        }

        GameController.GetInstance().Invoke(() => {
            UnitManager.GetInstance().units.ForEach(u => { u.Initialize(); });
            CreateSkillList();
        }, 0.1f);
    }

    public void CreateSkillList()
    {
        var unitSkillData = character.GetComponent<CharacterStatus>().skills;
        var UIContent = transform.Find("Content");
        var allButtons = new List<GameObject>();
        GameObject button;
        foreach (var skill in unitSkillData)
        {
            var tempSkill = (UnitSkill)SkillManager.GetInstance().skillList.Find(s => s.EName == skill.Key);

            button = GameObject.Instantiate(_Button, UIContent);

            button.GetComponentInChildren<Text>().alignment = TextAnchor.MiddleLeft;
            button.GetComponentInChildren<Text>().text = tempSkill.CName;
            button.GetComponentInChildren<Text>().resizeTextForBestFit = false;
            button.GetComponentInChildren<Text>().fontSize = 45;
            button.GetComponentInChildren<Text>().GetComponent<RectTransform>().sizeDelta = new Vector2(-30, 0);
            button.name = skill.Key;
            //button.GetComponent<Button>().onClick.AddListener(OnButtonClick);
            button.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 72);
            button.GetComponent<RectTransform>().pivot = new Vector2(0f, 1f);
            button.GetComponent<RectTransform>().anchorMin = new Vector2(0, 1);
            button.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);
            allButtons.Add(button);

            var imageUI = UnityEngine.Object.Instantiate(_SkillButtonImages, button.transform);

            var _Class = imageUI.transform.Find("SkillClass").GetComponent<Image>();
            var _Type = imageUI.transform.Find("SkillType").GetComponent<Image>();
            var _Combo = imageUI.transform.Find("SkillCombo").GetComponent<Image>();
            //Debug.Log(imagesList[0].name.Substring(11));
            _Class.sprite = imagesList.Find(i => i.name.Substring(11) == tempSkill.skillClass.ToString());
            _Type.sprite = imagesList.Find(i => i.name.Substring(10) == tempSkill.skillType.ToString());
            _Combo.gameObject.SetActive(tempSkill.comboType != UnitSkill.ComboType.cannot);
        }

        UIContent.GetComponent<RectTransform>().sizeDelta = new Vector2(UIContent.GetComponent<RectTransform>().sizeDelta.x, allButtons[0].GetComponent<RectTransform>().sizeDelta.y * (allButtons.Count));

        //设置按钮位置
        for (int i = 0; i < allButtons.Count; i++)
        {
            allButtons[i].transform.localPosition = new Vector3(allButtons[i].transform.localPosition.x, -(int)(i * (allButtons[i].GetComponent<RectTransform>().sizeDelta.y)), 0);
        }
    }
}
