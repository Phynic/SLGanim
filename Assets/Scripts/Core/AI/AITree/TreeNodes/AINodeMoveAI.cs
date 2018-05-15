using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AINodeMoveAI : AINode<bool> {

    public override bool Execute()
    {
        StartCoroutine(MoveAI(aiTree.aiUnit, aiTree.moveTarget));
        if (lastNode == AITree.aINodeCloseToNearestEnemy)
            return false;
        else
            return true;
    }

    IEnumerator MoveAI(Unit aiUnit, Vector3 targetFloor)
    {
        //target position is as same as aiUnit, don't need to move
        if (aiUnit.transform.position == targetFloor)
        {
            Debug.Log(aiUnit.name + " don't need to move");
            yield return 0;
        }
        else
        {
            //auto move forward target
            aiUnit.GetComponent<CharacterAction>().SetSkill("Move");
            Move moveSkill = SkillManager.GetInstance().skillQueue.Peek().Key as Move;

            moveSkill.Init(aiUnit.transform);

            GameObject floor = BattleFieldManager.GetInstance().GetFloor(targetFloor);
            moveSkill.Focus(floor.GetComponent<Floor>());
            yield return new WaitForSeconds(0.5f);

            aiTree.outline.CancelRender();
            moveSkill.Confirm();
            yield return new WaitUntil(() => { return moveSkill.skillState == Skill.SkillState.reset; });
            yield return new WaitForSeconds(0.1f);
        }
    }

}
