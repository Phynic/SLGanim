using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundStateUnitSelected : RoundState {

    private Unit _unit;
    
    public RoundStateUnitSelected(RoundManager roundManager, Unit unit) : base(roundManager)
    {
        _unit = unit;
        RoundManager.GetInstance().CurrentUnit = _unit;
    }

    public override void OnStateEnter()
    {
        base.OnStateEnter();

        _unit.OnUnitSelected();
    }

    public override void OnStateExit()
    {
        _unit.OnUnitDeselected();
    }
}

