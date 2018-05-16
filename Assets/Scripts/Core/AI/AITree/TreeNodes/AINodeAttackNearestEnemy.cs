using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AINodeAttackNearestEnemy : AINode<bool> {

    public override IEnumerator Execute()
    {
        //choose a floor in moveRange and it is the neareast from aiUnit to aiTarget
        //get the four floors next aiTarget
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
                if (u.transform.position == v)
                    availableFloors.Remove(v);
            }
        }

        if (availableFloors.Count > 0)
        {
            //find the nearest floor
            aiTree.moveTarget = AIPublicFunc.GetNeareastFloor(aiTree.aiUnit, availableFloors);
            Data = true;
        }
        else
        {
            if (RemoteAttackDetect())
            {
                RemoteAttack();
                Data = true;
            }
            else
                Data = false;

        }
        yield return 0;
    }

    /// <summary>
    /// if there has been 4 team mates around target, aiUnit should remote attack detection,
    /// if remote attack detection fails too,he should think about other enemies to attack.
    /// </summary>
    private bool RemoteAttackDetect()
    {
        return false;
    }

    private void RemoteAttack() {

    }

}
