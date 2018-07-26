using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundStateUnitSelected : RoundState {
    private Unit _unit;
    MoveRange range = new MoveRange();
    public override void OnUnitClicked(Unit unit)
    {
        
        foreach (var f in BattleFieldManager.GetInstance().floors)
        {
            f.Value.SetActive(false);
        }
        range = new MoveRange();
        if (unit.playerNumber.Equals(roundManager.CurrentPlayerNumber) && !unit.UnitEnd && SkillManager.GetInstance().skillQueue.Peek().Key.EName == "FirstAction")
        {
            SkillManager.GetInstance().skillQueue.Peek().Key.Reset();
            roundManager.RoundState = new RoundStateUnitSelected(roundManager, unit);
        }
        else if(SkillManager.GetInstance().skillQueue.Peek().Key.EName == "FirstAction")
        {
            
            var outline = Camera.main.GetComponent<RenderBlurOutline>();
            if (outline)
                outline.RenderOutLine(unit.transform);

            SkillManager.GetInstance().skillQueue.Peek().Key.Reset();
            
            RoundManager.GetInstance().RoundState = new RoundStateWaitingForInput(RoundManager.GetInstance());
            range.CreateMoveRange(unit.transform);
            ((RoundStateWaitingForInput)RoundManager.GetInstance().RoundState).CreatePanel(unit);
        }
        Camera.main.GetComponent<RTSCamera>().FollowTarget(unit.transform.position);
    }

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

