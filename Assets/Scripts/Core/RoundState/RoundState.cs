using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RoundState {
    public virtual void OnUnitClicked(Unit unit) { }
    public virtual void OnStateEnter() { }
    public virtual void OnStateExit() { }
}
