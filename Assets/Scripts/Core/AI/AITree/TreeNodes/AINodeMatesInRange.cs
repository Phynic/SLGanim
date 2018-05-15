using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AINodeMatesInRange : AINode<bool> {

    public override bool Execute()
    {
        //get move range
        MoveRange moveRange = new MoveRange();
        moveRange.CreateMoveRange(aiTree.aiUnit.transform);
        //find the nearest mate
        aiTree.aiTarget = AIPublicFunc.GetNeareatMate(aiTree.aiUnit);
        //find if the nearest mate in attack range
        List<Vector3> mateFloor = moveRange.mateFloor;
        List<Unit> mateList = new List<Unit>(); //get all of enemies of current ai unit
        foreach (Vector3 v in mateFloor)
        {
            mateList.Add(AIPublicFunc.GetUnit(v));
        }

        if (mateList.Contains(aiTree.aiTarget))
            //if the enemy is in attack range of aiUnit
            return true;
        else
            return false;
    }


}
