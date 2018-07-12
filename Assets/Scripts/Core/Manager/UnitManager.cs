using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour {
    public static UnitManager instance;
    public List<Unit> units;

    public static UnitManager GetInstance()
    {
        return instance;
    }

    void Awake()
    {
        instance = this;
        var temp = FindObjectsOfType<Unit>();
        foreach(var u in temp)
        {
            units.Add(u);
        }
    }

    private void Start()
    {
        units.ForEach(u => { u.Initialize(); });
    }

    public void AddUnit(Unit unit)
    {
        units.Add(unit);
        unit.UnitSelected += UIManager.GetInstance().OnUnitSelected;
        RoundManager.GetInstance().AddUnit(unit);
    }
}
