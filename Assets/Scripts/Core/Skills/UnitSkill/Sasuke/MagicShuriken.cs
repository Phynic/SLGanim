using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicShuriken : AttackSkill {

    public override void SetLevel(int level)
    {
        skillRange = 3 + (level - 1) * (int)growFactor;
    }
    
}
