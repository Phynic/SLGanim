using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// AI will fight with Human Player without drama. We call this mode free battle mode.
/// </summary>
public class AIFreeBattle : MonoBehaviour {

    private Unit aiUnit; //current control which ai unit
    //private StrategyType strategy; //use which strategy to deal with enemy
    private AITree aiTree; //decide strategy to deal with enemy

    #region Decision
    /// <summary>
    /// start up whole AI System externally 
    /// </summary>
    public IEnumerator ActiveAI(Unit _aiUnit)
    {
        aiUnit = _aiUnit;

        ChooseBaseStrategy();
        yield return StartCoroutine(DoAction());
    }

    private void ChooseBaseStrategy()
    {
        aiTree = AITreeDefend.inst;
    }

    IEnumerator DoAction()
    {
        yield return StartCoroutine(aiTree.ActivateAITree(aiUnit));
    }
    #endregion
}
