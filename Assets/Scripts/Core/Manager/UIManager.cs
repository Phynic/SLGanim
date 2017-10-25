using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class UIManager : MonoBehaviour {
    public static UIManager instance;
    public static UIManager GetInstance()
    {
        return instance;
    }
    
    private Transform character;
    
    private List<Transform> UI = new List<Transform>();
    

    

    public void OnGameStart(object sender, EventArgs e)
    {
        StartCoroutine(OnGameStart());
    }

    public IEnumerator OnGameStart()
    {
        UI.Find(g => g.name == "GameStart").gameObject.SetActive(true);
        yield return new WaitForSeconds(RoundManager.GetInstance().gameStartTime);
        UI.Find(g => g.name == "GameStart").gameObject.SetActive(false);
    }

    public void OnRoundStart(object sender, EventArgs e)
    {
        StartCoroutine(OnRoundStart());
    }

    public void OnTurnStart(object sender, EventArgs e)
    {
        StartCoroutine(OnTurnStart());
    }

    public void OnUnitSelected(object sender, EventArgs e)
    {
        character = (sender as Unit).transform;
        character.GetComponent<CharacterAction>().SetSkill("FirstAction");
    }
    
    public IEnumerator OnRoundStart()
    {
        var go = UI.Find(g => g.name == "GameStart").gameObject;
        go.GetComponentInChildren<Text>().text = "第" + RoundManager.GetInstance().roundNumber + "回合";
        go.SetActive(true);
        yield return new WaitForSeconds(RoundManager.GetInstance().roundStartTime);
        go.SetActive(false);
    }

    public IEnumerator OnTurnStart()
    {
        var go = UI.Find(g => g.name == "GameStart").gameObject;
        if(RoundManager.GetInstance().CurrentPlayerNumber == 0)
        {
            go.GetComponentInChildren<Text>().text = "我方回合";
        }
        else
        {
            go.GetComponentInChildren<Text>().text = "敌方回合";
        }
        go.SetActive(true);
        yield return new WaitForSeconds(RoundManager.GetInstance().roundStartTime);
        go.SetActive(false);
    }

    public void OnRoundEnd(object sender, EventArgs e)
    {
        
    }

    void Awake()
    {
        instance = this;
    }

    void Start () {
        
        RoundManager.GetInstance().GameStarted += OnGameStart;
        RoundManager.GetInstance().RoundStarted += OnRoundStart;
        RoundManager.GetInstance().RoundEnded += OnRoundEnd;
        RoundManager.GetInstance().TurnStarted += OnTurnStart;

        UnitManager.GetInstance().units.ForEach(u => u.GetComponent<Unit>().UnitSelected += OnUnitSelected);

        UI.Add(GameObject.Find("GameStart").transform);
        UI.Add(GameObject.Find("RoundStart").transform);
        UI.Add(GameObject.Find("TurnStart").transform);

        GameObject.Find("GameStart").SetActive(false);
        GameObject.Find("RoundStart").SetActive(false);
        GameObject.Find("TurnStart").SetActive(false);
    }
	
	void Update () {
        //GetMousePosition();

        if (character)
        {
            if (Input.GetMouseButtonDown(1))
            {
                if (SkillManager.GetInstance().skillQueue.Count > 0)
                {
                    //Debug.Log("UIManager : " + SkillManager.GetInstance().skillQueue.Peek().Key.CName + " 队列剩余 " + SkillManager.GetInstance().skillQueue.Count);
                    SkillManager.GetInstance().skillQueue.Peek().Key.Reset();
                }
            }
        }
    }
}
