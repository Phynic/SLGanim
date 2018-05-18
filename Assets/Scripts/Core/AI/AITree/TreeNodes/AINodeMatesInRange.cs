using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AINodeMatesInRange : AINode<bool> {

    public override IEnumerator Execute()
    {
        //find the nearest mate
        aiTree.aiTarget = AIPublicFunc.GetNeareatMate(aiTree.aiUnit);
        if (aiTree.aiTarget == null)
            Data = false;

        //find if the nearest mate in attack range
        List<Vector3> mateFloor = aiTree.moveRange.mateFloor;
        List<Unit> mateList = new List<Unit>(); //get all of enemies of current ai unit
        foreach (Vector3 v in mateFloor)
        {
            mateList.Add(AIPublicFunc.GetUnit(v));
        }

        if (mateList.Contains(aiTree.aiTarget))
            //if the enemy is in attack range of aiUnit
            Data = true;
        else
            Data = false;
        yield return 0;
    }


}
