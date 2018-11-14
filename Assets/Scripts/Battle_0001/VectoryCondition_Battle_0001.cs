using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class VectoryCondition_Battle_0001 : VectoryCondition
{
    public override int CheckVectory(List<Unit> Units)
    {
        //Battle01胜利条件
        if(Units.FindAll(u => ((CharacterStatus)u).roleCName == "岩墙").Count < 8)
        {
            return 1;
        }

        return base.CheckVectory(Units);
    }
}
