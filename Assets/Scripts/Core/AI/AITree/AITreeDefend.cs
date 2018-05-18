using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This strategy tends to defend.
/// That means AI would think about protect self and mates at first.
/// </summary>
public class AITreeDefend : AITree {
    public static AITreeDefend inst;
    private void Awake()
    {
        inst = this;
    }

    void Start() {
        Structure();
    }

    /// <summary>
    /// The struction of this tree is created by me:P
    /// I'll restructure it by ID3 algorithm later on
    /// </summary>
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
        aINodeChooseSkill.RChild = aINodeYellSkill;
        aINodeYellSkill.RChild = aINodeUseSkill;
        aINodeUseSkill.RChild = aINodeAfterSkill;

        aINodeAfterSkill.LChild = aINodeDefend;
        aINodeAfterSkill.RChild = aINodeNoDefend;

    }

}
