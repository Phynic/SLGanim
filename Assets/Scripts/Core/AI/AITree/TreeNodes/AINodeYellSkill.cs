using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AINodeYellSkill : AINode<bool> {

    public override IEnumerator Execute()
    {
        yield return StartCoroutine(DramaYellSkill(aiTree.aiUnit, aiTree.skillName));
        Data = true;
        yield return 0;
    }

    private IEnumerator DramaYellSkill(Unit aiUnit, string skillName)
    {
        DramaYellSkill dys = AIManager.GetInstance().AIDrama.GetComponent<DramaYellSkill>();
        dys.skillName = skillName;
        dys.unit = aiUnit;
        yield return StartCoroutine(dys.Play());
    }

}
