using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillRange : Range {
    public List<GameObject> hoverRangeList = new List<GameObject>();
    
    public void CreateSkillRange(int range, Transform character)
    {
        this.character = character;
        startRotation = character.rotation;
        var list = CreateRange(range, character.position);
        foreach (var position in list)
        {
            //检测到障碍物时地块不显示。后期实现水上行走，跳远，树上行走。
            if (DetectObstacle(position))
                continue;
            
            BattleFieldManager.GetInstance().GetFloor(position).SetActive(true);
            rangeDic.Add(position, BattleFieldManager.GetInstance().GetFloor(position));
        }

        var buffer = new List<Vector3>();
        foreach (var floor in rangeDic)
        {
            if (floor.Key != character.position && UseAstar(character.position, floor.Key, range).Count == 0)
            {
                floor.Value.SetActive(false);
                buffer.Add(floor.Key);
            }
        }
        foreach (var a in buffer)
        {
            rangeDic.Remove(a);
        }
    }

    public void CreateStraightSkillRange(int range, Transform character)
    {
        this.character = character;
        startRotation = character.rotation;
        var list = CreateStraightRange(range, character.position);

        foreach (var position in list)
        {
            if (CheckEnemy(Detect.DetectObject(position)))
            {
                enemyFloor.Add(position);
            }
            else if (DetectObstacle(position))
            {
                obstacleFloor.Add(position);
                continue;
            }
                
            BattleFieldManager.GetInstance().GetFloor(position).SetActive(true);
            rangeDic.Add(position, BattleFieldManager.GetInstance().GetFloor(position));
        }

        //直线施法的障碍物遮挡效果
        var buffer = new Dictionary<Vector3, GameObject>();

        var listBuffer = new List<Vector3>();
        foreach(var a in enemyFloor)
        {
            listBuffer.Add(a);
        }

        foreach(var a in obstacleFloor)
        {
            if (!listBuffer.Contains(a))
            {
                listBuffer.Add(a);
            }
        }

        foreach (var floor in rangeDic.Values)
        {
            foreach (var f in listBuffer)
            {
                if (floor.activeInHierarchy)
                {
                    var dis = floor.transform.position - character.position;
                    var eDis = f - character.position;
                    //两向量方向相同，且dis距离大于eDis，则不显示。即被遮挡住。
                    if ((dis.normalized == eDis.normalized) && (dis.magnitude > eDis.magnitude))
                    {
                        BattleFieldManager.GetInstance().GetFloor(floor.transform.position).SetActive(false);
                        buffer.Add(floor.transform.position, floor);
                    }
                }
            }
        }
        foreach (var pair in buffer)
        {
            rangeDic.Remove(pair.Key);
        }
    }

    public void CreateSkillHoverRange(int range, Vector3 p)
    {
        hoverRangeList.Clear();
        var list = CreateRange(range, p);
        foreach (var position in list)
        {
            if (DetectObstacle(position))
                continue;
            if (!BattleFieldManager.GetInstance().GetFloor(position).activeSelf)
            {
                BattleFieldManager.GetInstance().GetFloor(position).SetActive(true);
                BattleFieldManager.GetInstance().GetFloor(position).GetComponent<Collider>().enabled = false;
            }
            hoverRangeList.Add(BattleFieldManager.GetInstance().GetFloor(position));
        }
    }

    public void ExcuteChangeColorAndRotate(int hoverRange, int skillRange, Vector3 position, bool rotate)
    {
        //这里可能会有问题，先调用变红，再调用变黄，不知为何是管用的，可能是覆盖。
        RecoverColor();
        List<Vector3> pathList = ChangePathType(UseAstar(character.position, position, skillRange));
        if (rotate)
        {
            if (position != character.position)
            {
                Quaternion wantedRot = Quaternion.LookRotation(pathList[1] - character.position);
                character.rotation = wantedRot;
            }
        }
        CreateSkillHoverRange(hoverRange, position);
        foreach (var a in hoverRangeList)
        {
            a.GetComponent<Floor>().ChangeRangeColorToYellow();
        }
    }

    public void RecoverColor()
    {
        foreach (var f in rangeDic.Values)
        {
            f.GetComponent<Floor>().ChangeRangeColorToRed();
        }
    }

    public void DeleteHoverRange()
    {
        foreach (var a in hoverRangeList)
        {
            GameObject value;
            if (!rangeDic.TryGetValue(a.transform.position, out value))
            {
                a.GetComponent<Collider>().enabled = true;
                a.SetActive(false);
            }
        }
        hoverRangeList.Clear();
    }

    public override void Reset()
    {
        DeleteHoverRange();
        base.Reset();
    }
}
