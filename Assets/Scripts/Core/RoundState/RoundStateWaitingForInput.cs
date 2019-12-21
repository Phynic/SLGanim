using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

public class RoundStateWaitingForInput : RoundState
{
    MoveRange range = new MoveRange();
    Unit focus;
    public RoundStateWaitingForInput()
    {
        focus = null;
    }
    public RoundStateWaitingForInput(Unit focus)
    {
        this.focus = focus;
    }
    public override void OnUnitClicked(Unit unit)
    {
        range = new MoveRange();

        if (focus != unit)
        {
            RoleInfoView.TryClose();
            BattleFieldManager.GetInstance().HideAllFloors();
            //角色被选中（可对角色进行操作）
            if (unit.playerNumber.Equals(RoundManager.GetInstance().CurrentPlayerNumber) && !unit.UnitEnd)
            {
                RoundManager.GetInstance().RoundState = new RoundStateUnitSelected(unit);
            }
            //角色未被选中（可能是敌方角色）
            else
            {
                focus = unit;
                var outline = Camera.main.GetComponent<RenderBlurOutline>();
                if (outline)
                    outline.RenderOutLine(unit.transform);
                range.CreateMoveRange(unit.transform);
                RoleInfoView.GetInstance().Open(unit.transform);
            }
        }

#if (UNITY_STANDALONE)
        Camera.main.GetComponent<RTSCamera>().FollowTarget(unit.transform.position);
#endif
    }

    private void BackSpace()
    {
        focus = null;

        var outline = Camera.main.GetComponent<RenderBlurOutline>();
        if (outline)
            outline.CancelRender();
        BattleFieldManager.GetInstance().HideAllFloors();
        RoleInfoView.TryClose();
    }

    public override void OnStateEnter()
    {
        RoundManager.GetInstance().BackSpaceAction += BackSpace;

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
        RoundManager.GetInstance().BackSpaceAction -= BackSpace;
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
