using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeastClone : Clone
{
    protected override void SetIdentity(GameObject clone)
    {
        var cloneUnit = clone.GetComponent<Unit>();
        var characterUnit = character.GetComponent<Unit>();
        cloneUnit.Init(characterUnit.CharacterData, 1003, characterUnit.attributes);
        cloneUnit.characterIdentity = Unit.CharacterIdentity.beastClone;
        cloneUnit.identity = "赤丸";
        characterUnit.UnitDestroyed += cloneUnit.OnDestroyed;
        cloneUnit.UnitDestroyed += characterUnit.OnDestroyed;
    }
}
