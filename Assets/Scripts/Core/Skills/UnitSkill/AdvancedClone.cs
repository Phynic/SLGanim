using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdvancedClone : Clone {

    protected override void SetIdentity(GameObject clone)
    {
        //复制的是character的数据，所以这里传character
        clone.GetComponent<Unit>().Init(character.GetComponent<Unit>().CharacterData, 1003, character.GetComponent<Unit>().attributes);
    }
}
