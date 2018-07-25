using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        if (SceneManager.GetActiveScene().name == "Main")
            GameController.GetInstance().Invoke(() => { units.ForEach(u => { u.Initialize(); }); }, 1f);
        else
            units.ForEach(u => { u.Initialize(); });
    }

    public void AddUnit(Unit unit)
    {
        units.Add(unit);
        unit.UnitSelected += UIManager.GetInstance().OnUnitSelected;
        RoundManager.GetInstance().AddUnit(unit);
    }
}
