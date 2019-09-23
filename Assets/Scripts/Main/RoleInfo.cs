using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoleInfo : MonoBehaviour {
    public void UpdateView()
    {
        CreateRoleInfo(Controller_Main.GetInstance().character);
    }

    public void CreateRoleInfo(Transform character)
    {
        gameObject.SetActive(true);
        var roleEName = character.GetComponent<CharacterStatus>().roleEName;
        var data = GameManager.GetInstance().characterDB.characterDataList.Find(c => c.roleEName == roleEName);
        

        //头像、经验值、体力、查克拉、忍具数、攻击力、防御力、敏捷度、移动力、印。
        var image = transform.Find("Image").GetComponent<Image>();
        var expInfo = transform.Find("ExpInfo").GetComponent<Text>();
        var hpInfo = transform.Find("HpInfo").GetComponent<Text>();
        var mpInfo = transform.Find("MpInfo").GetComponent<Text>();
        var itemNumInfo = transform.Find("ItemNumInfo").GetComponent<Text>();
        var atkInfo = transform.Find("AtkInfo").GetComponent<Text>();
        var defInfo = transform.Find("DefInfo").GetComponent<Text>();
        var dexInfo = transform.Find("DexInfo").GetComponent<Text>();
        var mudInfo = transform.Find("MudInfo").GetComponent<Text>();
        var mrgInfo = transform.Find("MrgInfo").GetComponent<Text>();

        var expData = data.attributes.Find(d => d.eName == "exp");
        var hpData = data.attributes.Find(d => d.eName == "hp");
        var mpData = data.attributes.Find(d => d.eName == "mp");
        var itemNumData = data.attributes.Find(d => d.eName == "itemNum");
        var atkData = data.attributes.Find(d => d.eName == "atk");
        var defData = data.attributes.Find(d => d.eName == "def");
        var dexData = data.attributes.Find(d => d.eName == "dex");
        var mudData = data.attributes.Find(d => d.eName == "mud");
        var mrgData = data.attributes.Find(d => d.eName == "mrg");

        image.sprite = Controller_Main.GetInstance().headShots.Find(s => s.name == roleEName.ToLower());

        expInfo.text = expData.value + " / " + expData.valueMax;
        hpInfo.text = hpData.value + " / " + (hpData.valueMax - hpData.bonus) + " (" + "+" + hpData.bonus + ")";
        mpInfo.text = mpData.value + " / " + (mpData.valueMax - mpData.bonus) + " (" + "+" + mpData.bonus + ")";
        //itemNum的value和valueMax与其他不同，再思考
        itemNumInfo.text = data.items.Count + " / " + (itemNumData.value - itemNumData.bonus) + " (" + "+" + itemNumData.bonus + ")";
        atkInfo.text = atkData.value + " (" + "+" + atkData.bonus + ")";
        defInfo.text = defData.value + " (" + "+" + defData.bonus + ")";
        dexInfo.text = dexData.value + " (" + "+" + dexData.bonus + ")";
        mudInfo.text = mudData.value + " (" + "+" + mudData.bonus + ")";
        mrgInfo.text = mrgData.value + " (" + "+" + mrgData.bonus + ")";
    }

    public void Clear(object sender, EventArgs e)
    {
        gameObject.SetActive(false);
    }
}
