using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VectoryCondition : MonoBehaviour {

    /// <summary>
    /// 
    /// </summary>
    /// <returns>
    /// 0 未结束
    /// 1 胜利
    /// 2 失败
    /// </returns>
    public virtual int CheckVectory(List<Unit> Units)
    {
        //常规胜利条件
        var totalPlayersAlive = Units.Select(u => u.playerNumber).Distinct().ToList(); //Checking if the game is over
        if (totalPlayersAlive.Count == 1)
        {

            if (totalPlayersAlive[0] == 0)
            {
                return 1;
            }
            else
            {
                return 2;
            }
        }
        return 0;
    }
}
