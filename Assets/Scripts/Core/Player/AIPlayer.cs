using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// AI Round
/// </summary>
public class AIPlayer : Player
{
    public bool AIControl = true; //could switch between auto and artifical control
    public bool useDrama = false; //it's taken over by AIManager; otherwise =true it's taken over by Drama 

    public override void Play()
    {

        if (AIControl)
        {
            RoundManager.GetInstance().RoundState = new RoundStateAITurn();
            if (useDrama)
                StartCoroutine(AIManager.GetInstance().playDrama());
            else
                StartCoroutine(AIManager.GetInstance().playFree(playerNumber));
        }
        else
        {
            RoundManager.GetInstance().RoundState = new RoundStateWaitingForInput();
        }
    }

}
