using System;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Controller_Main : Singleton<Controller_Main> {
    public event EventHandler ClearUI;
    public event EventHandler UnitSelected;
    [HideInInspector]
    public Transform character;
    public SkillMenu skillMenu;
    public ItemMenu itemMenu;
    public ItemMenu_Role itemMenu_Role;
    public BaseInfo baseInfo;
    public RoleInfo roleInfo;
    public Transform mainMenu;
    public ScreenFader screenFader;
    public List<Sprite> headShots = new List<Sprite>();
    
    private void Start()
    {
        GameController.GetInstance().Invoke(() =>
        {
            UnitManager.GetInstance().units.ForEach(u => u.GetComponent<Unit>().UnitClicked += OnUnitClicked);
        }, 0.1f);

        UnitSelected += baseInfo.UpdateView;
        ClearUI += baseInfo.Clear;
        ClearUI += skillMenu.Clear;
        ClearUI += itemMenu.Clear;
        ClearUI += itemMenu_Role.Clear;
        ClearUI += roleInfo.Clear;
        var load = Resources.LoadAll<Sprite>("Textures/HeadShots");
        foreach (var p in load)
        {
            headShots.Add(p);
        }

#if (!UNITY_EDITOR && (UNITY_IOS || UNITY_ANDROID))
        GameController.GetInstance().TwoTouches += BackSpace;
#endif
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
#if (UNITY_STANDALONE || UNITY_EDITOR)
        if (Input.GetMouseButtonDown(1))
        {
            BackSpace(this, null);
        }
#endif
    }

    private void BackSpace(object sender, EventArgs e)
    {
        var outline = Camera.main.GetComponent<RenderBlurOutline>();
        if (outline)
            outline.CancelRender();
        if (ClearUI != null)
            ClearUI.Invoke(this, new EventArgs());
        mainMenu.gameObject.SetActive(true);
    }

    public void NextScene()
    {
        screenFader.FadeOut(() => {
            Global.GetInstance().NextScene();
        }, true);
    }
    
    public void EndBattlePrepare()
    {
        UnitManager.GetInstance().units.ForEach(u => u.GetComponent<Unit>().UnitClicked -= OnUnitClicked);

    }
}
