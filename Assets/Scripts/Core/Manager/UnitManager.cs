using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UnitManager : SingletonComponent<UnitManager>
{
    //在GameStart后准备完毕
    public List<Unit> units;

    private void Start()
    {
        if(SceneManager.GetActiveScene().name == "Main")
        {
            InitUnits();
        }
    }

    public void InitUnits()
    {
        var temp = FindObjectsOfType<Unit>();
        foreach (var u in temp)
        {
            units.Add(u);
        }

        units.ForEach(u => u.GetComponent<Unit>().UnitClicked += Controller_Main.GetInstance().OnUnitClicked);
    }

    public void AddUnit(Unit unit)
    {
        units.Add(unit);
        unit.UnitSelected += UIManager.GetInstance().OnUnitSelected;
        RoundManager.GetInstance().AddUnit(unit);
    }
}