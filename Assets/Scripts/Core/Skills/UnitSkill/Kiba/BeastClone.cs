using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeastClone : AdvancedClone
{
    protected override void SetIdentity(GameObject clone)
    {
        base.SetIdentity(clone);
        clone.GetComponent<CharacterStatus>().identity = "赤丸";
    }

}
