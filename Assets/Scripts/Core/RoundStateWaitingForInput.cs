using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
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
            var outline = Camera.main.GetComponent<RenderBlurOutline>();
            if (outline)
                outline.RenderOutLine(unit.transform);
            range.CreateMoveRange(unit.transform);
            CreatePanel(unit);
        }
        Camera.main.GetComponent<RTSCamera>().FollowTarget(unit.transform.position);
    }

    public override void OnStateEnter()
    {
        if (roleInfoPanel)
            GameObject.Destroy(roleInfoPanel);
        GameObject.Find("Canvas").transform.Find("MenuButton").gameObject.SetActive(true);
        base.OnStateEnter();
    }

    public override void OnStateExit()
    {
        GameObject.Find("Canvas").transform.Find("MenuButton").gameObject.SetActive(false);
        GameObject.Find("Canvas").transform.Find("DebugMenu").gameObject.SetActive(false);
        if (roleInfoPanel)
            GameObject.Destroy(roleInfoPanel);
        if (range != null)
            range.Delete();
        base.OnStateExit();
    }
}
