using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AINodeAction : AINode<bool>
{
    public override IEnumerator Execute()
    {
        //Move
        if (aiTree.aiUnit.transform.position == aiTree.moveTarget)   //target position is as same as aiUnit, don't need to move
        {
            Debug.Log(aiTree.aiUnit.name + " don't need to move");
            yield return new WaitForSeconds(0.5f);
        }
        else
        {
            //auto move forward target
            aiTree.aiUnit.GetComponent<CharacterAction>().SetSkill("Move");
            Move moveSkill = SkillManager.GetInstance().skillQueue.Peek().Key as Move;

            moveSkill.Init(aiTree.aiUnit.transform);

            GameObject floor = BattleFieldManager.GetInstance().GetFloor(aiTree.moveTarget);
            moveSkill.Focus(floor.GetComponent<Floor>());
            yield return new WaitForSeconds(0.5f);

            aiTree.outline.CancelRender();
            moveSkill.Confirm();
            yield return new WaitUntil(() => { return moveSkill.skillState == Skill.SkillState.reset; });
            
        }
        aiTree.moveRange.Delete();
        yield return new WaitForSeconds(0.1f);

        //Use Skill
        if(aiTree.skillName != "")
        {
            yield return StartCoroutine(AIPublicFunc.TurnToAI(aiTree.aiUnit, aiTree.aiTarget));
            if (aiTree.outline)
                aiTree.outline.RenderOutLine(aiTree.aiUnit.transform);

            aiTree.aiUnit.GetComponent<CharacterAction>().SetSkill(aiTree.skillName);

            UnitSkill unitSkill = SkillManager.GetInstance().skillQueue.Peek().Key as UnitSkill;
            aiTree.rtsCamera.FollowTarget(aiTree.aiTarget.transform.position);
            yield return new WaitForSeconds(0.5f);
            unitSkill.Init(aiTree.aiUnit.transform);
            unitSkill.Focus(aiTree.aiTarget.transform.position);

            yield return new WaitForSeconds(0.5f);
            if (aiTree.outline)
                aiTree.outline.CancelRender();
            unitSkill.Confirm();
            yield return new WaitUntil(() => { return unitSkill.complete == true; });
            aiTree.rtsCamera.FollowTarget(aiTree.aiUnit.transform.position);
        }

        //ChooseDirection
        yield return StartCoroutine(AIPublicFunc.TurnToAI(aiTree.aiUnit, aiTree.finalDirection));

        //ChooseTrick
        
        Data = true;
        yield return 0;
    }
}
