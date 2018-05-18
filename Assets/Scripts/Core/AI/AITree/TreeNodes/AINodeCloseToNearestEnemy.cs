using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AINodeCloseToNearestEnemy : AINode<bool> {

    public override IEnumerator Execute()
    {
        MoveMoreStep();
        Data = true;
        yield return 0;
    }

    /// <summary>
    /// if the aiUnit can't reach enemies in one round, he should choose more round to complete attack
    /// the move target should not only be the neareast from the neareast enemy
    /// but also in move range
    /// </summary>
    private void MoveMoreStep()
    {
        List<Vector3> availableFloors = new List<Vector3>();
        foreach (KeyValuePair<Vector3, GameObject> kp in aiTree.moveRange.rangeDic)
        {
            availableFloors.Add(kp.Key);
        }
        aiTree.moveTarget = AIPublicFunc.GetNeareastFloor(aiTree.aiTarget, availableFloors);
    }

}
