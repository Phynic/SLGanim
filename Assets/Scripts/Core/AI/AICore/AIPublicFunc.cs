using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Shared functions for AI
/// </summary>
public class AIPublicFunc : MonoBehaviour {

    public static Unit GetNeareatEnemy(Unit aiUnit)
    {
        //this is a temporary method for test
        //find the nearest enemy unit as ai target
        List<Unit> nonMyUnitList = UnitManager.GetInstance().units.FindAll(p => p.playerNumber != aiUnit.playerNumber);

        if (nonMyUnitList.Count == 0)
            return null;

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

        return nearUnit;
    }

    public static Unit GetNeareatMate(Unit aiUnit)
    {
        //this is a temporary method for test
        //find the nearest enemy unit as ai target
        List<Unit> myUnitList = UnitManager.GetInstance().units.FindAll(p => p.playerNumber == aiUnit.playerNumber);
        //myUnitList must not include aiUnit self
        myUnitList.Remove(aiUnit);

        if (myUnitList.Count == 0)
            return null;

        int nearestIdx = 0;
        float disMin = 9999;
        for (int i = 0; i < myUnitList.Count; ++i)
        {
            float distance = Vector3.Distance(aiUnit.transform.position, myUnitList[i].transform.position);
            if (disMin > distance)
            {
                disMin = distance;
                nearestIdx = i;
            }
        }
        Unit nearUnit = myUnitList[nearestIdx];
        //Debug.Log("NearPlayer Name is=>" + nearUnit.name);

        return nearUnit;
    }

    public static Unit GetUnit(Vector3 floor)
    {
        List<Unit> unitList = UnitManager.GetInstance().units;
        return unitList.Find(eu => eu.transform.position == floor);
    }

    /// <summary>
    /// caculate all of coordinates of floors around
    /// </summary>
    public static List<Vector3> GetNextFloors(Vector3 floor)
    {
        List<Vector3> result = new List<Vector3>(4);
        //the orientation points to top right conner is forward
        Vector3 forward = floor + new Vector3(-1, 0, 0);
        Vector3 back = floor + new Vector3(1, 0, 0);
        Vector3 left = floor + new Vector3(0, 0, -1);
        Vector3 right = floor + new Vector3(0, 0, 1);
        result.Add(forward);
        result.Add(back);
        result.Add(left);
        result.Add(right);
        return result;
    }

    /// <summary>
    /// get the neareast floor position from unit among floorList
    /// </summary>
    public static Vector3 GetNeareastFloor(Unit unit, List<Vector3> floorList)
    {
        int nearestIdx = 0;
        float disMin = 9999;
        for (int i = 0; i < floorList.Count; ++i)
        {
            float distance = Vector3.Distance(unit.transform.position, floorList[i]);
            if (disMin > distance)
            {
                disMin = distance;
                nearestIdx = i;
            }
        }
        return floorList[nearestIdx];
    }

    public static IEnumerator TurnToAI(Unit aiUnit,string orientation)
    {
        if (SkillManager.GetInstance().skillQueue.Count == 0)
            aiUnit.GetComponent<CharacterAction>().SetSkill("ChooseDirection"); //for more move step

        ChooseDirection chooseDirection = SkillManager.GetInstance().skillQueue.Peek().Key as ChooseDirection;
        yield return 0;
        chooseDirection.OnArrowHovered(orientation);
        yield return new WaitForSeconds(1f);
        chooseDirection.Confirm_AI();
        yield return 0;
    }

    public static IEnumerator TurnToAI(Unit aiUnit, Unit target)
    {
        aiUnit.transform.LookAt(target.transform);
        yield return new WaitForSeconds(0.2f);
    }
}
