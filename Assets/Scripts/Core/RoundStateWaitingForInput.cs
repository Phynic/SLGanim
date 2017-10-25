using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundStateWaitingForInput : RoundState {
    public RoundStateWaitingForInput(RoundManager roundManager) : base(roundManager)
    {
    }

    public override void OnUnitClicked(Unit unit)
    {
        if (unit.playerNumber.Equals(roundManager.CurrentPlayerNumber) && !unit.UnitEnded)
        {
            roundManager.RoundState = new RoundStateUnitSelected(roundManager, unit);
        }
    }
}
