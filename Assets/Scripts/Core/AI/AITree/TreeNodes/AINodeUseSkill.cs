using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AINodeUseSkill : AINode<bool> {

    public override IEnumerator Execute()
    {
        yield return StartCoroutine(UseUnitSkillAI(aiTree.skillName, aiTree.aiUnit, aiTree.aiTarget));
        Data = true;
        yield return 0;
    }

    IEnumerator UseUnitSkillAI(string skillName, Unit aiUnit, Unit target)
    {

        yield return StartCoroutine(AIPublicFunc.TurnToAI(aiUnit, target));

        aiTree.outline.RenderOutLine(aiUnit.transform);

        aiUnit.GetComponent<CharacterAction>().SetSkill(skillName);

        UnitSkill unitSkill = SkillManager.GetInstance().skillQueue.Peek().Key as UnitSkill;
        aiTree.rtsCamera.FollowTarget(target.transform.position);
        yield return new WaitForSeconds(0.5f);
        unitSkill.Init(aiUnit.transform);
        unitSkill.Focus(target.transform.position);

        yield return new WaitForSeconds(0.5f);

        aiTree.outline.CancelRender();
        unitSkill.Confirm();
        yield return new WaitUntil(() => { return unitSkill.complete == true; });
        aiTree.rtsCamera.FollowTarget(aiUnit.transform.position);
        yield return 0;
    }

}
