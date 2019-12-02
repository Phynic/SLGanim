using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeastClone : Clone
{
    protected override void SetIdentity(GameObject clone)
    {
        var cloneCS = clone.GetComponent<Unit>();
        var characterCS = character.GetComponent<Unit>();
        cloneCS.SetBeastClone(characterCS);
        cloneCS.identity = "赤丸";
        characterCS.UnitDestroyed += cloneCS.OnDestroyed;
        cloneCS.UnitDestroyed += characterCS.OnDestroyed;
    }
}
