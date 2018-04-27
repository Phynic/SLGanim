using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DramaBattle01 : SceneDrama
{
    RTSCamera rtsCamera;
    RenderBlurOutline outline;

    void Start()
    {
        rtsCamera = Camera.main.GetComponent<RTSCamera>();
        outline = Camera.main.GetComponent<RenderBlurOutline>();
    }

    public override IEnumerator Play()
    {
        yield return StartCoroutine(jiroubouDrama());
        yield return StartCoroutine(rockDrama());
        RoundManager.GetInstance().EndTurn();
    }

    private IEnumerator jiroubouDrama()
    {
        Unit u = UnitManager.GetInstance().units.Find(p => p.name == "jiroubou_1");

        rtsCamera.FollowTarget(u.transform.position);
        outline.RenderOutLine(u.transform);
        
        yield return StartCoroutine(UseSkill("EarthStyleDorodomuBarrier", u.transform));
        u.OnUnitEnd();   //真正的回合结束所应执行的逻辑。
        DebugLogPanel.GetInstance().Log(u.GetComponent<CharacterStatus>().roleCName + "执行完毕");
        yield return new WaitForSeconds(1f);
        
        outline.CancelRender();

        yield return 0;
    }

    private IEnumerator UseSkill(string skillName, Transform character)
    {
        yield return new WaitForSeconds(1.5f);
        character.GetComponent<CharacterAction>().SetSkill(skillName);
        var f = new Vector3(40.5f, 0, 34.5f);
        UnitSkill unitSkill = SkillManager.GetInstance().skillQueue.Peek().Key as UnitSkill;
        rtsCamera.FollowTarget(f);
        yield return new WaitForSeconds(0.5f);
        unitSkill.Focus(f);

        yield return new WaitForSeconds(0.5f);
        unitSkill.Confirm();
        yield return new WaitUntil(() => { return unitSkill.complete == true; });
        rtsCamera.FollowTarget(character.position);
        ChooseDirection chooseDirection = SkillManager.GetInstance().skillQueue.Peek().Key as ChooseDirection;
        yield return null;
        chooseDirection.OnArrowHovered("right");
        yield return new WaitForSeconds(1f);
        chooseDirection.Confirm_AI();

    }

    private IEnumerator rockDrama() {
        var rockUnits = UnitManager.GetInstance().units.FindAll(p => p.GetComponent<CharacterStatus>().roleEName == "Rock");

        foreach (var u in rockUnits)
        {
            if (u.GetComponent<Unit>().UnitEnd)
                break;
            outline.RenderOutLine(u.transform);
            rtsCamera.FollowTarget(u.transform.position);

            //rock auto recovers
            var currentHp = u.GetComponent<CharacterStatus>().attributes.Find(d => d.eName == "hp").value;
            var currentHPMax = u.GetComponent<CharacterStatus>().attributes.Find(d => d.eName == "hp").valueMax;
            var restValue = (int)(currentHPMax * 0.3f);
            //if the recover HP makes currentHp full, then Hp gets full
            //else just recovers currentHPMax * 0.3f HP
            restValue = currentHp + restValue > currentHPMax ? currentHPMax - currentHp : restValue;

            var hp = currentHp + restValue;

            UIManager.GetInstance().FlyNum(u.GetComponent<CharacterStatus>().arrowPosition / 2 + u.transform.position + Vector3.down * 0.2f, restValue.ToString(), UIManager.hpColor);

            ChangeData.ChangeValue(u.transform, "hp", hp);

            u.OnUnitEnd();   //真正的回合结束所应执行的逻辑。
            DebugLogPanel.GetInstance().Log(u.GetComponent<CharacterStatus>().roleCName + "执行完毕");
            yield return new WaitForSeconds(1f);
        }   
        
        yield return 0;
    }
}

