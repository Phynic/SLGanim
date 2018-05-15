using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Range {
    protected float anchorPoint = 0.5f;        //每一个移动格子的锚点（0.5,0.5）
    protected Transform character;
    protected Quaternion startRotation;
    public Dictionary<Vector3, GameObject> rangeDic = new Dictionary<Vector3, GameObject>();    //此次范围的字典。是否应该放在继承类中？ //I think it's put here is a right choice. (besides, GameObjects are all Floors so far.)
    protected List<Vector3> floorAroundEnemy = new List<Vector3>();
    public List<Vector3> enemyFloor = new List<Vector3>();
    public List<Vector3> mateFloor = new List<Vector3>();
    protected List<Vector3> obstacleFloor = new List<Vector3>();
    int[,] array = new int[BattleFieldManager.GridX, BattleFieldManager.GridY];     //可优化为更小的地图。
    protected BattleFieldManager BFM;
    public Range()
    {
        BFM = BattleFieldManager.GetInstance();
    }

    //返回一个列表，包含以p为中心大小为range的菱形范围的所有Vector3。
    public static List<Vector3> CreateRange(int range, Vector3 p)
    {
        List<Vector3> list = new List<Vector3>();
        //这个数组存放每一行（或者是每一列）应有的方块数，例如：如果range = 3，则数组应为{1,3,5,7,5,3,1}。
        int[] num = new int[2 * range + 1];
        //这个数组应该是奇数个元素个数，并且以中间元素为轴对称。
        for (int i = 0; i < range + 1; i++)
        {
            num[i] = 2 * i + 1;
            num[2 * range - i] = num[i];
        }
        //根据这个数组，来创建范围。遍历的两个维度，一个是数组的长度，即行数，另一个是数组每一个值，即每一行应该有的方块数。
        for (int i = 0; i < num.Length; i++)
        {
            for (int j = 0; j < num[i]; j++)
            {
                //根据range、i、j、角色position算出每块地板的坐标。
                //中心点为transform.position，每一列应有一个偏移量，使最终显示结果为菱形而不是三角形。
                float rX = p.x + (range - i);
                float rZ = p.z + (range - j) - Mathf.Abs(range - i);

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

    protected List<Vector3> CreateStraightRange(int range, Vector3 p)
    {
        List<Vector3> list = new List<Vector3>();
        int[] num = new int[2 * range + 1];

        for (int i = 0; i < range; i++)
        {
            num[i] = 1;
            num[2 * range - i] = num[i];
        }
        num[range] = 2 * range + 1;
        //根据这个数组，来创建范围。遍历的两个维度，一个是数组的长度，即行数，另一个是数组每一个值，即每一行应该有的方块数。
        for (int i = 0; i < num.Length; i++)
        {
            for (int j = 0; j < num[i]; j++)
            {
                //根据range、i、j、角色position算出每块地板的坐标。
                //中心点为transform.position，每一列应有一个偏移量。
                float rX = p.x + (range - i);
                float rZ;
                if (i != range)
                {
                    rZ = p.z + (range - j) - range;
                }
                else
                {
                    rZ = p.z + (range - j);
                }
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

    //调用A*算法
    
    List<Point> path = new List<Point>();
    Vector3 floorPosition = new Vector3();
    protected List<Point> UseAstar(Vector3 origin, Vector3 destination, int range)
    {
        path.Clear();
        for (int i = 0; i < BattleFieldManager.GridX; i++)
        {
            for (int j = 0; j < BattleFieldManager.GridY; j++)
            {
                array[i, j] = 1;
            }
        }

        var minX = origin.x - range;
        var minY = origin.z - range;
        var maxX = origin.x + range;
        var maxY = origin.z + range;
        //检测整张地图的地板块激活情况，判断障碍物，存入数组。
        
        for (int i = (int)minX; i <= (int)maxX; i++)
        {
            for (int j = (int)minY; j <= (int)maxY; j++)
            {
                floorPosition = Vector3.zero;
                floorPosition.x += (i + anchorPoint);
                floorPosition.z += (j + anchorPoint);
                GameObject floor = BFM.GetFloor(floorPosition);
                if (floor && floor.activeSelf && (floor.transform.position == destination || !floorAroundEnemy.Contains(floor.transform.position)))   //地板是激活的且不在敌人周围，可用作寻路路径。如果是目的地且在激活的状态下，一定可以作为寻路路径。
                {
                    array[i, j] = 0;
                }
                else
                {
                    array[i, j] = 1;
                }
            }
        }
        //调用A*。
        Astar astar = new Astar(array);
        Point start = new Point((int)origin.x, (int)origin.z);
        Point end = new Point((int)destination.x, (int)destination.z);

        var parent = astar.FindPath(start, end, false);

        while (parent != null)
        {
            path.Add(parent);
            parent = parent.ParentPoint;
        }
        path.Reverse();     //里面的元素顺序是反的，即从目的指向起点，这里变为由起点指向目的。
        return path;
    }

    protected bool DetectObstacle(Vector3 position)
    {
        var cols = Detect.DetectObject(position);
        foreach (var col in cols)
        {
            if (col.parent)
            {
                if (col.parent.GetComponent<Obstacle>())
                    return true;
            }
        }
        return false;
    }

    //将计算出的路径坐标列表转换为Vector3类型的列表
    protected List<Vector3> ChangePathType(List<Point> path)
    {
        List<Vector3> v_path = new List<Vector3>();

        foreach (var p in path)
        {
            Vector3 v_p = new Vector3(p.X + anchorPoint, 0f, p.Y + anchorPoint);
            v_path.Add(v_p);
        }
        return v_path;
    }

    protected int CheckEnemy(List<Transform> detectList)
    {
        foreach (var d in detectList)
        {
            if (d.GetComponent<Unit>())      //能获取到角色属性脚本，说明是角色
            {
                if (d.GetComponent<Unit>().playerNumber != character.GetComponent<Unit>().playerNumber)    //角色的playerNumber不一致说明是敌人。
                {
                    return 2;
                }
                else
                {
                    return 1;
                }
            }
        }
        return 0;
    }
    
    public virtual void Reset()
    {
        foreach(var a in rangeDic)
        {
            a.Value.SetActive(false);
        }
        character.rotation = startRotation;
        rangeDic.Clear();
    }

    public void Delete()
    {
        foreach (var a in rangeDic)
        {
            a.Value.SetActive(false);
        }
    }
}
