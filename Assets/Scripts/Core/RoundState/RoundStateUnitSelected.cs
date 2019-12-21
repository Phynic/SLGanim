using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RoundStateUnitSelected : RoundState
{
    private Unit _unit;
    MoveRange range = new MoveRange();
    public override void OnUnitClicked(Unit unit)
    {
        foreach (var f in BattleFieldManager.GetInstance().floors)
        {
            f.Value.SetActive(false);
        }
        range = new MoveRange();

        //如果点击角色和当前角色不同
        if (RoundManager.GetInstance().CurrentUnit != unit)
        {
            //点己方角色
            if (unit.playerNumber.Equals(RoundManager.GetInstance().CurrentPlayerNumber) && !unit.UnitEnd && SkillManager.GetInstance().skillQueue.Peek().Key.EName == "FirstAction")
            {
                //FirstAction中点击其他角色，手动Reset
                SkillManager.GetInstance().skillQueue.Peek().Key.Reset();
                RoundManager.GetInstance().RoundState = new RoundStateUnitSelected(unit);
            }
            //点敌人，或点己方已结束角色
            else if (SkillManager.GetInstance().skillQueue.Peek().Key.EName == "FirstAction")
            {
                //FirstAction中点击其他角色，手动Reset
                SkillManager.GetInstance().skillQueue.Peek().Key.Reset();
                RoundManager.GetInstance().RoundState = new RoundStateWaitingForInput(unit);
                range.CreateMoveRange(unit.transform);
                RoleInfoView.GetInstance().Open(unit.transform);

                var outline = Camera.main.GetComponent<RenderBlurOutline>();
                if (outline)
                    outline.RenderOutLine(unit.transform);
            }
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

        if (RoundManager.GetInstance().CurrentUnit != _unit)
            _unit.OnUnitSelected();
        //FirstAction中已处理
        //RoleInfoView.GetInstance().Refresh(_unit.transform);
    }

    public override void OnStateExit()
    {
        _unit.OnUnitDeselected();
    }
}

