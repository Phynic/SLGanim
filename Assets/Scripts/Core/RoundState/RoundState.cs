using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RoundState {
    protected RoundManager roundManager;

    protected RoundState(RoundManager roundManager)
    {
        this.roundManager = roundManager;
    }

    public virtual void OnUnitClicked(Unit unit) { }

    public virtual void OnStateEnter() { }
    public virtual void OnStateExit() { }
}
