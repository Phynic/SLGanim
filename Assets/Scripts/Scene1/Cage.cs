using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class Cage : MonoBehaviour {
    private int weakPoint = 3;
    private int strongPoint;
    private List<CharacterStatus> rocks = new List<CharacterStatus>();

    private void Awake()
    {
        var temp = GetComponentsInChildren<CharacterStatus>();
        foreach(var c in temp)
        {
            rocks.Add(c);
        }
        
        //weakPoint = UnityEngine.Random.Range(0, 12);

        strongPoint = weakPoint + 6;
        if(strongPoint >= 12)
        {
            strongPoint = strongPoint % 12;
        }
        
    }

    private void Start()
    {
        RoundManager.GetInstance().UnitEnded += OnUnitEnded;
        RoundManager.GetInstance().GameStarted += OnGameStarted;
    }

    private void OnGameStarted(object sender, EventArgs e)
    {
        for (int i = 1; i < 6; i++)
        {
            ChangeData.ChangeValue(rocks[(weakPoint + i) % 12].transform, "def", rocks[(weakPoint + i) % 12].attributes.Find(d => d.eName == "def").value + 10 * i);
            ChangeData.ChangeValue(rocks[(weakPoint + 12 - i) % 12].transform, "def", rocks[(weakPoint + 12 - i) % 12].attributes.Find(d => d.eName == "def").value + 10 * i);
        }
        ChangeData.ChangeValue(rocks[strongPoint].transform, "def", 70);
    }

    private void OnUnitEnded(object sender, EventArgs e)
    {
        //foreach(var c in rocks)
        //{
        //    ChangeData.ChangeValue(c.transform, "hp", c.attributes.Find(d => d.eName == "hp").valueMax);
        //}
    }
}
