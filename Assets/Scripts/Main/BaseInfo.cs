using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaseInfo : MonoBehaviour {

    public Text roleName;
    public Text roleLevel;
    public Text roleSkillPointInfo;
    public Text info;
    public Slider experience;
    public Transform roleMenu;
    public Transform skillMenu;
    public Transform itemMenu_Role;

    public void SyncRoleMenu()
    {
        if (roleMenu.gameObject.activeSelf)
            roleMenu.gameObject.SetActive(false);
        else
            roleMenu.gameObject.SetActive(true);
    }

    public void UpdateView(object sender, EventArgs e)
    {
        gameObject.SetActive(true);
        CreateBaseInfo(Controller_Main.GetInstance().character);
        if (skillMenu.gameObject.activeSelf)
            skillMenu.GetComponent<SkillMenu>().UpdateView();
        if (itemMenu_Role.gameObject.activeSelf)
            itemMenu_Role.GetComponent<ItemMenu_Role>().UpdateView();
    }
    
    public void CreateBaseInfo(Transform character)
    {
        var CS = character.GetComponent<CharacterStatus>();
        roleName.text = CS.roleCName;
        roleLevel.text = "Lv " + CS.attributes.Find(d => d.eName == "lev").value.ToString();
        roleSkillPointInfo.text = CS.attributes.Find(d => d.eName == "skp").value.ToString();
        var currentHP = character.GetComponent<CharacterStatus>().attributes.Find(d => d.eName == "hp").value;
        var currentMP = character.GetComponent<CharacterStatus>().attributes.Find(d => d.eName == "mp").value;
        info.text = currentHP + "\n" + currentMP;
        experience.maxValue = CS.attributes.Find(d => d.eName == "exp").valueMax;
        experience.value = CS.attributes.Find(d => d.eName == "exp").value;
    }

    public void Clear(object sender, EventArgs e)
    {
        transform.Find("RoleMenu").gameObject.SetActive(false);
        gameObject.SetActive(false);
    }
}
