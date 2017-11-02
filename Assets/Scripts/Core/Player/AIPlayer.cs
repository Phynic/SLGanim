using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPlayer : Player {
    //AI
    //public override void Play(RoundManager roundManager)
    //{
    //    roundManager.RoundState = new RoundStateAITurn(roundManager);
    //    StartCoroutine(Play());
    //}

    //Human
    public override void Play(RoundManager roundManager)
    {
        roundManager.RoundState = new RoundStateWaitingForInput(roundManager);
    }

    private IEnumerator Play()
    {
        var myUnits = UnitManager.GetInstance().units.FindAll(u => u.playerNumber == playerNumber);
        foreach (var u in myUnits)
        {
            u.OnUnitEnd();   //真正的回合结束所应执行的逻辑。
            DebugLogPanel.GetInstance().Log(u.GetComponent<CharacterStatus>().roleCName + "执行完毕");
            yield return new WaitForSeconds(0.2f);
        }
        RoundManager.GetInstance().EndTurn();
        yield return null;
    }
}
