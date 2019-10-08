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
    private SkillMenu skillMenu;
    private ItemMenu_Role itemMenu_Role;
    private RoleInfo roleInfo;
    private Transform roleMenu;
    public void SyncRoleMenu()
    {
        skillMenu.gameObject.SetActive(false);
        itemMenu_Role.gameObject.SetActive(false);
        roleInfo.gameObject.SetActive(false);
        if (roleMenu.gameObject.activeSelf)
            roleMenu.gameObject.SetActive(false);
        else
            roleMenu.gameObject.SetActive(true);
    }

    public void UpdateView(Transform character)
    {
        itemMenu_Role = transform.Find("ItemMenu_Role").GetComponent<ItemMenu_Role>();
        skillMenu = transform.Find("SkillMenu").GetComponent<SkillMenu>();
        roleInfo = transform.Find("RoleInfo").GetComponent<RoleInfo>();

        RoleInfoView.TryClose();
        if (BattlePrepareView.isInit && character.GetComponent<CharacterStatus>().playerNumber == Global.playerNumber)
        {
            gameObject.SetActive(true);
            CreateBaseInfo(character);
        }
        else {
            gameObject.SetActive(false);
            //敌方创建RoleInfoView
            RoleInfoView.GetInstance().Open(character);
        }

        var roleMenuButton = transform.Find("RoleMenuButtonBack/RoleMenuButton").GetComponent<Button>();
        roleMenuButton.onClick.RemoveAllListeners();
        roleMenuButton.onClick.AddListener(SyncRoleMenu);
        roleMenu = transform.Find("RoleMenu");
        var roleInfoButton = roleMenu.Find("Content/RoleInfoButton").GetComponent<Button>();
        roleInfoButton.onClick.RemoveAllListeners();
        roleInfoButton.onClick.AddListener(() => { roleInfo.GetComponent<RoleInfo>().UpdateView(character); roleMenu.gameObject.SetActive(false); });
        var ninjaToolsButton = roleMenu.Find("Content/NinjaToolsButton").GetComponent<Button>();
        ninjaToolsButton.onClick.RemoveAllListeners();
        ninjaToolsButton.onClick.AddListener(() => { itemMenu_Role.GetComponent<ItemMenu_Role>().UpdateView(character); roleMenu.gameObject.SetActive(false); });
        var skillMenuButton = roleMenu.Find("Content/SkillMenuButton").GetComponent<Button>();
        skillMenuButton.onClick.RemoveAllListeners();
        skillMenuButton.onClick.AddListener(() => { skillMenu.GetComponent<SkillMenu>().UpdateView(character); roleMenu.gameObject.SetActive(false); });

        if (skillMenu.gameObject.activeSelf)
            skillMenu.GetComponent<SkillMenu>().UpdateView(character);
        if (itemMenu_Role.gameObject.activeSelf)
            itemMenu_Role.GetComponent<ItemMenu_Role>().UpdateView(character);
        if(roleInfo.gameObject.activeSelf)
            roleInfo.GetComponent<RoleInfo>().UpdateView(character);
    }
    
    public void CreateBaseInfo(Transform character)
    {
        var DB = Global.characterDataList.Find(d => d.roleEName == character.GetComponent<CharacterStatus>().roleEName);
        var characterInfo = CharacterInfoDictionary.GetParam(character.GetComponent<CharacterStatus>().characterInfoID);
        roleName.text = DB.roleCName;
        roleLevel.text = "Lv " + DB.attributes.Find(d => d.eName == "lev").Value.ToString();
        roleSkillPointInfo.text = DB.attributes.Find(d => d.eName == "skp").Value.ToString();
        var currentHP = DB.attributes.Find(d => d.eName == "hp").Value;
        var currentMP = DB.attributes.Find(d => d.eName == "mp").Value;
        info.text = currentHP + "\n" + currentMP;
        experience.maxValue = DB.attributes.Find(d => d.eName == "exp").ValueMax;
        experience.value = DB.attributes.Find(d => d.eName == "exp").Value;
    }

    public void Clear(object sender, EventArgs e)
    {
        transform.Find("RoleMenu").gameObject.SetActive(false);
        gameObject.SetActive(false);
        RoleInfoView.TryClose();
    }
}
