using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AINodeChooseSkill : AINode<bool> {

    public override IEnumerator Execute()
    {
        aiTree.skillName = "NinjaCombo";
        Data = true;
        yield return 0;
    }

}
