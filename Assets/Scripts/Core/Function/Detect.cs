using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Detect {
    public static List<Transform> DetectObject(Vector3 position)
    {
        var list = new List<Transform>();
        RaycastHit[] hits;
        Ray ray = new Ray(position + Vector3.down * 1f, Vector3.up);
        hits = Physics.RaycastAll(ray);
        //Debug.DrawRay(position + Vector3.down * 1f, Vector3.up, Color.white, 1000);
        //Debug.Log(hits.Length);
        foreach (var hit in hits)
        {
            Transform obj = null;
            
            obj = hit.transform;
            if(!list.Contains(obj))
                list.Add(obj);
        }
        return list;
    }

    public static List<List<Transform>> DetectObjects(int range, Vector3 position)
    {
        var rangeList = Range.CreateRange(range, position);
        List<List<Transform>> result = new List<List<Transform>>();
        foreach (var l in rangeList)
        {
            result.Add(DetectObject(l));

            //var list = new List<Transform>();
            //RaycastHit[] hits;
            //Ray ray = new Ray(l + Vector3.up * 1f, Vector3.down);   //划重点。。。这个地方应该用的是rangeList中的坐标
            //hits = Physics.RaycastAll(ray);
            ////Debug.DrawRay(position + Vector3.up * 1f, Vector3.down,Color.white,1000);
            ////Debug.Log(hits.Length);
            //foreach (var hit in hits)
            //{
            //    Transform obj = null;

            //    obj = hit.transform;
            //    if (!list.Contains(obj))
            //        list.Add(obj);
            //}
            //result.Add(list);
        }
        return result;
    }
}
