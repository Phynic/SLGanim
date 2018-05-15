using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// AI will fight with Human Player without drama. We call this mode free battle mode.
/// </summary>
public class AIFreeBattle : MonoBehaviour {
    //enum StrategyType
    //{
    //    attack,
    //    defend,
    //    heal,
    //    doNothing
    //}

    private Unit aiUnit; //current control which ai unit
    //private StrategyType strategy; //use which strategy to deal with enemy
    private AITree aiTree; //decide strategy to deal with enemy

    #region Decision
    /// <summary>
    /// start up whole AI System externally 
    /// </summary>
    public IEnumerator ActiveAI(Unit _aiUnit)
    {
        aiUnit = _aiUnit;

        ChooseBaseStrategy();
        yield return StartCoroutine(DoAction());
    }

    private void ChooseBaseStrategy()
    {
        //strategy = StrategyType.attack;
        aiTree = AITreeDefend.inst;
    }

    IEnumerator DoAction()
    {
        yield return StartCoroutine(aiTree.ActivateAITree(aiUnit));
        //switch (strategy)
        //{
        //    case StrategyType.attack:
        //        yield return StartCoroutine(Attack());
        //        break;
        //    case StrategyType.defend:
        //        break;
        //    case StrategyType.heal:
        //        break;
        //    case StrategyType.doNothing:
        //        break;
        //}
        //yield return 0;
    }

    //IEnumerator Attack()
    //{
    //    yield return StartCoroutine(decideMoveTarget(GetAttackMovePosition)); //find target
    //    yield return StartCoroutine(moveAI(aiUnit, moveTarget)); //close the target

    //    //if enemy is out of the attack range, aiUnit won't attack and just near to his enemy 
    //    if (strategy == StrategyType.attack)
    //    {
    //        yield return StartCoroutine(decideSkill());
    //        yield return StartCoroutine(dramaYellSkill(aiUnit, skillName)); //yell skill Name
    //        yield return StartCoroutine(useUnitSkillAI(skillName, aiUnit, aiTarget)); //attack target with skill
    //    }

    //    yield return StartCoroutine(turnToAI("forward")); //turn to best orientation
    //}

    //private IEnumerator decideSkill()
    //{
    //    skillName = "NinjaCombo";
    //    yield return 0;
    //}

    //private IEnumerator dramaYellSkill(Unit aiUnit,string skillName)
    //{
    //    DramaYellSkill dys = AIManager.GetInstance().AIDrama.GetComponent<DramaYellSkill>();
    //    dys.skillName = skillName;
    //    dys.unit = aiUnit;
    //    yield return StartCoroutine(dys.Play());
    //}

    //IEnumerator decideMoveTarget(DecideMoveTarget moveCallback)
    //{
    //    moveCallback();
    //    yield return 0;
    //}

    //private void GetAttackMovePosition()
    //{
    //    //get move range
    //    MoveRange moveRange = new MoveRange();
    //    moveRange.CreateMoveRange(aiUnit.transform);
    //    //find the nearest enemy
    //    getNeareatTarget(aiUnit);
    //    //find if the nearest enemy in attack range
    //    List<Vector3> enemyFloor = moveRange.enemyFloor;
    //    List<Unit> enemyList=new List<Unit>(); //get all of enemies of current ai unit
    //    foreach (Vector3 v in enemyFloor)
    //    {
    //        enemyList.Add(getEnemyUnit(v));
    //    }

    //    if (enemyList.Contains(aiTarget))
    //    { //if the enemy is in attack range of aiUnit
    //        //choose a floor in moveRange and it is the neareast from aiUnit to aiTarget
    //        //get the four floors next aiTarget
    //        List<Vector3> nextFloors = getNextFloors(aiTarget.transform.position);
            
    //        //just leave these floors that could be reached in moveRange
    //        List<Vector3> canReachNextFloors = nextFloors.FindAll(nf => moveRange.rangeDic.ContainsKey(nf));
            
    //        //detect if there are other team mates in these floors
    //        List<Unit> teamMatesList = UnitManager.GetInstance().units.FindAll(p => p.playerNumber == aiUnit.playerNumber);
    //        List<Vector3> availableFloors = new List<Vector3>(canReachNextFloors);
    //        foreach (Unit u in teamMatesList) {
    //            foreach (Vector3 v in canReachNextFloors) {
    //                if (u.transform.position == v)
    //                    availableFloors.Remove(v);
    //            }
    //        }

    //        if (availableFloors.Count > 0)
    //        {
    //            //find the nearest floor
    //            moveTarget = getNeareastFloor(aiUnit, availableFloors);
    //        }
    //        else {
    //            if (remoteAttackDetect()) 
    //                remoteAttack();
    //            else
    //                moveMoreStep(moveRange);

    //        }
    //        //cancel render moveRange
    //        moveRange.Delete();
    //    }
    //    else {
    //        moveMoreStep(moveRange);
    //    }
    //}
    //#endregion

    //#region Detail Actions
    //private void remoteAttack() {

    //}

    ///// <summary>
    ///// if there has been 4 team mates around target, aiUnit should remote attack detection,
    ///// if remote attack detection fails too,he should think about other enemies to attack.
    ///// </summary>
    //private bool remoteAttackDetect() {
    //    return false;
    //}

    ///// <summary>
    ///// if the aiUnit can't reach enemies in one round, he should choose more round to complete attack
    ///// the move target should not only be the neareast from the neareast enemy
    ///// but also in move range
    ///// </summary>
    //private void moveMoreStep(MoveRange moveRange) {
    //    List<Vector3> availableFloors = new List<Vector3>();
    //    foreach (KeyValuePair<Vector3, GameObject> kp in moveRange.rangeDic) {
    //        availableFloors.Add(kp.Key);
    //    }
    //    moveTarget = getNeareastFloor(aiTarget, availableFloors);
    //    strategy = StrategyType.doNothing;
    //}

    //private Unit getEnemyUnit(Vector3 enemyFloor) {
    //    List<Unit> enemyUnitList = UnitManager.GetInstance().units.FindAll(p => p.playerNumber != aiUnit.playerNumber);
    //    return enemyUnitList.Find(eu => eu.transform.position == enemyFloor);
    //}

    ///// <summary>
    ///// get the neareast floor position from unit among floorList
    ///// </summary>
    //private Vector3 getNeareastFloor(Unit unit,List<Vector3> floorList) {
    //    int nearestIdx = 0;
    //    float disMin = 9999;
    //    for (int i = 0; i < floorList.Count; ++i)
    //    {
    //        float distance = Vector3.Distance(unit.transform.position, floorList[i]);
    //        if (disMin > distance)
    //        {
    //            disMin = distance;
    //            nearestIdx = i;
    //        }
    //    }
    //    return floorList[nearestIdx];
    //}

    

    //IEnumerator moveAI(Unit aiUnit, Vector3 targetFloor)
    //{
    //    //target position is as same as aiUnit, don't need to move
    //    if (aiUnit.transform.position == targetFloor)
    //    {
    //        Debug.Log(aiUnit.name + " don't need to move");
    //        yield return 0;
    //    }
    //    else
    //    {
    //        //auto move forward target
    //        aiUnit.GetComponent<CharacterAction>().SetSkill("Move");
    //        Move moveSkill = SkillManager.GetInstance().skillQueue.Peek().Key as Move;

    //        moveSkill.Init(aiUnit.transform);

    //        GameObject floor = BattleFieldManager.GetInstance().GetFloor(targetFloor);
    //        moveSkill.Focus(floor.GetComponent<Floor>());
    //        yield return new WaitForSeconds(0.5f);

    //        outline.CancelRender();
    //        moveSkill.Confirm();
    //        yield return new WaitUntil(() => { return moveSkill.skillState == Skill.SkillState.reset; });
    //        yield return new WaitForSeconds(0.1f);
    //    }
    //}

    //IEnumerator useUnitSkillAI(string skillName, Unit aiUnit, Unit target)
    //{

    //    yield return StartCoroutine(turnToAI(aiUnit, target));

    //    outline.RenderOutLine(aiUnit.transform);

    //    bool isSuccess = aiUnit.GetComponent<CharacterAction>().SetSkill(skillName);
    //    //Debug.Log("useUnitSkill=>" + skillName + "=>" + isSuccess);

    //    UnitSkill unitSkill = SkillManager.GetInstance().skillQueue.Peek().Key as UnitSkill;
    //    rtsCamera.FollowTarget(target.transform.position);
    //    yield return new WaitForSeconds(0.5f);
    //    unitSkill.Init(aiUnit.transform);
    //    unitSkill.Focus(target.transform.position);

    //    yield return new WaitForSeconds(0.5f);

    //    outline.CancelRender();
    //    unitSkill.Confirm();
    //    yield return new WaitUntil(() => { return unitSkill.complete == true; });
    //    rtsCamera.FollowTarget(aiUnit.transform.position);
    //    yield return 0;
    //}

    //IEnumerator turnToAI(string orientation)
    //{
    //    if(SkillManager.GetInstance().skillQueue.Count==0)
    //        aiUnit.GetComponent<CharacterAction>().SetSkill("ChooseDirection"); //for more move step

    //    ChooseDirection chooseDirection = SkillManager.GetInstance().skillQueue.Peek().Key as ChooseDirection;
    //    yield return 0;
    //    chooseDirection.OnArrowHovered(orientation);
    //    yield return new WaitForSeconds(1f);
    //    chooseDirection.Confirm_AI();
    //    yield return 0;
    //}

    //IEnumerator turnToAI(Unit aiUnit, Unit target)
    //{
    //    aiUnit.transform.LookAt(target.transform);
    //    yield return new WaitForSeconds(0.2f);
    //}
    //#endregion

    //#region Other Functions
    ///// <summary>
    ///// caculate all of coordinates of floors around
    ///// </summary>
    //private List<Vector3> getNextFloors(Vector3 floor)
    //{
    //    List<Vector3> result = new List<Vector3>(4);
    //    //the orientation points to top right conner is forward
    //    Vector3 forward = floor + new Vector3(-1, 0, 0);
    //    Vector3 back = floor + new Vector3(1, 0, 0);
    //    Vector3 left = floor + new Vector3(0, 0, -1);
    //    Vector3 right = floor + new Vector3(0, 0, 1);
    //    result.Add(forward);
    //    result.Add(back);
    //    result.Add(left);
    //    result.Add(right);
    //    return result;
    //}

    ///// <summary>
    ///// detect whether other is next to me
    ///// </summary>
    //private bool isNextMe(Unit me, Unit other)
    //{
    //    List<Vector3> aroundMe = getNextFloors(me.transform.position);
    //    if (aroundMe.Contains(other.transform.position))
    //        return true;
    //    else
    //        return false;
    //}
    //private bool isNextMe(Unit me, Vector3 floor)
    //{
    //    List<Vector3> aroundMe = getNextFloors(me.transform.position);
    //    if (aroundMe.Contains(floor))
    //        return true;
    //    else
    //        return false;
    //}
    #endregion
}
