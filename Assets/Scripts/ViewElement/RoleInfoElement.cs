using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoleInfoElement : MonoBehaviour
{
    private Text roleName;
    private Text roleIdentity;
    private Text roleState;
    private Slider healthSlider;
    private Slider chakraSlider;
    private Text info;
    public void Init(Transform character)
    {
        roleName = transform.Find("RoleName").GetComponent<Text>();
        roleIdentity = transform.Find("RoleIdentity").GetComponent<Text>();
        roleState = transform.Find("RoleState").GetComponent<Text>();
        healthSlider = transform.Find("Health").GetComponent<Slider>();
        chakraSlider = transform.Find("Chakra").GetComponent<Slider>();
        info = transform.Find("Info").GetComponent<Text>();

        var characterStatus = character.GetComponent<CharacterStatus>();

        roleName.text = characterStatus.roleCName.Replace(" ", "");
        roleIdentity.text = characterStatus.identity;
        roleState.text = characterStatus.UnitEnd ? "结束" : "待机";
        roleState.color = characterStatus.UnitEnd ? Utils_Color.redTextColor : Utils_Color.purpleTextColor;
        healthSlider.maxValue = characterStatus.attributes.Find(d => d.eName == "hp").ValueMax;
        healthSlider.value = characterStatus.attributes.Find(d => d.eName == "hp").Value;
        chakraSlider.maxValue = characterStatus.attributes.Find(d => d.eName == "mp").ValueMax;
        chakraSlider.value = characterStatus.attributes.Find(d => d.eName == "mp").Value;
        info.text = healthSlider.GetComponent<Slider>().value + "\n" + chakraSlider.GetComponent<Slider>().value;
    }
}
