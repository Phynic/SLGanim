using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BattlePrepareView : ViewBase<BattlePrepareView>
{
    public UnityAction<Transform> UnitSelected;
    public event EventHandler ClearUI;
    private Transform battleBegin;
    private Button beginButton;
    private Button saveButton;
    private Button loadButton;

    private Transform roleMenu;
    public List<Sprite> headShots = new List<Sprite>();

    
    private BaseInfo baseInfo;
    public ItemMenu itemMenu;

    private GameObject confirmUI;
    private SkillMenu skillMenu;
    private ItemMenu_Role itemMenu_Role;
    private RoleInfo roleInfo;

    public void Open(LevelInfo levelInfo, UnityAction onInit = null)
    {
        if (!isInit)
        {
            battleBegin = transform.Find("BattleBegin");
            beginButton = transform.Find("BattleBegin/Begin").GetComponent<Button>();
            saveButton = transform.Find("BattleBegin/Save").GetComponent<Button>();
            loadButton = transform.Find("BattleBegin/Load").GetComponent<Button>();

            saveButton.onClick.AddListener(() => { SaveLoadView.GetInstance().Open(true); });
            loadButton.onClick.AddListener(() => { SaveLoadView.GetInstance().Open(false); });
            beginButton.onClick.AddListener(ShowConfirm);

            var task = transform.Find("Task");
            task.Find("TaskTitle").GetComponent<Text>().text = levelInfo.taskTitle;
            task.Find("TaskContent").GetComponent<Text>().text = "\n　　" + levelInfo.taskContent + "\n\n" + "胜利条件：\n　　" + levelInfo.vectoryCondition + "\n\n" + "失败条件：\n　　" + levelInfo.failureCondition;

            baseInfo = transform.Find("BaseInfo").GetComponent<BaseInfo>();
            itemMenu = transform.Find("ItemMenu").GetComponent<ItemMenu>();
            //Old
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
#if (!UNITY_EDITOR && (UNITY_IOS || UNITY_ANDROID))
            GameController.GetInstance().TwoTouches += BackSpace;
#endif
            RoundManager.GetInstance().Units.ForEach(u => u.GetComponent<Unit>().UnitClicked += OnUnitClicked);
        }
        base.Open(onInit);
    }

    public void OnUnitClicked(Unit sender)
    {
        var character = sender.transform;
        battleBegin.gameObject.SetActive(false);
        var outline = Camera.main.GetComponent<RenderBlurOutline>();
        if (outline)
            outline.RenderOutLine(character);
        if (UnitSelected != null)
            UnitSelected.Invoke(character);
        itemMenu.gameObject.SetActive(false);
    }

    private void Update()
    {
#if (UNITY_STANDALONE || UNITY_EDITOR)
        if (Input.GetMouseButtonDown(1))
        {
            BackSpace();
        }
#endif
    }

    public void BackSpace()
    {
        var outline = Camera.main.GetComponent<RenderBlurOutline>();
        battleBegin.gameObject.SetActive(true);
        if (confirmUI)
            Destroy(confirmUI);
        if (outline)
            outline.CancelRender();
        if (ClearUI != null)
            ClearUI.Invoke(this, new EventArgs());
    }

    public void ShowConfirm()
    {
        if (confirmUI != null)
            return;
        var go = (GameObject)Resources.Load("Prefabs/UI/Confirm");
        confirmUI = Instantiate(go, GameObject.Find("Canvas").transform);
        confirmUI.transform.Find("Return").GetComponent<Button>().onClick.AddListener(() => { Destroy(confirmUI); });
        confirmUI.transform.Find("Confirm").GetComponent<Button>().onClick.AddListener(() => 
        {
            RoundManager.GetInstance().BattleBegin = true;
            Close();
        });
    }

    public override void Close()
    {
        RoundManager.GetInstance().Units.ForEach(u => u.GetComponent<Unit>().UnitClicked -= OnUnitClicked);
        ClearUI = null;
        UnitSelected = null;
        if (confirmUI)
            Destroy(confirmUI);
        base.Close();
    }
}
