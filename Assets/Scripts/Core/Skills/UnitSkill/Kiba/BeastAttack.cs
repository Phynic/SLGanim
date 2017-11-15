using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeastAttack : AttackSkill {

    public override void SetLevel(int level)
    {
        damageFactor = 20 + level * 5;
    }
}
