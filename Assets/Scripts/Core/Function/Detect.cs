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

    public static List<List<Transform>> DetectObjects(List<Vector3> rangeList)
    {
        //var rangeList = Range.CreateRange(range, position);
        List<List<Transform>> result = new List<List<Transform>>();
        foreach (var l in rangeList)
        {
            result.Add(DetectObject(l));
        }
        return result;
    }
}
