using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class Move : Skill 
{
    private MoveRange range = new MoveRange();
    private Movement movement = new Movement();
    private List<Vector3> path = new List<Vector3>();
    private Vector3 focus;
    private bool final;
    
    public override bool Init(Transform character)
    {
        this.character = character;
        
        range.CreateMoveRange(character);
        
        focus = new Vector3(-1, -1, -1);
        final = false;
        foreach (var f in BattleFieldManager.GetInstance().floors)
        {
            if (f.Value.activeInHierarchy)
            {
                f.Value.GetComponent<Floor>().FloorClicked += Confirm;
                f.Value.GetComponent<Floor>().FloorHovered += Focus;
                f.Value.GetComponent<Floor>().FloorExited += RecoverColor;
            }
        }
        //角色加入忽略层，方便选取
        UnitManager.GetInstance().units.FindAll(u => u.playerNumber == character.GetComponent<Unit>().playerNumber).ForEach(u => BattleFieldManager.GetInstance().GetFloor(u.transform.position).gameObject.layer = 2);
        UnitManager.GetInstance().units.ForEach(u => u.gameObject.layer = 2);
        
        return true;
    }

    public override bool Check()
    {
        return true;
    }

    public override bool OnUpdate(Transform character)
    {
        
        switch (skillState)
        {
            case SkillState.init:
                Init(character);
                skillState = SkillState.waitForInput;
                break;
            case SkillState.waitForInput:
                if (final)
                {
                    if (Check())
                    {
                        foreach (var f in BattleFieldManager.GetInstance().floors)
                        {
                            f.Value.GetComponent<Floor>().FloorClicked -= Confirm;
                            f.Value.GetComponent<Floor>().FloorHovered -= Focus;
                            f.Value.GetComponent<Floor>().FloorExited -= RecoverColor;
                        }
                        path = range.CreatePath(focus);
                        //角色取出忽略层
                        UnitManager.GetInstance().units.FindAll(u => u.playerNumber == character.GetComponent<Unit>().playerNumber).ForEach(u => BattleFieldManager.GetInstance().GetFloor(u.transform.position).gameObject.layer = 0);
                        UnitManager.GetInstance().units.ForEach(u => u.gameObject.layer = 0);
                        skillState = SkillState.confirm;
                    }
                    else
                    {
                        final = false;
                    }
                }
                break;
            case SkillState.confirm:
                movement.SetMovement(path, character);
                skillState = SkillState.applyEffect;
                break;
            case SkillState.applyEffect:
                if (movement.ExcuteMove())
                {
                    range.Delete();
                    //SecondAction
                    character.GetComponent<CharacterAction>().SetSkill("SecondAction");
                    
                    return true;
                }
                break;
            case SkillState.reset:
                
                return true;
        }
        return false;
    }
    
    private void Focus(object sender, EventArgs e)
    {
        var go = sender as GameObject;
        focus = go.transform.position;
        range.ExcuteChangeRoadColorAndRotate(focus);
    }

    public void Focus(Floor floor)
    {
        focus = floor.transform.position;
        range.ExcuteChangeRoadColorAndRotate(focus);
    }

    private void Confirm(object sender, EventArgs e)
    {
        final = true;
    }
    
    public void Confirm()
    {
        final = true;
    }

    void RecoverColor(object sender, EventArgs e)
    {
        range.RecoverColor();
    }

    public override void Reset()
    {
        //按照顺序，逆序消除影响。因为每次会Init()，所以不必都Reset。
        movement.Reset();
        range.Reset();
        
        foreach (var f in BattleFieldManager.GetInstance().floors)
        {
            f.Value.GetComponent<Floor>().FloorClicked -= Confirm;
            f.Value.GetComponent<Floor>().FloorHovered -= Focus;
            f.Value.GetComponent<Floor>().FloorExited -= RecoverColor;
        }
        //角色取出忽略层
        UnitManager.GetInstance().units.FindAll(u => u.playerNumber == character.GetComponent<Unit>().playerNumber).ForEach(u => BattleFieldManager.GetInstance().GetFloor(u.transform.position).gameObject.layer = 0);
        UnitManager.GetInstance().units.ForEach(u => u.gameObject.layer = 0);

        base.Reset();
        
    }
}
