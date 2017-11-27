using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundStateWaitingForInput : RoundState {
    MoveRange range = new MoveRange();
    public RoundStateWaitingForInput(RoundManager roundManager) : base(roundManager)
    {
    }

    public override void OnUnitClicked(Unit unit)
    {
        foreach (var f in BattleFieldManager.GetInstance().floors)
        {
            f.Value.SetActive(false);
        }
        range = new MoveRange();
        if (unit.playerNumber.Equals(roundManager.CurrentPlayerNumber) && !unit.UnitEnd)
        {
            roundManager.RoundState = new RoundStateUnitSelected(roundManager, unit);
        }
        else
        {
            Camera.main.GetComponent<RenderBlurOutline>().RenderOutLine(unit.transform);
            range.CreateMoveRange(unit.transform);
        }
        
    }
    public override void OnStateExit()
    {
        if (range != null)
            range.Delete();
        base.OnStateExit();
    }
}
