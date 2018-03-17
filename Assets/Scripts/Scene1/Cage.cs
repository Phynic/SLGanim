using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class Cage : MonoBehaviour {
    private int weakPoint = 2;
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
        //固定弱点
        strongPoint = weakPoint + 4;

        //if(strongPoint >= 12)
        //{
        //    strongPoint = strongPoint % 12;
        //}
        
    }

    private void Start()
    {
        
        RoundManager.GetInstance().GameStarted += OnGameStarted;
    }

    private void OnGameStarted(object sender, EventArgs e)
    {
        for (int i = 1; i < 4; i++)
        {
            ChangeData.ChangeValue(rocks[(weakPoint + i) % 8].transform, "def", rocks[(weakPoint + i) % 8].attributes.Find(d => d.eName == "def").value + 10 * i);
            ChangeData.ChangeValue(rocks[(weakPoint + 8 - i) % 8].transform, "def", rocks[(weakPoint + 8 - i) % 8].attributes.Find(d => d.eName == "def").value + 10 * i);
        }
        ChangeData.ChangeValue(rocks[strongPoint].transform, "def", 50);
    }
    
}
