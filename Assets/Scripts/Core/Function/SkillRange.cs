using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillRange : Range {
    public List<GameObject> hoverRangeList = new List<GameObject>();
    private List<Vector3> customizedHoverRangeList = new List<Vector3>();
    
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
            
            BFM.GetFloor(position).SetActive(true);
            rangeDic.Add(position, BFM.GetFloor(position));
        }

        //剔除障碍物阻挡
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

    public void CreateStraightSkillRange(int range, Transform character, bool aliesObstruct)
    {
        this.character = character;
        startRotation = character.rotation;
        var list = CreateStraightRange(range, character.position);
        var listBuffer = new List<Vector3>();
        foreach (var position in list)
        {
            int check = CheckEnemy(Detect.DetectObject(position));
            if (check > 0)
            {
                if (check == 2)
                    enemyFloor.Add(position);
                else if (check == 1 && aliesObstruct)
                    listBuffer.Add(position);   //技能现在可被友军阻挡。
            }
            else if (DetectObstacle(position))
            {
                obstacleFloor.Add(position);
                continue;
            }
                
            BFM.GetFloor(position).SetActive(true);
            rangeDic.Add(position, BFM.GetFloor(position));
        }

        //直线施法的障碍物遮挡效果
        var buffer = new Dictionary<Vector3, GameObject>();

        
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
                        BFM.GetFloor(floor.transform.position).SetActive(false);
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

    public void CreateCustomizedRange(List<Vector3> customizedRangeList, List<Vector3> customizedHoverRangeList, bool enablePathFinding, Transform character)
    {
        this.character = character;
        startRotation = character.rotation;
        this.customizedHoverRangeList = customizedHoverRangeList;
        foreach (var position in customizedRangeList)
        {
            //检测到障碍物时地块不显示。后期实现水上行走，跳远，树上行走。
            if (DetectObstacle(position))
                continue;
            
            BFM.GetFloor(position).SetActive(true);
            rangeDic.Add(position, BFM.GetFloor(position));
        }
    }

    private void CreateSkillHoverRange(int range, Vector3 p)
    {
        hoverRangeList.Clear();
        List<Vector3> list;
        if (customizedHoverRangeList.Count == 0)
            list = CreateRange(range, p);
        else
            list = customizedHoverRangeList;
        foreach (var position in list)
        {
            if (DetectObstacle(position))
                continue;
            if (!BFM.GetFloor(position).activeSelf)
            {
                BFM.GetFloor(position).SetActive(true);
                BFM.GetFloor(position).GetComponent<Collider>().enabled = false;
            }
            hoverRangeList.Add(BFM.GetFloor(position));
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
