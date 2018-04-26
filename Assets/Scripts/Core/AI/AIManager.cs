using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Control All AI Unit Actions
/// </summary>
public class AIManager: MonoBehaviour {
    
    public List<Drama> unitDramaList; //unitDramaList won't run in order,such as combine skill, it'll be called in AIFreeBattle
    public List<Drama> sceneDramaList; //sceneDramaList doesn't have Unit para, it'll run in order
    private static AIManager instance;
    private RTSCamera rtsCamera;
    private RenderBlurOutline outline;
    private AIFreeBattle aiFreeBattle; //free battle mode handler

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
        aiFreeBattle = transform.Find("AIFreeBattle").GetComponent<AIFreeBattle>();
        preloadDrama();
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

    /// <summary>
    /// only refer to play scene drama
    /// </summary>
    public IEnumerator playDrama() {
        foreach (Drama drama in sceneDramaList) {
            yield return StartCoroutine(drama.Play());
        }
    }

    private void preloadDrama() {
        unitDramaList = new List<Drama>();
        sceneDramaList = new List<Drama>();

        GameObject AIDrama = GameObject.Find("GameController/AIManager/AIDrama");

        unitDramaList.AddRange(AIDrama.GetComponents<UnitDrama>());
        sceneDramaList.AddRange(AIDrama.GetComponents<SceneDrama>());
    }
}
