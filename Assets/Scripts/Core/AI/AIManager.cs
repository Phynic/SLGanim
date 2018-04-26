using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Control All AI Unit Actions
/// </summary>
public class AIManager: MonoBehaviour {
    
    public AIFreeBattle aiFreeBattle; //free battle mode handler
    public List<Drama> dramaDict; //predefine the relation between unit own and its drama
    private static AIManager instance;
    private RTSCamera rtsCamera;
    private RenderBlurOutline outline;

    public static AIManager GetInstance() {
        return instance;
    }

    void Awake()
    {
        instance = this;    
    }

    void Start()
    {
        rtsCamera = Camera.main.GetComponent<RTSCamera>();
        outline = Camera.main.GetComponent<RenderBlurOutline>();
    }

    public IEnumerator playFree(int playerNumber) {
        var nonMyUnits = UnitManager.GetInstance().units.FindAll(u => u.playerNumber == playerNumber);

        foreach (var u in nonMyUnits)
        {
            if (u.GetComponent<Unit>().UnitEnd)
                break;
            outline.RenderOutLine(u.transform);
            rtsCamera.FollowTarget(u.transform.position);

            yield return StartCoroutine(aiFreeBattle.activeAI(u));
            u.OnUnitEnd();   //真正的回合结束所应执行的逻辑。
            DebugLogPanel.GetInstance().Log(u.GetComponent<CharacterStatus>().roleCName + "执行完毕");
            yield return new WaitForSeconds(1f);

        }
        RoundManager.GetInstance().EndTurn();
    }

    public IEnumerator playDrama() {
        //do drama doesn't have Unit

        ////check out dramaList if there's aiUnit in it.
        //Drama drama = dramaDict.Find(d => d.owners.Contains(aiUnit));
        //if (drama != null)
        //{
        //    yield return StartCoroutine(drama.Play()); //play drama if find out
        //}
        //else
            yield return 0;
    }
}
