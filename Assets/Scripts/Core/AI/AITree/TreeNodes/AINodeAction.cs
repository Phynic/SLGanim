using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AINodeAction : AINode<bool>
{
    public override IEnumerator Execute()
    {
        Transform targetTransform = aiTree.aiTarget.transform;
        Transform unitTransform = aiTree.aiUnit.transform;
        
        //Move
        if (unitTransform.position == aiTree.moveTarget)   //target position is as same as aiUnit, don't need to move
        {
            Debug.Log(aiTree.aiUnit.name + " don't need to move");
            yield return new WaitForSeconds(0.5f);
        }
        else
        {
            //auto move forward target
            aiTree.aiUnit.GetComponent<CharacterAction>().SetSkill("Move");
            Move moveSkill = SkillManager.GetInstance().skillQueue.Peek().Key as Move;

            moveSkill.Init(unitTransform);
            
            //var path = aiTree.moveTarget - unitTransform.position;
            //Debug.Log(path);
            //if(path.x > 0)
            //{
            //    for (int i = 0; i < path.x; i++)
            //    {
            //        moveSkill.Focus(unitTransform.position + new Vector3(i, 0, 0));
            //        yield return new WaitForSeconds(0.1f);
            //     }
            //}
            //if(path.z > 0)
            //{
            //    for (int i = 0; i < path.z; i++)
            //    {
            //        moveSkill.Focus(unitTransform.position + new Vector3(path.x, 0, 0) + new Vector3(0, 0, i));
            //        yield return new WaitForSeconds(0.1f);
            //    }
            //}
            
            moveSkill.Focus(aiTree.moveTarget);
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
                aiTree.outline.RenderOutLine(unitTransform);

            aiTree.aiUnit.GetComponent<CharacterAction>().SetSkill(aiTree.skillName);

            UnitSkill unitSkill = SkillManager.GetInstance().skillQueue.Peek().Key as UnitSkill;
            aiTree.rtsCamera.FollowTarget(targetTransform.position);
            yield return new WaitForSeconds(0.5f);
            unitSkill.Init(unitTransform);
            
            unitSkill.Focus(targetTransform.position);
            
            yield return new WaitForSeconds(0.5f);
            if (aiTree.outline)
                aiTree.outline.CancelRender();
            unitSkill.Confirm();
            yield return new WaitUntil(() => { return unitSkill.complete == true; });
            aiTree.rtsCamera.FollowTarget(unitTransform.position);
        }

        //ChooseDirection
        yield return StartCoroutine(AIPublicFunc.TurnToAI(aiTree.aiUnit, aiTree.finalDirection));

        //ChooseTrick
        
        Data = true;
        yield return 0;
    }
}
