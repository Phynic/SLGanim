using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveRange : Range {
    
    int range;
    //传入范围和创建范围角色的transform.position即可创建。p即transform.position
    public void CreateMoveRange(Transform character)
    {
        this.character = character;
        startRotation = character.rotation;
        range = character.GetComponent<CharacterStatus>().attributes.Find(d => d.eName == "mrg").value;
        if (range < 1)
            return;
        var list = CreateRange(range, character.position);
        //比list大一圈以保证enemy在范围外一圈时范围内颜色的正常显示。
        var detect = CreateRange(range + 1, character.position);    //此处创建范围比应有范围大1，正确的显示效果 是在下面 A*检测到路径但距离大于角色mrg的地块不显示 处得到保证。
        
        
        //检测到敌人时地块不显示，且加入名单。
        foreach (var position in detect)
        {
            if (CheckEnemy(Detect.DetectObject(position)) == 2)
            {
                enemyFloor.Add(position);
                continue;
            }
            if (CheckEnemy(Detect.DetectObject(position)) == 1)
            {
                mateFloor.Add(position);
            }
            //检测到障碍物时地块不显示。后期实现水上行走，跳远，树上行走。
            if (DetectObstacle(position))
                continue;
            
            BattleFieldManager.GetInstance().GetFloor(position).SetActive(true);

            rangeDic.Add(position, BattleFieldManager.GetInstance().GetFloor(position));

        }
        //添加enemyFloor周围的坐标进入容器。
        foreach (var floor in enemyFloor)
        {
            Vector3 V1 = new Vector3(floor.x - 1, 0, floor.z);
            Vector3 V2 = new Vector3(floor.x + 1, 0, floor.z);
            Vector3 V3 = new Vector3(floor.x, 0, floor.z - 1);
            Vector3 V4 = new Vector3(floor.x, 0, floor.z + 1);
            TryAddValue(floorAroundEnemy, V1);
            TryAddValue(floorAroundEnemy, V2);
            TryAddValue(floorAroundEnemy, V3);
            TryAddValue(floorAroundEnemy, V4);
        }
        
        var buffer0 = new List<Vector3>();
        foreach (var floor in rangeDic)
        {
            if (!list.Contains(floor.Key))
            {
                floor.Value.SetActive(false);
                buffer0.Add(floor.Key);
            }
        }
        foreach (var a in buffer0)
        {
            rangeDic.Remove(a);
        }

        //A*检测到路径但距离大于角色mrg的地块不显示。A*无法检测到路径的地块不显示。必须第二次遍历，即创建完成，A*寻路才有意义。
        //不能迭代时改变或删除Dictionary中的元素。
        var buffer = new List<Vector3>();
        foreach (var floor in rangeDic)
        {
            if ((UseAstar(character.position, floor.Key, range).Count > range + 1) || (floor.Key != character.position && UseAstar(character.position, floor.Key, range).Count == 0))
            {
                floor.Value.SetActive(false);
                buffer.Add(floor.Key);
            }
        }
        foreach (var a in buffer)
        {
            rangeDic.Remove(a);
        }
        
        RecoverColor();
    }

    

    public void RecoverColor()
    {
        foreach (var floor in rangeDic)
        {
            if (floor.Value.GetComponent<Renderer>().material != floor.Value.GetComponent<Floor>().yellowFloor)
            {
                floor.Value.GetComponent<Floor>().ChangeRangeColorToBlue();
            }
            KeepYellowFloor();
        }
    }

    //敌人周围四块变为黄色。
    void KeepYellowFloor()
    {
        if (floorAroundEnemy.Count != 0)
        {
            foreach (var a in floorAroundEnemy)
            {
                if (BattleFieldManager.GetInstance().GetFloor(a).GetComponent<Floor>().isActiveAndEnabled)
                {
                    BattleFieldManager.GetInstance().GetFloor(a).GetComponent<Floor>().ChangeRangeColorToYellow();
                }
            }
        }
    }

    void TryAddValue(List<Vector3> list, Vector3 value)
    {
        if (!list.Contains(value))
        {
            list.Add(value);
        }
    }

    public List<Vector3> CreatePath(Vector3 destination)
    {
        //移动时只需要拐点。
        var finalPath = ChangePathType(InflectionPoint(UseAstar(character.position, destination, range), destination));
        return finalPath;
    }
    
    //提取路径中拐点保存
    List<Point> InflectionPoint(List<Point> path, Vector3 destination)
    {
        List<Point> inflectionPoint = new List<Point>();
        for (int i = 1; i < path.Count - 1; i++)
        {
            //拐点特性，（父子X不同，子孙Y不同）或（父子Y不同，子孙X不同）。
            if ((path[i - 1].X == path[i].X && path[i].X != path[i + 1].X) || (path[i - 1].Y == path[i].Y && path[i].Y != path[i + 1].Y))
            {
                inflectionPoint.Add(path[i]);
            }
        }
        inflectionPoint.Add(new Point((int)destination.x, (int)destination.z));
        return inflectionPoint;
    }
    
    public void ExcuteChangeRoadColorAndRotate(Vector3 position)
    {
        //显示路径需要每一个路径点。
        RecoverColor();
        if (rangeDic.ContainsKey(position))
        {
            List<Vector3> pathList = ChangePathType(UseAstar(character.position, position, range));
            if (pathList.Count > 1)
            {
                Quaternion wantedRot = Quaternion.LookRotation(pathList[1] - character.position);
                character.rotation = wantedRot;
            }
            //这里可能会有问题，先调用变蓝，再调用变红，不知为何是管用的，可能是覆盖。
            foreach (var f in pathList)
            {
                GameObject p = BattleFieldManager.GetInstance().GetFloor(f);
                if (p != null)
                {
                    p.GetComponent<Floor>().ChangeRangeColorToRed();
                }
            }
            BattleFieldManager.GetInstance().GetFloor(character.position).GetComponent<Floor>().ChangeRangeColorToRed();
        }
    }
}
