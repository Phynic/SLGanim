using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Control All AI Unit Actions
/// </summary>
public class AIManager : MonoBehaviour {
    enum StrategyType {
        attack,
        defend,
        heal,
        doNothing
    }

    private static AIManager instance;
    private RTSCamera rtsCamera; //rtsCamera will follow the aiUnit or its skill
    private RenderBlurOutline outline; //render unit outline
    private Unit aiUnit; //current control which ai unit
    private Unit aiTarget; //current target of aiUnit
    private StrategyType strategy; //use which strategy to deal with enemy

    #region Mono
    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        rtsCamera = Camera.main.GetComponent<RTSCamera>();
        outline = Camera.main.GetComponent<RenderBlurOutline>();
    }

    public static AIManager GetInstance()
    {
        return instance;
    }
    #endregion

    #region Decision
    /// <summary>
    /// start up whole AI System externally 
    /// </summary>
    public IEnumerator activeAI(Unit _auUnit) {
        aiUnit = _auUnit;
        actionJudgement();
        yield return StartCoroutine(doAction());
    }

    private void actionJudgement()
    {
        strategy = StrategyType.attack;
    }

    IEnumerator doAction() {
        switch (strategy)
        {
            case StrategyType.attack:
                yield return StartCoroutine(Attack());
                break;
            case StrategyType.defend:
                break;
            case StrategyType.heal:
                break;
            case StrategyType.doNothing:
                break;
        }
        yield return 0;
    }

    IEnumerator Attack() {
        yield return StartCoroutine(seekTarget()); //find target
        yield return StartCoroutine(moveAI(aiUnit,aiTarget)); //close the target
        yield return StartCoroutine(useUnitSkillAI("NinjaCombo", aiUnit,aiTarget)); //attack target with skill
        yield return StartCoroutine(turnToAI("forward")); //turn to best orientation
    }
    #endregion

    #region Detail Actions
    IEnumerator seekTarget() { 
        //this is a temporary method for test
        //find the nearest enemy unit as ai target
        List<Unit> nonMyUnitList = UnitManager.GetInstance().units.FindAll(p => p.playerNumber != aiUnit.playerNumber);
        int nearestIdx = 0;
        float disMin = 9999;
        for (int i = 0; i < nonMyUnitList.Count; ++i)
        {
            float distance = Vector3.Distance(aiUnit.transform.position, nonMyUnitList[i].transform.position);
            if (disMin > distance)
            {
                disMin = distance;
                nearestIdx = i;
            }
        }
        Unit nearUnit = nonMyUnitList[nearestIdx];
        //Debug.Log("NearPlayer Name is=>" + nearUnit.name);

        aiTarget = nearUnit;
        yield return 1;
    }

    IEnumerator moveAI(Unit aiUnit, Unit targetUnit)
    {
        //if the targetUnit is next to aiUnit, don't need to move
        if (isNextMe(aiUnit, targetUnit))
        {
            Debug.Log(aiUnit.name + " don't need to move");
            yield return 0;
        }
        else
        {
            //auto move forward nearPlayer
            aiUnit.GetComponent<CharacterAction>().SetSkill("Move");
            Move moveSkill = SkillManager.GetInstance().skillQueue.Peek().Key as Move;
            var f1 = targetUnit.transform.position + new Vector3(1, 0, 0);

            moveSkill.Init(aiUnit.transform);

            GameObject floor = BattleFieldManager.GetInstance().GetFloor(f1);
            moveSkill.Focus(floor.GetComponent<Floor>());
            yield return new WaitForSeconds(0.5f);

            outline.CancelRender();
            moveSkill.Confirm();
            yield return new WaitUntil(() => { return moveSkill.skillState == Skill.SkillState.reset; });

        }
    }

    IEnumerator useUnitSkillAI(string skillName, Unit aiUnit, Unit target)
    {
        yield return StartCoroutine(turnToAI(aiUnit, target));

        outline.RenderOutLine(aiUnit.transform);

        bool isSuccess = aiUnit.GetComponent<CharacterAction>().SetSkill(skillName);
        //Debug.Log("useUnitSkill=>" + skillName + "=>" + isSuccess);

        UnitSkill unitSkill = SkillManager.GetInstance().skillQueue.Peek().Key as UnitSkill;
        rtsCamera.FollowTarget(target.transform.position);
        yield return new WaitForSeconds(0.5f);
        unitSkill.Init(aiUnit.transform);
        unitSkill.Focus(target.transform.position);

        yield return new WaitForSeconds(0.5f);

        outline.CancelRender();
        unitSkill.Confirm();
        yield return new WaitUntil(() => { return unitSkill.complete == true; });
        rtsCamera.FollowTarget(aiUnit.transform.position);
        yield return 0;
    }

    IEnumerator turnToAI(string orientation)
    {
        ChooseDirection chooseDirection = SkillManager.GetInstance().skillQueue.Peek().Key as ChooseDirection;
        yield return null;
        chooseDirection.OnArrowHovered(orientation);
        yield return new WaitForSeconds(1f);
        chooseDirection.Confirm_AI();
        yield return 0;
    }

    IEnumerator turnToAI(Unit aiUnit, Unit target)
    {
        aiUnit.transform.LookAt(target.transform);
        yield return new WaitForSeconds(0.2f);
    }
    #endregion

    #region Other Functions
    /// <summary>
    /// caculate all of coordinates of floors around
    /// </summary>
    private List<Vector3> getNextFloors(Vector3 floor) {
        List<Vector3> result = new List<Vector3>(4);
        //the orientation points to top right conner is forward
        Vector3 forward = floor + new Vector3(-1, 0, 0);
        Vector3 back = floor + new Vector3(1, 0, 0);
        Vector3 left = floor + new Vector3(0, 0, -1);
        Vector3 right = floor + new Vector3(1, 0, 1);
        result.Add(forward);
        result.Add(back);
        result.Add(left);
        result.Add(right);
        return result;
    }

    /// <summary>
    /// detect whether other is next to me
    /// </summary>
    private bool isNextMe(Unit me, Unit other)
    {
        List<Vector3> aroundMe = getNextFloors(me.transform.position);
        if (aroundMe.Contains(other.transform.position))
            return true;
        else
            return false;
    }
    #endregion
}
