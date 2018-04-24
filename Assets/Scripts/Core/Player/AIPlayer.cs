using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPlayer : Player
{
    RTSCamera rtsCamera;
    RenderBlurOutline outline;
    public bool AI = true; //could switch between auto and artifical control
    //AI
    public override void Play(RoundManager roundManager)
    {
        rtsCamera = Camera.main.GetComponent<RTSCamera>();
        outline = Camera.main.GetComponent<RenderBlurOutline>();

        if (AI)
        {
            roundManager.RoundState = new RoundStateAITurn(roundManager);
            StartCoroutine(Play());
        }
        else
        {
            roundManager.RoundState = new RoundStateWaitingForInput(roundManager);
        }
    }
    
    private IEnumerator Play()
    {
        var nonMyUnits = UnitManager.GetInstance().units.FindAll(u => u.playerNumber == playerNumber);
        
        foreach (var u in nonMyUnits)
        {
            if (u.GetComponent<Unit>().UnitEnd)
                break;
            outline.RenderOutLine(u.transform);
            rtsCamera.FollowTarget(u.transform.position); 
            if (u.GetComponent<CharacterStatus>().roleEName == "Rock")
            {
                //rock auto recovers
                var currentHp = u.GetComponent<CharacterStatus>().attributes.Find(d => d.eName == "hp").value;
                var currentHPMax = u.GetComponent<CharacterStatus>().attributes.Find(d => d.eName == "hp").valueMax;
                var restValue = (int)(currentHPMax * 0.3f);
                //if the recover HP makes currentHp full, then Hp gets full
                //else just recovers currentHPMax * 0.3f HP
                restValue = currentHp + restValue > currentHPMax ? currentHPMax - currentHp : restValue;

                var hp = currentHp + restValue;

                UIManager.GetInstance().FlyNum(u.GetComponent<CharacterStatus>().arrowPosition / 2 + u.transform.position + Vector3.down * 0.2f, restValue.ToString(), UIManager.hpColor);


                ChangeData.ChangeValue(u.transform, "hp", hp);


                u.OnUnitEnd();   //真正的回合结束所应执行的逻辑。
                DebugLogPanel.GetInstance().Log(u.GetComponent<CharacterStatus>().roleCName + "执行完毕");
                yield return new WaitForSeconds(1f);
            }
            else
            { 
                yield return StartCoroutine(AIManager.GetInstance().activeAI(u));
                u.OnUnitEnd();   //真正的回合结束所应执行的逻辑。
                DebugLogPanel.GetInstance().Log(u.GetComponent<CharacterStatus>().roleCName + "执行完毕");
                yield return new WaitForSeconds(1f);
            }
        }
        //outline.CancelRender();
        RoundManager.GetInstance().EndTurn();
    } 
}
