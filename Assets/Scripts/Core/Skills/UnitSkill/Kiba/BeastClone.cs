using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeastClone : Clone
{
    protected override void SetIdentity(GameObject clone)
    {
        var cloneCS = clone.GetComponent<CharacterStatus>();
        var characterCS = character.GetComponent<CharacterStatus>();
        cloneCS.SetBeastClone(character);
        cloneCS.identity = "赤丸";
        characterCS.UnitDestroyed += cloneCS.OnDestroyed;
        cloneCS.UnitDestroyed += characterCS.OnDestroyed;
    }
}
