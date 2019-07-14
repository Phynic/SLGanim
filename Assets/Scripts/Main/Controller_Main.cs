using System;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Controller_Main : SceneSingleton<Controller_Main>
{
    public event EventHandler ClearUI;
    public event EventHandler UnitSelected;
    [HideInInspector]
    public Transform character;
    public BaseInfo baseInfo;
    public ItemMenu itemMenu;
    public Transform mainMenu;
    public MaskView screenFader;
    public List<Sprite> headShots = new List<Sprite>();

    private GameObject confirmUI;
    private SkillMenu skillMenu;
    private ItemMenu_Role itemMenu_Role;
    private RoleInfo roleInfo;
    private void Start()
    {
        skillMenu = baseInfo.transform.Find("SkillMenu").GetComponent<SkillMenu>();
        itemMenu_Role = baseInfo.transform.Find("ItemMenu_Role").GetComponent<ItemMenu_Role>();
        roleInfo = baseInfo.transform.Find("RoleInfo").GetComponent<RoleInfo>();

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
        UnitManager.GetInstance().InitUnits();
        UnitManager.GetInstance().units.ForEach(u => u.GetComponent<Unit>().UnitClicked += OnUnitClicked);
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
        if (mainMenu != null)
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
        if (confirmUI)
            Destroy(confirmUI);
        if (outline)
            outline.CancelRender();
        if (ClearUI != null)
            ClearUI.Invoke(this, new EventArgs());
        if (mainMenu != null)
            mainMenu.gameObject.SetActive(true);
    }

    private void BackSpace(object sender, EventArgs e)
    {
        BackSpace();
    }

    public void NextScene()
    {
        GameController.GetInstance().ChangeProcedure<Procedure_Gal>();
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

    private void OnDestroy()
    {
        ClearUI = null;
        UnitSelected = null;
    }
}
