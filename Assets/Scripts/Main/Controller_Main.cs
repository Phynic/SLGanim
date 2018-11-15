using System;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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
    private GameObject confirmUI;
    private void Start()
    {
        
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

    public void OnUnitClicked(object sender, EventArgs e)
    {
        character = (sender as Unit).transform;

        var outline = Camera.main.GetComponent<RenderBlurOutline>();
        if (outline)
            outline.RenderOutLine(character);

        if (UnitSelected != null)
            UnitSelected.Invoke(this, new EventArgs());
        mainMenu.gameObject.SetActive(false);
        itemMenu.gameObject.SetActive(false);
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

    public void BackSpace()
    {
        var outline = Camera.main.GetComponent<RenderBlurOutline>();
        if(confirmUI)
            Destroy(confirmUI);
        if (outline)
            outline.CancelRender();
        if (ClearUI != null)
            ClearUI.Invoke(this, new EventArgs());
        mainMenu.gameObject.SetActive(true);
    }

    private void BackSpace(object sender, EventArgs e)
    {
        BackSpace();
    }

    public void NextScene()
    {
        screenFader.FadeOut(() => {
            Global.GetInstance().NextScene("_Battle");
        }, true);
    }

    public void ShowConfirm()
    {
        if (confirmUI != null)
            return;
        var go = (GameObject)Resources.Load("Prefabs/UI/Confirm");
        confirmUI = Instantiate(go, GameObject.Find("Canvas").transform);
        confirmUI.transform.Find("Return").GetComponent<Button>().onClick.AddListener(() => { Destroy(confirmUI); });
        confirmUI.transform.Find("Confirm").GetComponent<Button>().onClick.AddListener(() => { RoundManager.GetInstance().BattleBegin = true; Destroy(confirmUI); });
    }

    public void EndBattlePrepare()
    {
        UnitManager.GetInstance().units.ForEach(u => u.GetComponent<Unit>().UnitClicked -= OnUnitClicked);
    }
}
