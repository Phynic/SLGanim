using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdvancedClone : Clone {

    protected override void SetIdentity(GameObject clone)
    {
        clone.GetComponent<CharacterStatus>().SetAdvancedClone(character);
    }
}
