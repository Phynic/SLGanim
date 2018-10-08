using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthStyleDorodomuBarrier : AttackSkill {

    public override void SetLevel(int level)
    {
        customizedRangeList.Clear();
        customizedHoverRangeList.Clear();
        var v = character.position + character.forward * 5;
        v = new Vector3((int)v.x + 0.5f, 0, (int)v.z + 0.5f);
        customizedRangeList.Add(v);
        skipDodge = true;
        customizedHoverRangeList = CreateHoverRangeList();
        rotateToPathDirection = false;
    }


    //正方形去除四角的范围。
    List<Vector3> CreateHoverRangeList()
    {
        var p = character.position + character.forward * 5;
        p = new Vector3((int)p.x + 0.5f, 0, (int)p.z + 0.5f);
        List<Vector3> list = new List<Vector3>();
        //这个数组存放每一行（或者是每一列）应有的方块数。
        int[] num = new int[2 * hoverRange + 1];
        //这个数组应该是奇数个元素个数。
        for (int i = 0; i < 2 * hoverRange + 1; i++)
        {
            if(i == 0 || i == 2 * hoverRange)
                num[i] = 2 * hoverRange - 1;
            else
                num[i] = 2 * hoverRange + 1;
        }
        //根据这个数组，来创建范围。遍历的两个维度，一个是数组的长度，即行数，另一个是数组每一个值，即每一行应该有的方块数。
        for (int i = 0; i < num.Length; i++)
        {
            for (int j = 0; j < num[i]; j++)
            {
                //根据range、i、j、角色position算出每块地板的坐标。
                //中心点为transform.position，个别列有偏移量。
                float rX = p.x + (hoverRange - i);
                float rZ;
                if (i == 0 || i == num.Length - 1)
                    rZ = p.z + (hoverRange - j) - 1;
                else
                    rZ = p.z + (hoverRange - j);
                if (rX < 1 || rZ < 1 || rX > BattleFieldManager.GridX - 1 || rZ > BattleFieldManager.GridY - 1)//超出边界的不创建  由于A*报数组下标越界（因为要检测一个单元周围的所有单元，而边界单元没有完整的周围单元），所以这里把边界缩小一圈。
                {
                    continue;
                }
                Vector3 position = new Vector3(rX, p.y, rZ);
                list.Add(position);
            }
        }
        return list;
    }

    public override void Effect()
    {
        base.Effect();
        FXManager.GetInstance().Spawn("Chakra_Dorodomu_Hand", character.position, 2.8f);
    }

    public override void GetHit()
    {
        foreach (var o in other)
        {
            for (int i = 0; i < hit; i++)
            {
                RoundManager.GetInstance().Invoke(() => {
                    if (o)
                    {
                        if (o.GetComponent<Animator>())
                        {
                            FXManager.GetInstance().Spawn("Chakra_Dorodomu", o.position, o.rotation, 3.2f);
                            FXManager.GetInstance().HitPointSpawn(o.GetComponent<Animator>().GetBoneTransform(HumanBodyBones.Chest).position, Quaternion.identity, null, 0);
                            //不计算受击角度。
                            //o.GetComponent<Animator>().SetFloat("HitAngle", Vector3.SignedAngle(o.position - character.position, -o.forward, Vector3.up));
                            o.GetComponent<Animator>().Play("GetHit", 0, i == 0 ? 0 : 0.2f);
                        }
                    }
                }, 0.2f * i);
            }
        }

    }

    //后处理附加效果
    protected override void PostEffect(Transform o)
    {
        var damageMP = 1;
        var currentMp = o.GetComponent<CharacterStatus>().attributes.Find(d => d.eName == "mp").value;
        //DebugLogPanel.GetInstance().Log(damageMP.ToString() + " MP" + "（" + character.GetComponent<CharacterStatus>().roleCName + " -> " + o.GetComponent<CharacterStatus>().roleCName + "）");
        var mp = currentMp - damageMP;
        ChangeData.ChangeValue(o, "mp", mp);
        UIManager.GetInstance().FlyNum(o.GetComponent<CharacterStatus>().arrowPosition / 2 + o.position + Vector3.down * 0.3f, "-" + damageMP.ToString(), new Color(80f / 255f, 248f / 255f, 144f / 255f));
    }
}
