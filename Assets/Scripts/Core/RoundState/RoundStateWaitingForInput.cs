using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

public class RoundStateWaitingForInput : RoundState {
    MoveRange range = new MoveRange();
    public RoundStateWaitingForInput()
    {

    }

    public override void OnUnitClicked(Unit unit)
    {
        RoleInfoView.TryClose();
        foreach (var f in BattleFieldManager.GetInstance().floors)
        {
            f.Value.SetActive(false);
        }
        range = new MoveRange();
        //角色被选中（可对角色进行操作）
        if (unit.playerNumber.Equals(RoundManager.GetInstance().CurrentPlayerNumber) && !unit.UnitEnd)
        {
            RoundManager.GetInstance().RoundState = new RoundStateUnitSelected(unit);
        }
        //角色未被选中（可能是敌方角色）
        else
        {
            var outline = Camera.main.GetComponent<RenderBlurOutline>();
            if (outline)
                outline.RenderOutLine(unit.transform);
            range.CreateMoveRange(unit.transform);
            RoleInfoView.GetInstance().Open(unit.transform);
        }
#if (UNITY_STANDALONE)
        Camera.main.GetComponent<RTSCamera>().FollowTarget(unit.transform.position);
#endif
    }

    public override void OnStateEnter()
    {
        RoleInfoView.TryClose();
        if (BattleView.isInit)
        {
            BattleView.GetInstance().menuButton.gameObject.SetActive(true);
        }
        RoundManager.GetInstance().CurrentUnit = null;
        base.OnStateEnter();
    }

    public override void OnStateExit()
    {
        if (BattleView.isInit)
        {
            BattleView.GetInstance().menuButton.gameObject.SetActive(false);
            BattleView.GetInstance().debugMenu.gameObject.SetActive(false);
        }
        RoleInfoView.TryClose();
        if (range != null)
            range.Delete();
        base.OnStateExit();
    }
}
