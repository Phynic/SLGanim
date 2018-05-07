using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DramaYellSkill : UnitDrama {
    public GameObject skillNamePF;
    public string skillName;
    public Unit unit;

    private Transform skillNamePanel;

    void Start() {
        skillNamePanel = GameObject.Find("Canvas/SkillNamePanel").transform;
    }

    public override IEnumerator Play()
    {
        yield return StartCoroutine(yellSkillName());
    }

    private IEnumerator yellSkillName()
    {
        GameObject snpf = Instantiate(skillNamePF);
        snpf.transform.SetParent(skillNamePanel);
        snpf.GetComponent<Text>().text = getSkillChName(skillName)+"!";
        //the position of display should on the left top of unit
        snpf.GetComponent<TextFlyAboveAnim>().unitPosition = unit.transform.position;
        snpf.GetComponent<TextFlyAboveAnim>().doAnim();
        yield return new WaitForSeconds(1);
        
    }

    private string getSkillChName(string skillEnName) {
        Skill skill = SkillManager.GetInstance().skillList.Find(s => s.EName == skillEnName);
        return skill.CName;
    } 
}
