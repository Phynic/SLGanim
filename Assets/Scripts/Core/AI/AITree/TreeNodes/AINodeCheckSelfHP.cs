using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AINodeCheckSelfHP : AINode<bool> {
    private const float hpThreshold = 0.3f;
    public override IEnumerator Execute()
    {
        int aiHP = aiTree.aiUnit.GetComponent<Unit>().attributes.Find(a => a.eName == "hp").Value;
        int aiMaxHP = aiTree.aiUnit.GetComponent<Unit>().attributes.Find(a => a.eName == "hp").ValueMax;
        if (aiHP > aiMaxHP * hpThreshold)
            Data = true;
        else
            Data = false;
        yield return 0;
    }

}
