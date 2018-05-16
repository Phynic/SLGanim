using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AINode<T>: Node<T>
{
    public AITree aiTree;
    public AINode<bool> lastNode; //current node comes from which node. PS: some node need to know last node to decide how to do, such as CloseToNearestEnemy ChooseSkill etc.
    /// <summary>
    /// satisfy enter condition or not
    /// need complete later on
    /// </summary>
    public virtual bool Enter(AITree _aiTree) {
        aiTree = _aiTree;
        return true;
    }

    /// <summary>
    /// the return stands for a decision which chooses the next Node to be implemented
    /// if return true, do right child, otherwise do left child
    /// </summary>
    public virtual IEnumerator Execute() {
        yield return 0;
    }

}
