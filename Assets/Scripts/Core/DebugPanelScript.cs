using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugPanelScript : MonoBehaviour {

    private float height = 0f;                    //角色锚点高度
    private RaycastHit[] hits;
    private RaycastHit hit;
    private List<Unit> unit = new List<Unit>();
    private Text debugText;
    void GetMousePosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out hit);
    }

    Vector3 HandleMouse()
    {
        Vector3 pos;
        int x = (int)(hit.point.x);
        int y = (int)(hit.point.z);
        float GridX = x;
        float GridY = y;
        GridX += BattleFieldManager.anchorPoint;
        GridY += BattleFieldManager.anchorPoint;
        pos = new Vector3(GridX, height, GridY);
        return pos;
    }

    void LogMessage()
    {
        String s = null;
        if (unit.Count == 1)
        {
            
            if (unit[0] is CharacterStatus)
            {
                var c = (CharacterStatus)unit[0];
                s += c.roleCName + "\n";
                s += c.GetIdentity() + "\n";
            }
            else
            {
                s += unit[0].name + "\n";
            }
            
            foreach (var a in unit[0].attributes)
            {
                s += a.cName + " : " + a.value + " / " + a.valueMax + "\n";
            }
        }
        debugText.text = s;
    }

    // Use this for initialization
    void Start () {
        debugText = GetComponentInChildren<Text>();
	}
	
    List<Unit> Selectunit(List<Transform> mouseHover)
    {
        var list = new List<Unit>();
        foreach(var a in mouseHover)
        {
            var c = a.GetComponent<Unit>();
            if (c != null)
            {
                list.Add(c);
            }
        }
        return list;
    }

	// Update is called once per frame
	void Update () {
        GetMousePosition();
        unit = Selectunit(Detect.DetectObject(HandleMouse()));
        LogMessage();
	}
}
