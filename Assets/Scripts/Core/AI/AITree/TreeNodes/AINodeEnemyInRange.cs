using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AINodeEnemyInRange : AINode<bool> {

    public override IEnumerator Execute()
    {
        //find the nearest enemy
        aiTree.aiTarget = AIPublicFunc.GetNeareatEnemy(aiTree.aiUnit);

        //find if the nearest enemy in attack range
        List<Vector3> enemyFloor = aiTree.moveRange.enemyFloor;
        List<Unit> enemyList = new List<Unit>(); //get all of enemies of current ai unit
        foreach (Vector3 v in enemyFloor)
        {
            enemyList.Add(AIPublicFunc.GetUnit(v));
        }

        if (enemyList.Contains(aiTree.aiTarget))
            //if the enemy is in attack range of aiUnit
            Data = true;
        else
            Data = false;
        yield return 0;
    }

}
