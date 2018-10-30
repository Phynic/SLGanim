using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteelShuriken : Shuriken
{
    public override void SetLevel(int level)
    {
        damageFactor = damageFactor + (level - 1) * (int)growFactor;
        hoverRange = 1;
        switch (level)
        {
            case 2:
                _cName = "强八方手里剑";
                break;
            case 3:
                _cName = "超力八方手里剑";
                break;
        }
    }
}
