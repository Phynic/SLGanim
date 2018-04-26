using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// AI Round
/// </summary>
public class AIPlayer : Player
{
    RTSCamera rtsCamera;
    RenderBlurOutline outline;
    public bool AIControl = true; //could switch between auto and artifical control
    public bool DramaOrFree = false; //it's taken over by AIManager; otherwise =true it's taken over by Drama 

    public override void Play(RoundManager roundManager)
    {
        rtsCamera = Camera.main.GetComponent<RTSCamera>();
        outline = Camera.main.GetComponent<RenderBlurOutline>();

        if (AIControl)
        {
            roundManager.RoundState = new RoundStateAITurn(roundManager);
            if (DramaOrFree)
                StartCoroutine(AIManager.GetInstance().playDrama());
            else
                StartCoroutine(AIManager.GetInstance().playFree(playerNumber));
        }
        else
        {
            roundManager.RoundState = new RoundStateWaitingForInput(roundManager);
        }
    }

}
