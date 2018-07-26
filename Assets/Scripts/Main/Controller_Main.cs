using System;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine;

public class Controller_Main : MonoBehaviour {
    private static Controller_Main instance;
    public event EventHandler ClearUI;
    public event EventHandler UnitSelected;
    [HideInInspector]
    public Transform character;
    public SkillMenu skillMenu;
    public BaseInfo baseInfo;
    public Transform mainMenu;
    public ScreenFader screenFader;

    public static Controller_Main GetInstance()
    {
        return instance;
    }

    void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        UnitManager.GetInstance().units.ForEach(u => u.GetComponent<Unit>().UnitClicked += OnUnitClicked);
        
        UnitSelected += baseInfo.UpdateView;
        ClearUI += baseInfo.Clear;
        ClearUI += skillMenu.Clear;
    }

    private void OnUnitClicked(object sender, EventArgs e)
    {
        character = (sender as Unit).transform;
        
        var outline = Camera.main.GetComponent<RenderBlurOutline>();
        if (outline)
            outline.RenderOutLine(character);

        if (UnitSelected != null)
            UnitSelected.Invoke(this, new EventArgs());
        mainMenu.gameObject.SetActive(false);
    }
    
    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            var outline = Camera.main.GetComponent<RenderBlurOutline>();
            if (outline)
                outline.CancelRender();
            if (ClearUI != null)
                ClearUI.Invoke(this, new EventArgs());
            mainMenu.gameObject.SetActive(true);
        }
    }

    public void NextScene()
    {
        screenFader.FadeOut(() => {
            SceneManager.LoadScene("Loading");
        }, true);
    }
}
