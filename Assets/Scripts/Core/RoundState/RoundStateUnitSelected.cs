using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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
        if (unit.playerNumber.Equals(RoundManager.GetInstance().CurrentPlayerNumber) && !unit.UnitEnd && SkillManager.GetInstance().skillQueue.Peek().Key.EName == "FirstAction")
        {
            SkillManager.GetInstance().skillQueue.Peek().Key.Reset();
            RoundManager.GetInstance().RoundState = new RoundStateUnitSelected(unit);
        }
        else if(SkillManager.GetInstance().skillQueue.Peek().Key.EName == "FirstAction")
        {
            
            var outline = Camera.main.GetComponent<RenderBlurOutline>();
            if (outline)
                outline.RenderOutLine(unit.transform);

            SkillManager.GetInstance().skillQueue.Peek().Key.Reset();

            
            
            RoundManager.GetInstance().RoundState = new RoundStateWaitingForInput();
            range.CreateMoveRange(unit.transform);
            RoleInfoView.GetInstance().Open(unit.transform);
        }
#if UNITY_STANDALONE
        Camera.main.GetComponent<RTSCamera>().FollowTarget(unit.transform.position);
#endif
    }

    public RoundStateUnitSelected(Unit unit)
    {
        _unit = unit;
    }

    public override void OnStateEnter()
    {
        base.OnStateEnter();
        _unit.OnUnitSelected();
        //FirstAction中已处理
        //RoleInfoView.GetInstance().Refresh(_unit.transform);
    }

    public override void OnStateExit()
    {
        _unit.OnUnitDeselected();
    }
}

