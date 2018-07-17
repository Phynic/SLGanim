using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SkillMenu : MonoBehaviour {
    
    private GameObject _Button;
    private GameObject _SkillButtonImages;
    private GameObject _SkillLevelImages;
    private List<Sprite> imagesList = new List<Sprite>();
    private List<GameObject> allButtons = new List<GameObject>();
    private void Awake()
    {
        _Button = (GameObject)Resources.Load("Prefabs/UI/Button");
        _SkillButtonImages = (GameObject)Resources.Load("Prefabs/UI/SkillButtonImages_Single");
        _SkillLevelImages = (GameObject)Resources.Load("Prefabs/UI/SkillLevelImages");

        var images = Resources.LoadAll("Textures/SkillButtonImages/Single", typeof(Sprite));

        foreach (var i in images)
        {
            imagesList.Add((Sprite)i);
        }
    }
    
    public void UpdateView()
    {
        gameObject.SetActive(true);
        foreach (var b in allButtons)
        {
            Destroy(b);
        }
        CreateSkillList(Controller_Main.GetInstance().character);
    }

    public void CreateSkillList(Transform character)
    {
        var unitSkillData = character.GetComponent<CharacterStatus>().skills;
        var UIContent = transform.Find("Scroll View").Find("Viewport").Find("Content");
        allButtons.Clear();
        GameObject button;
        foreach (var skill in unitSkillData)
        {
            var tempSkill = SkillManager.GetInstance().skillList.Find(s => s.EName == skill.Key);

            button = GameObject.Instantiate(_Button, UIContent);

            button.GetComponentInChildren<Text>().alignment = TextAnchor.MiddleLeft;
            button.GetComponentInChildren<Text>().text = tempSkill.CName;
            button.GetComponentInChildren<Text>().resizeTextForBestFit = false;
            button.GetComponentInChildren<Text>().fontSize = 45;
            button.GetComponentInChildren<Text>().GetComponent<RectTransform>().sizeDelta = new Vector2(-30, 0);
            button.name = skill.Key;




            button.GetComponent<Button>().onClick.AddListener(OnButtonClick);




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
            if(tempSkill is UnitSkill)
            {
                var tempUnitSkill = (UnitSkill)tempSkill;
                _Class.sprite = imagesList.Find(i => i.name.Substring(11) == tempUnitSkill.skillClass.ToString());
                _Type.sprite = imagesList.Find(i => i.name.Substring(10) == tempUnitSkill.skillType.ToString());
                _Combo.gameObject.SetActive(tempUnitSkill.comboType != UnitSkill.ComboType.cannot);
            }
            else
            {
                _Class.sprite = imagesList.Find(i => i.name.Substring(11) == UnitSkill.SkillClass.passive.ToString());
                _Type.gameObject.SetActive(false);
                _Combo.gameObject.SetActive(false);
            }
            var levelUI = UnityEngine.Object.Instantiate(_SkillLevelImages, button.transform);
            for(int i = 0;tempSkill.maxLevel > i; i++)
            {
                var toggle = levelUI.transform.Find("Level" + (i + 1).ToString()).gameObject;
                toggle.SetActive(true);
                if (skill.Value > i)
                {
                    toggle.GetComponent<Toggle>().isOn = true;
                }
            }

        }

        UIContent.GetComponent<RectTransform>().sizeDelta = new Vector2(UIContent.GetComponent<RectTransform>().sizeDelta.x, allButtons[0].GetComponent<RectTransform>().sizeDelta.y * (allButtons.Count));

        //设置按钮位置
        for (int i = 0; i < allButtons.Count; i++)
        {
            allButtons[i].transform.localPosition = new Vector3(allButtons[i].transform.localPosition.x, -(int)(i * (allButtons[i].GetComponent<RectTransform>().sizeDelta.y)), 0);
        }
    }

    private void OnButtonClick()
    {
        var btn = EventSystem.current.currentSelectedGameObject;
        LevelUp(btn.name);
    }


    public void LevelUp(string skillName)
    {
        var CS = Controller_Main.GetInstance().character.GetComponent<CharacterStatus>();
        var XM = XMLManager.GetInstance();
        var tempSkill = SkillManager.GetInstance().skillList.Find(s => s.EName == skillName);
        if (CS.skills[skillName] < tempSkill.maxLevel)
        {
            CS.skills[skillName]++;
            XM.characterDB.characterDataList.Find(c => c.roleEName == Controller_Main.GetInstance().character.GetComponent<CharacterStatus>().roleEName).skills.Find(s => s.skillName == skillName).skillLevel++;
            XM.SaveCharacters();
            CreateSkillList(Controller_Main.GetInstance().character);
        }
    }

    public void LevelDown()
    {

    }

    public void Clear(object sender, EventArgs e)
    {
        foreach(var b in allButtons)
        {
            Destroy(b);
        }
        gameObject.SetActive(false);
    }
}
