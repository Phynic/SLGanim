using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundStateWaitingForInput : RoundState {
    MoveRange range = new MoveRange();
    GameObject roleInfoPanel;
    public RoundStateWaitingForInput(RoundManager roundManager) : base(roundManager)
    {
    }

    public void CreatePanel(Unit unit)
    {
        roleInfoPanel = UIManager.GetInstance().CreateRoleInfoPanel(unit.transform);
    }

    public void DestroyPanel()
    {
        if (roleInfoPanel)
            GameObject.Destroy(roleInfoPanel);
    }

    public override void OnUnitClicked(Unit unit)
    {
        if (roleInfoPanel)
            GameObject.Destroy(roleInfoPanel);
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
            CreatePanel(unit);
        }
        
    }

    public override void OnStateEnter()
    {
        if (roleInfoPanel)
            GameObject.Destroy(roleInfoPanel);
        base.OnStateEnter();
    }

    public override void OnStateExit()
    {
        
        if (roleInfoPanel)
            GameObject.Destroy(roleInfoPanel);
        if (range != null)
            range.Delete();
        base.OnStateExit();
    }
}
