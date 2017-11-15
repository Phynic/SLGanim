using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicShuriken : AttackSkill {

    public override void SetLevel(int level)
    {
        skillRange = 2 + level;
    }
    
}
