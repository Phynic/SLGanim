using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MultipleShadowClone : Clone
{
    List<Vector3> clonePos = new List<Vector3>();
    List<Vector3> randomPos = new List<Vector3>();

    public override void SetLevel(int level)
    {
        factor = factor + (level - 1) * (int)growFactor;
        hoverRange = factor;
    }

    public override void Effect()
    {
        base.BaseEffect();
        
        int cloneNum;

        //float cloneRate = Random.Range(0f, 1f);
        //if (cloneRate >= 0 && cloneRate < 0.1f)
        //{
        //    cloneNum = 1;
        //}
        //else if(cloneRate >= 0.1f && cloneRate < 0.6f)
        //{
        //    cloneNum = 2;
        //}
        //else if (cloneRate >= 0.6f && cloneRate < 0.9f)
        //{
        //    cloneNum = 3;
        //}
        //else
        //{
        //    cloneNum = 4;
        //}

        cloneNum = 4;

        randomPos.Clear();
        
        if (clonePos.Count > cloneNum)
        {
            for(int i = 0; i < cloneNum; i++)
            {
                int index = UnityEngine.Random.Range(0, clonePos.Count);
                randomPos.Add(clonePos[index]);
                clonePos.RemoveAt(index);
            }
        }
        else
            randomPos = clonePos;
        
        animator.speed = 0f;

        GameController.GetInstance().Invoke(() => {
            render = character.Find("Render").gameObject;
            FXManager.GetInstance().SmokeSpawn(character.position, character.rotation, null);
            render.SetActive(false);
            if (switchPosition)
            {
                int index = UnityEngine.Random.Range(0, randomPos.Count);
                Vector3 noumenonPos = randomPos[index];
                randomPos.Add(character.position);
                character.position = noumenonPos;
            }
        }, 0.6f);

        for (int i = 0; i < randomPos.Count; i++)
        {
            //把clone改成局部c，就可以传递正确的结果。。。
            var c = GameObject.Instantiate(character.gameObject);

            character.GetComponent<Unit>().UnitEnded += (object a, EventArgs b) => { c.GetComponent<Unit>().OnUnitEnd(); };

            GameController.GetInstance().Invoke(j =>
            {
                FXManager.GetInstance().SmokeSpawn(randomPos[j], character.rotation, null);
                animator.speed = 1f;
            }, 1.4f + i * 0.2f, i);

            GameController.GetInstance().Invoke((j, cl) =>
            {
                GameObject clone = (GameObject)cl;
                clone.transform.position = randomPos[j];

                SetIdentity(clone);

                UnitManager.GetInstance().AddUnit(clone.GetComponent<Unit>());
                clone.GetComponent<Unit>().Buffs.Add(new DirectionBuff());
                clone.GetComponent<Animator>().Play(animator.GetCurrentAnimatorStateInfo(0).fullPathHash, 0, animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
                clone.GetComponent<Animator>().SetInteger("Skill", 0);
                
                render.SetActive(true);
            }, 1.6f + i * 0.2f, i, c);
            
        }
    }
    
    //检测范围内目标，符合条件的可被添加至other容器。AttackSkill中默认选中敌人，有特殊需求请覆盖。
    public override bool Check()
    {
        clonePos.Clear();
        List<Vector3> unitList = new List<Vector3>();
        List<List<Transform>> list;
        List<Vector3> hover = new List<Vector3>();
        foreach (var item in range.hoverRangeList)
        {
            hover.Add(item.transform.position);
        }
        list = Detect.DetectObjects(hover);
        foreach(var listT in list)
        {
            foreach(var t in listT)
            {
                if (t.GetComponent<Unit>())
                {
                    unitList.Add(t.position);
                }
            }
        }

        foreach(var p in hover)
        {
            if (!unitList.Contains(p))
                clonePos.Add(p);
        }

        return clonePos.Count > 0;
    }
}
