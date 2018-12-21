using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MultipleShadowClone : ShadowClone
{
    List<Vector3> clonePos = new List<Vector3>();       //可放置clone的位置集合
    List<GameObject> clones = new List<GameObject>();

    public override void SetLevel(int level)
    {
        factor = factor + (level - 1) * (int)growFactor;
        hoverRange = factor;
    }

    public override void Effect()
    {
        base.BaseEffect();
        
        int cloneNum;

        float cloneRate = UnityEngine.Random.Range(0f, 1f);

        if (cloneRate >= 0 && cloneRate < 0.1f)
        {
            cloneNum = 1;
        }
        else if (cloneRate >= 0.1f && cloneRate < 0.6f)
        {
            cloneNum = 2;
        }
        else if (cloneRate >= 0.6f && cloneRate < 0.9f)
        {
            cloneNum = 3;
        }
        else
        {
            cloneNum = 4;
        }

        List<Vector3> randomPos = new List<Vector3>();

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
        }, 0.6f);

        clones.Clear();

        for (int i = 0; i < randomPos.Count; i++)
        {
            //把clone改成局部c，就可以传递正确的结果。。。
            var c = GameObject.Instantiate(character.gameObject);
            
            SetIdentity(c);
            UnitManager.GetInstance().AddUnit(c.GetComponent<Unit>());
            c.GetComponent<Unit>().Buffs.Add(new DirectionBuff());
            clones.Add(c);
        }

        character.GetComponent<Unit>().UnitEnded += SetClonesEnd;

        int seed = UnityEngine.Random.Range(0, randomPos.Count);
        randomPos.Insert(seed, character.position);
        if(switchPosition)
            clones.Insert(UnityEngine.Random.Range(0, clones.Count), character.gameObject);
        else
            clones.Insert(seed, character.gameObject);
        for (int i = 0; i < clones.Count; i++)
        {
            GameController.GetInstance().Invoke(j =>
            {
                FXManager.GetInstance().SmokeSpawn(randomPos[j], character.rotation, null);
            }, 1.4f + i * 0.15f, i);

            GameController.GetInstance().Invoke(j =>
            {
                clones[j].transform.position = randomPos[j];
                if (clones[j] == character.gameObject)
                {
                    render.SetActive(true);
                }
                else
                {
                    clones[j].GetComponent<Animator>().Play(animator.GetCurrentAnimatorStateInfo(0).fullPathHash, 0, animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
                    clones[j].GetComponent<Animator>().SetInteger("Skill", 0);
                }
                clones[j].GetComponent<Animator>().speed = 0f;
            }, 1.6f + i * 0.15f, i);
        }


        GameController.GetInstance().Invoke(() =>
        {
            foreach(var c in clones)
            {
                c.GetComponent<Animator>().speed = 1;
            }
        }, 2f + clones.Count * 0.15f);

    }
    
    void SetClonesEnd(object sender, EventArgs e)
    {
        for(int i = 0; i < clones.Count; i++)
        {
            if (clones[i] == character.gameObject)
                continue;
            clones[i].GetComponent<Unit>().OnUnitEnd();
        }

        character.GetComponent<Unit>().UnitEnded -= SetClonesEnd;

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
