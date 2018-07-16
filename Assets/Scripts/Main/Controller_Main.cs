using System;
using System.Collections.Generic;
using UnityEngine;

public class Controller_Main : MonoBehaviour {
    private static Controller_Main instance;
    public event EventHandler ClearUI;
    public event EventHandler UnitSelected;
    public Transform character;
    public SkillMenu skillMenu;
    public BaseInfo baseInfo;

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
    }

    private void OnUnitClicked(object sender, EventArgs e)
    {
        character = (sender as Unit).transform;
        Camera.main.GetComponent<RenderBlurOutline>().RenderOutLine(character);
        if (UnitSelected != null)
            UnitSelected.Invoke(this, new EventArgs());
    }
    
    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Camera.main.GetComponent<RenderBlurOutline>().CancelRender();
            if (ClearUI != null)
                ClearUI.Invoke(this, new EventArgs());
        }
    }
}
