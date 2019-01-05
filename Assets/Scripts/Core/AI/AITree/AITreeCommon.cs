using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AITreeCommon : AITree
{
    public static AITreeCommon inst;
    private void Awake()
    {
        inst = this;
    }

    void Start()
    {
        Structure();
    }

    private void Structure()
    {
        root = aINodeCheckSelfHP;
        root.LChild = aINodeHealSelf;
        root.RChild = aINodeMatesInRange;

        aINodeHealSelf.LChild = aINodeStayRest;
        aINodeHealSelf.RChild = aINodeMoveMedicine;

        aINodeMatesInRange.LChild = aINodeEnemyInRange;
        aINodeMatesInRange.RChild = aINodeCheckMatesHP;

        aINodeEnemyInRange.LChild = aINodeCloseToNearestEnemy;
        aINodeEnemyInRange.RChild = aINodeAttackNearestEnemy;

        aINodeCloseToNearestEnemy.RChild = aINodeMoveAI;

        aINodeAttackNearestEnemy.RChild = aINodeMoveAI;

        aINodeMoveAI.LChild = aINodeAfterSkill;
        aINodeMoveAI.RChild = aINodeChooseSkill;

        aINodeCheckMatesHP.LChild = aINodeHealLowestHPMate;
        aINodeCheckMatesHP.RChild = aINodeEnemyInRange;

        aINodeHealLowestHPMate.LChild = aINodeEnemyInRange;
        aINodeHealLowestHPMate.RChild = aINodeMoveMedicine;

        aINodeMoveMedicine.RChild = aINodeMoveAI;
        aINodeChooseSkill.RChild = aINodeUseSkill;
        aINodeUseSkill.RChild = aINodeAfterSkill;

        aINodeAfterSkill.LChild = aINodeDefend;
        aINodeAfterSkill.RChild = aINodeNoDefend;

    }
}
