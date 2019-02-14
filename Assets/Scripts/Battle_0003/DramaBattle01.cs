using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DramaBattle01 : SceneDrama
{
    public Transform rocksTransform;
    RTSCamera rtsCamera;
    RenderBlurOutline outline;
    private List<CharacterStatus> rocks = new List<CharacterStatus>();

    void Start()
    {
        rtsCamera = Camera.main.GetComponent<RTSCamera>();
        outline = Camera.main.GetComponent<RenderBlurOutline>();

        var temp = rocksTransform.GetComponentsInChildren<CharacterStatus>();
        foreach (var c in temp)
        {
            rocks.Add(c);
        }

        RoundManager.GetInstance().GameStarted += OnGameStarted;
    }

    //设定岩墙防御
    private void OnGameStarted(object sender, EventArgs e)
    {
        GameController.GetInstance().Invoke(() => {
            foreach (var r in rocks)
            {
                ChangeData.ChangeValue(r.transform, "def", r.attributes.Find(d => d.eName == "def").value + 10 * GetRockIntensity(r.gameObject.name));
            }
        }, 1.5f);
        
    }

    private int GetRockIntensity(string rockName)
    {
        int intensity = 0;
        var temp = Convert.ToInt32(rockName.Substring(5));
        intensity = Mathf.Abs(-temp + 3);
        return intensity;
    }

    public override IEnumerator Play()
    {
        yield return StartCoroutine(JiroubouDrama());
        yield return StartCoroutine(RockDrama());
        RoundManager.GetInstance().EndTurn();
    }

    private IEnumerator JiroubouDrama()
    {
        Unit u = UnitManager.GetInstance().units.Find(p => p.GetComponent<CharacterStatus>().roleEName == "Jiroubou");

        rtsCamera.FollowTarget(u.transform.position);
        if (outline)
            outline.RenderOutLine(u.transform);
        
        yield return StartCoroutine(UseSkill("EarthStyleDorodomuBarrier", u.transform));
        u.OnUnitEnd();   //真正的回合结束所应执行的逻辑。
        DebugLogPanel.GetInstance().Log(u.GetComponent<CharacterStatus>().roleCName + "执行完毕");
        yield return new WaitForSeconds(1f);
        if (outline)
            outline.CancelRender();

        yield return 0;
    }

    private IEnumerator UseSkill(string skillName, Transform character)
    {
        yield return new WaitForSeconds(1.5f);
        character.GetComponent<CharacterAction>().SetSkill(skillName);
        var f = character.position + character.forward * 5;
        f = new Vector3((int)f.x + 0.5f, 0, (int)f.z + 0.5f);
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
        chooseDirection.OnArrowHovered("forward");
        yield return new WaitForSeconds(1f);
        chooseDirection.Confirm_AI();

    }

    private IEnumerator RockDrama() {
        var rockUnits = UnitManager.GetInstance().units.FindAll(p => p.GetComponent<CharacterStatus>().roleEName == "Rock");
        //执行顺序排序
        rockUnits.Sort((x, y) => { return int.Parse(x.name.Substring(5)).CompareTo(int.Parse(y.name.Substring(5))); });

        foreach (var u in rockUnits)
        {
            if (u.GetComponent<Unit>().UnitEnd)
                break;
            if (outline)
                outline.RenderOutLine(u.transform);
            rtsCamera.FollowTarget(u.transform.position);

            //rock auto recovers
            var currentHp = u.GetComponent<CharacterStatus>().attributes.Find(d => d.eName == "hp").value;
            var currentHPMax = u.GetComponent<CharacterStatus>().attributes.Find(d => d.eName == "hp").valueMax;
            var restValue = (int)(currentHPMax * (0.2f + GetRockIntensity(u.gameObject.name) * 0.1f));
            //if the recover HP makes currentHp full, then Hp gets full
            //else just recovers currentHPMax * (0.2f + GetRockIntensity(u.gameObject.name) * 0.1f) HP
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

