using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AINodeCheckSelfHP : AINode<bool> {
    private const float hpThreshold = 0.3f;
    public override IEnumerator Execute()
    {
        int aiHP = aiTree.aiUnit.GetComponent<CharacterStatus>().attributes.Find(a => a.eName == "hp").value;
        int aiMaxHP = aiTree.aiUnit.GetComponent<CharacterStatus>().attributes.Find(a => a.eName == "hp").valueMax;
        if (aiHP > aiMaxHP * hpThreshold)
            Data = true;
        else
            Data = false;
        yield return 0;
    }

}
