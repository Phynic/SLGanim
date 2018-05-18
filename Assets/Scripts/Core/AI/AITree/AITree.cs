using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Behaviour Tree for AI
/// </summary>
public class AITree:MonoBehaviour
{
    public Unit aiUnit; //current ai unit which can't be changed
    public Unit aiTarget; //current target unit of aiUnit which would be changed by tree
    public Vector3 moveTarget; //aiUnit moves position in current round
    public string skillName; //aiUnit use what skill
    public MoveRange moveRange; //moveRange will be the same value in a round
    [HideInInspector]
    public RenderBlurOutline outline; //render unit outline
    public RTSCamera rtsCamera; //rtsCamera will follow the aiUnit or its skill
    protected AINode<bool> root; //root node
    private Transform nodePool; //all of nodes put into this pool
    private AINode<bool> lastNodeTmp;
    public static AINodeCheckSelfHP aINodeCheckSelfHP;
    public static AINodeHealSelf aINodeHealSelf;
    public static AINodeMatesInRange aINodeMatesInRange;
    public static AINodeStayRest aINodeStayRest;
    public static AINodeMoveMedicine aINodeMoveMedicine;
    public static AINodeEnemyInRange aINodeEnemyInRange;
    public static AINodeCheckMatesHP aINodeCheckMatesHP;
    public static AINodeCloseToNearestEnemy aINodeCloseToNearestEnemy;
    public static AINodeAttackNearestEnemy aINodeAttackNearestEnemy;
    public static AINodeHealLowestHPMate aINodeHealLowestHPMate;
    public static AINodeDefend aINodeDefend;
    public static AINodeNoDefend aINodeNoDefend;
    public static AINodeMoveAI aINodeMoveAI;
    public static AINodeChooseSkill aINodeChooseSkill;
    public static AINodeYellSkill aINodeYellSkill;
    public static AINodeUseSkill aINodeUseSkill;
    public static AINodeAfterSkill aINodeAfterSkill;

    private void Awake()
    {
        nodePool = transform.Find("NodePool");
        PreloadNode();
    }

    internal IEnumerator ActivateAITree(Unit _aiUnit)
    {
        aiUnit = _aiUnit;
        rtsCamera = Camera.main.GetComponent<RTSCamera>();
        outline = Camera.main.GetComponent<RenderBlurOutline>();

        //create move Range and Delete() it after AINodeMoveAI
        moveRange = new MoveRange();
        moveRange.CreateMoveRange(aiUnit.transform);

        yield return StartCoroutine(ActionNode(root));
    }

    //just excute current node one time and return
    private IEnumerator ActionNode(AINode<bool> aiNode)
    {
        aiNode.lastNode = lastNodeTmp;

        bool isEnter = aiNode.Enter(this); //if false, AITree will ignore current aiNode and execute to its right child

        if (isEnter)
        {
            yield return StartCoroutine(aiNode.Execute());
            lastNodeTmp = aiNode;
        }

        // decide how to do the next step
        // if the node places in the last level(it dosen't have any child node)
        // just excute its behaviour and stop
        if (aiNode.Data)
        {
            if (aiNode.RChild != null)
                yield return StartCoroutine(ActionNode(aiNode.RChild as AINode<bool>));
        }
        else
        {
            if (aiNode.LChild != null)
                yield return StartCoroutine(ActionNode(aiNode.LChild as AINode<bool>));
        }

        yield return 0;
    }

    private void PreloadNode()
    {

        aINodeCheckSelfHP = nodePool.GetComponent<AINodeCheckSelfHP>();
        aINodeHealSelf = nodePool.GetComponent<AINodeHealSelf>();
        aINodeMatesInRange = nodePool.GetComponent<AINodeMatesInRange>();
        aINodeStayRest = nodePool.GetComponent<AINodeStayRest>();
        aINodeMoveMedicine = nodePool.GetComponent<AINodeMoveMedicine>();
        aINodeEnemyInRange = nodePool.GetComponent<AINodeEnemyInRange>();
        aINodeCheckMatesHP = nodePool.GetComponent<AINodeCheckMatesHP>();
        aINodeCloseToNearestEnemy = nodePool.GetComponent<AINodeCloseToNearestEnemy>();
        aINodeAttackNearestEnemy = nodePool.GetComponent<AINodeAttackNearestEnemy>();
        aINodeHealLowestHPMate = nodePool.GetComponent<AINodeHealLowestHPMate>();
        aINodeDefend = nodePool.GetComponent<AINodeDefend>();
        aINodeNoDefend = nodePool.GetComponent<AINodeNoDefend>();
        aINodeMoveAI = nodePool.GetComponent<AINodeMoveAI>();
        aINodeChooseSkill = nodePool.GetComponent<AINodeChooseSkill>();
        aINodeYellSkill = nodePool.GetComponent<AINodeYellSkill>();
        aINodeUseSkill = nodePool.GetComponent<AINodeUseSkill>();
        aINodeAfterSkill = nodePool.GetComponent<AINodeAfterSkill>();
    }

}
