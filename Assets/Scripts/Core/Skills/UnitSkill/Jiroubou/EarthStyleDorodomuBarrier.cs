using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthStyleDorodomuBarrier : AttackSkill {

    public override void SetLevel(int level)
    {
        customizedRangeList.Add(new Vector3(40.5f, 0f, 34.5f));

        customizedHoverRangeList = CreateHoverRangeList();
        rotateToPathDirection = false;
    }

    List<Vector3> CreateHoverRangeList()
    {
        var p = new Vector3(40.5f, 0f, 34.5f);
        List<Vector3> list = new List<Vector3>();
        //这个数组存放每一行（或者是每一列）应有的方块数，例如：如果range = 3，则数组应为{1,3,5,7,5,3,1}。
        int[] num = new int[2 * hoverRange + 1];
        //这个数组应该是奇数个元素个数，并且以中间元素为轴对称。
        for (int i = 0; i < 2 * hoverRange + 1; i++)
        {
            num[i] = 2 * hoverRange + 1;
        }
        //根据这个数组，来创建范围。遍历的两个维度，一个是数组的长度，即行数，另一个是数组每一个值，即每一行应该有的方块数。
        for (int i = 0; i < num.Length; i++)
        {
            for (int j = 0; j < num[i]; j++)
            {
                //根据range、i、j、角色position算出每块地板的坐标。
                //中心点为transform.position，每一列应有一个偏移量，使最终显示结果为菱形而不是三角形。
                float rX = p.x + (hoverRange - i);
                float rZ = p.z + (hoverRange - j);

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

}
