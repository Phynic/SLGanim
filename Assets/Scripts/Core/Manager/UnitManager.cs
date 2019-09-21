using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UnitManager : SingletonComponent<UnitManager>
{
    //在GameStart后准备完毕
    public List<Unit> units = new List<Unit>();

    public void InitUnits()
    {
        units.Clear();
        var temp = FindObjectsOfType<Unit>();
        foreach (var u in temp)
        {
            units.Add(u);
        }
    }

    public void AddUnit(Unit unit)
    {
        units.Add(unit);
        unit.UnitSelected += UIManager.GetInstance().OnUnitSelected;
        RoundManager.GetInstance().AddUnit(unit);
    }
}