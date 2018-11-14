using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UnitManager : Singleton<UnitManager>
{
    //在GameStart后准备完毕
    public List<Unit> units;
    
    public void AddUnit(Unit unit)
    {
        units.Add(unit);
        unit.UnitSelected += UIManager.GetInstance().OnUnitSelected;
        RoundManager.GetInstance().AddUnit(unit);
    }
}