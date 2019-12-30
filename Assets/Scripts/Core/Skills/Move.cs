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
        if (!isAI)
        {
            foreach (var f in BattleFieldManager.GetInstance().floors)
            {
                if (f.Value.activeInHierarchy)
                {
                    f.Value.GetComponent<Floor>().FloorClicked += Confirm;
                    f.Value.GetComponent<Floor>().FloorHovered += Focus;
                    f.Value.GetComponent<Floor>().FloorExited += RecoverColor;
                }
            }
        }

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
                if (!isAI)
                    Init(character); //we need to initial AI character manually
                skillState = SkillState.waitForInput;
                break;
            case SkillState.waitForInput:
                if (final)
                {
                    if (Check())
                    {
                        if (!isAI)
                        {
                            foreach (var f in BattleFieldManager.GetInstance().floors)
                            {
                                f.Value.GetComponent<Floor>().FloorClicked -= Confirm;
                                f.Value.GetComponent<Floor>().FloorHovered -= Focus;
                                f.Value.GetComponent<Floor>().FloorExited -= RecoverColor;
                            }
                        }
                        path = range.CreatePath(focus);

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

                    if (!isAI)
                        character.GetComponent<CharacterAction>().SetSkill("SecondAction"); //SecondAction

                    skillState = SkillState.reset;
                    return true;
                }
                break;
            case SkillState.reset:

                return true;
        }
        return false;
    }

    private void Focus(GameObject sender)
    {
        focus = sender.transform.position;
        range.ExcuteChangeRoadColorAndRotate(focus);
    }

    public void Focus(Vector3 floor)
    {
        focus = floor;
        range.ExcuteChangeRoadColorAndRotate(focus);
    }

    private void Confirm(GameObject sender)
    {
        final = true;
    }

    public void Confirm()
    {
        final = true;
    }

    void RecoverColor(GameObject sender)
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

        base.Reset();

    }
}
