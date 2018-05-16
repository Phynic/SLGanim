using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AINodeAfterSkill : AINode<bool> {

    public override IEnumerator Execute()
    {
        yield return StartCoroutine(AIPublicFunc.TurnToAI(aiTree.aiUnit, "forward"));
        Data = true;
        yield return 0;
    }

}
