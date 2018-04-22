using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPlayer : Player
{
    RTSCamera rtsCamera;
    RenderBlurOutline outline;
    public bool AI = true;
    //AI
    public override void Play(RoundManager roundManager)
    {
        rtsCamera = Camera.main.GetComponent<RTSCamera>();
        outline = Camera.main.GetComponent<RenderBlurOutline>();

        if (AI)
        {
            roundManager.RoundState = new RoundStateAITurn(roundManager);
            StartCoroutine(Play());
        }
        else
        {
            roundManager.RoundState = new RoundStateWaitingForInput(roundManager);
        }
    }
    
    private IEnumerator Play()
    {
        var myUnits = UnitManager.GetInstance().units.FindAll(u => u.playerNumber == playerNumber);
        
        foreach (var u in myUnits)
        {
            if (u.GetComponent<Unit>().UnitEnd)
                break;
            rtsCamera.FollowTarget(u.transform.position);
            outline.RenderOutLine(u.transform);
            if (u.GetComponent<CharacterStatus>().roleEName == "Rock")
            {
                //rock auto recovers
                var currentHp = u.GetComponent<CharacterStatus>().attributes.Find(d => d.eName == "hp").value;
                var currentHPMax = u.GetComponent<CharacterStatus>().attributes.Find(d => d.eName == "hp").valueMax;
                var restValue = (int)(currentHPMax * 0.3f);
                //if the recover HP makes currentHp full, then Hp gets full
                //else just recovers currentHPMax * 0.3f HP
                restValue = currentHp + restValue > currentHPMax ? currentHPMax - currentHp : restValue;

                var hp = currentHp + restValue;

                UIManager.GetInstance().FlyNum(u.GetComponent<CharacterStatus>().arrowPosition / 2 + u.transform.position + Vector3.down * 0.2f, restValue.ToString(), UIManager.hpColor);


                ChangeData.ChangeValue(u.transform, "hp", hp);


                u.OnUnitEnd();   //真正的回合结束所应执行的逻辑。
                DebugLogPanel.GetInstance().Log(u.GetComponent<CharacterStatus>().roleCName + "执行完毕");
                yield return new WaitForSeconds(1f);
            }
            else
            {
                //if the player is enemy
                //fix this one
                yield return StartCoroutine(UseSkill("NinjaCombo", u.transform));
                u.OnUnitEnd();   //真正的回合结束所应执行的逻辑。
                DebugLogPanel.GetInstance().Log(u.GetComponent<CharacterStatus>().roleCName + "执行完毕");
                yield return new WaitForSeconds(1f);
            }
        }
        outline.CancelRender();
        RoundManager.GetInstance().EndTurn();
    }

    private IEnumerator UseSkill(string skillName, Transform character)
    {
        yield return new WaitForSeconds(1.5f);

        //自动移动
        character.GetComponent<CharacterAction>().SetSkill("Move");
        Move moveSkill = SkillManager.GetInstance().skillQueue.Peek().Key as Move;
        var f1 = new Vector3(40.5f, 0, 34.5f);
        moveSkill.Init(character);
        moveSkill.FocusAI(f1);
        yield return new WaitForSeconds(0.5f);
        moveSkill.Confirm();
        yield return new WaitForSeconds(1f);

        yield return StartCoroutine(useUnitSkill(skillName,character));

        ChooseDirection chooseDirection = SkillManager.GetInstance().skillQueue.Peek().Key as ChooseDirection;
        yield return null;
        chooseDirection.OnArrowHovered("forward");
        yield return new WaitForSeconds(1f);
        chooseDirection.Confirm_AI();

    }

    IEnumerator useUnitSkill(string skillName, Transform character) {
        bool isSuccess = character.GetComponent<CharacterAction>().SetSkill(skillName);
        //Debug.Log("useUnitSkill=>" + skillName + "=>" + isSuccess);
        var f = new Vector3(40.5f - 1, 0, 34.5f);
        UnitSkill unitSkill = SkillManager.GetInstance().skillQueue.Peek().Key as UnitSkill;
        rtsCamera.FollowTarget(f);
        yield return new WaitForSeconds(0.5f);
        unitSkill.Init(character);
        unitSkill.Focus(f);

        yield return new WaitForSeconds(0.5f);
        unitSkill.Confirm();
        yield return new WaitUntil(() => { return unitSkill.complete == true; });
        rtsCamera.FollowTarget(character.position);
        yield return 0;
    }
}
