using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AINodeEnemyInRange : AINode<bool> {

    public override IEnumerator Execute()
    {
        //find the nearest enemy
        aiTree.aiTarget = AIPublicFunc.GetNearestEnemy(aiTree.aiUnit);
        
        List<Vector3> nextFloors = AIPublicFunc.GetNextFloors(aiTree.aiTarget.transform.position);

        //just leave these floors that could be reached in moveRange
        List<Vector3> canReachNextFloors = nextFloors.FindAll(nf => aiTree.moveRange.rangeDic.ContainsKey(nf));

        //detect if there are other team mates in these floors
        List<Unit> teamMatesList = UnitManager.GetInstance().units.FindAll(p => p.playerNumber == aiTree.aiUnit.playerNumber);
        List<Vector3> availableFloors = new List<Vector3>(canReachNextFloors);

        foreach (Unit u in teamMatesList)
        {
            foreach (Vector3 v in canReachNextFloors)
            {
                if (u.transform.position == v && u.transform != aiTree.aiUnit.transform)
                    availableFloors.Remove(v);
            }
        }

        //find if the nearest enemy in attack range
        //List<Vector3> enemyFloor = aiTree.moveRange.enemyFloor;
        //List<Unit> enemyList = new List<Unit>(); //get all of enemies of current ai unit
        //foreach (Vector3 v in enemyFloor)
        //{
        //    enemyList.Add(AIPublicFunc.GetUnit(v));
        //}

        if (availableFloors.Count > 0)
        {
            aiTree.moveTarget = AIPublicFunc.GetNeareastFloor(aiTree.aiUnit, availableFloors);
            //if the enemy is in attack range of aiUnit
            Data = true;
        }
        else
            Data = false;
        yield return 0;
    }

}
