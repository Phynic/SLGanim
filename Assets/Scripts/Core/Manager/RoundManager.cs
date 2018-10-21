using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections;
using UnityEngine.EventSystems;

public class RoundManager : Singleton<RoundManager> {
    /*  状态机划分依据：
     *  1.执行一次和每帧执行之间切换。
     *  2.可以明确的阶段。
     */
    
    public event EventHandler GameStarted;
    public event EventHandler GameEnded;
    public event EventHandler RoundStarted;
    public event EventHandler RoundEnded;
    public event EventHandler TurnStarted;
    public event EventHandler TurnEnded;
    public event EventHandler UnitEnded;
    
    public int NumberOfPlayers { get; private set; }
    
    public RoundState RoundState
    {
        get
        {

            return _roundState;
        }
        set
        {
            
            if (_roundState != null)
            {
                _roundState.OnStateExit();
            }
            
            _roundState = value;
            _roundState.OnStateEnter();
        }
    }

    public Unit CurrentUnit { get; set; }

    public int CurrentPlayerNumber { get; private set; }

    public Transform playersParent;

    public List<Player> Players { get; private set; }
    
    public int roundNumber = 0;

    public float gameStartTime = 2f;                 //状态持续时间。
    public float roundStartTime = 2f;                //状态持续时间。
    public float turnStartTime = 2f;                 //状态持续时间。

    private List<Unit> Units { get; set; }
    private RoundState _roundState;
    private VectoryCondition vc;

    IEnumerator GameStart()
    {
        yield return StartCoroutine(BattlePrepare());
        yield return StartCoroutine(FocusTeamMember());
        if (GameStarted != null)
            GameStarted.Invoke(this, new EventArgs());
        //角色加入忽略层
        Units.ForEach(u => u.gameObject.layer = 2);
        yield return new WaitForSeconds(gameStartTime);
        StartCoroutine(RoundStart());
    }
    
    IEnumerator RoundStart()
    {
        roundNumber++;
        if (RoundStarted != null)
            RoundStarted.Invoke(this, new EventArgs());
        //角色加入忽略层
        Units.ForEach(u => u.gameObject.layer = 2);
        
        yield return new WaitForSeconds(roundStartTime);
        
        Units.ForEach(u => { u.OnRoundStart(); });
        StartCoroutine(TurnStart());
    }

    IEnumerator TurnStart()
    {
        Units.ForEach(u => { u.OnTurnStart(); });
        if (TurnStarted != null)
            TurnStarted.Invoke(this, new EventArgs());
        yield return new WaitForSeconds(turnStartTime);
        
        //角色加入忽略层
        Units.ForEach(u => u.gameObject.layer = 2);

        //剧情对话
        yield return StartCoroutine(DialogManager.GetInstance().PlayDialog(roundNumber,CurrentPlayerNumber));
        
        //角色取出忽略层
        Units.ForEach(u => u.gameObject.layer = 0);

        //这里接一个EndTurn，目的应该是调用里面的Play，来让当前Player行动。
        EndTurn();
    }
    
    public void EndTurn()
    {
        if (CheckGameEnd())
            return;
        if (UnitEnded != null)
            UnitEnded.Invoke(this, null);
        if (Units.FindAll(u => u.playerNumber == CurrentPlayerNumber && u.UnitEnd == false).Count > 0)    //当前玩家仍有角色未操作。
        {
            Players.Find(p => p.playerNumber.Equals(CurrentPlayerNumber)).Play(this);
            
        }
        if (Units.FindAll(u => u.playerNumber == CurrentPlayerNumber && u.UnitEnd == false).Count == 0)   //当前Player的所有Unit执行完毕
        {
            
            CurrentPlayerNumber = (CurrentPlayerNumber + 1) % NumberOfPlayers;
            while (Units.FindAll(u => u.playerNumber.Equals(CurrentPlayerNumber)).Count == 0)
            {
                CurrentPlayerNumber = (CurrentPlayerNumber + 1) % NumberOfPlayers;
            }//Skipping players that are defeated.
            
            Units.ForEach(u => { u.OnTurnEnd(); });
            if (TurnEnded != null)
                TurnEnded.Invoke(this, new EventArgs());

            if (Units.FindAll(u => u.UnitEnd == false).Count == 0)    //所有Player的所有Unit执行完毕
            {
                EndRound();
            }
            else
            {
                StartCoroutine(TurnStart());
            }
        }
    }

    public void EndRound()
    {
        Units.ForEach(u => { u.OnRoundEnd(); });
        if (RoundEnded != null)
            RoundEnded.Invoke(this, new EventArgs());
        StartCoroutine(RoundStart());
    }

    public void ForceEndTurn()
    {
        var list = Units.FindAll(u => u.playerNumber == CurrentPlayerNumber);
        foreach(var u in list)
        {
            u.GetComponent<Unit>().OnUnitEnd();
        }
        EndTurn();
    }

    IEnumerator BattlePrepare()
    {
        yield return null;
    }

    IEnumerator FocusTeamMember()
    {
        //Units Initialize的时间
        yield return new WaitForSeconds(1);

        if (Camera.main.GetComponent<RenderBlurOutline>())
        {
            //Camera.main.GetComponent<RTSCamera>().enabled = false;
            foreach (var player in Players)
            {
                List<Transform> temp = new List<Transform>();
                foreach (var u in Units.FindAll(u => u.playerNumber == player.playerNumber))
                {
                    temp.Add(u.transform);
                }
                Camera.main.GetComponent<RenderBlurOutline>().RenderOutLine(temp);
                yield return new WaitForSeconds(2);
                Camera.main.GetComponent<RenderBlurOutline>().CancelRender();
                yield return new WaitForSeconds(1);
            }
            Camera.main.GetComponent<RenderBlurOutline>().CancelRender();
        }
        //Camera.main.GetComponent<RTSCamera>().enabled = true;
    }

    void Start () {
        Players = new List<Player>();
        
        vc = GetComponent<VectoryCondition>();
        //Units.ForEach(u => { u.Initialize(); }); //调整了SkillManager的执行顺序，放在Default之前，这里把Initialize挪到UnitManager中执行。方便其他场景的角色初始化。
        for (int i = 0; i < playersParent.childCount; i++)
        {
            var player = playersParent.GetChild(i).GetComponent<Player>();
            if (player != null)
            {
                Players.Add(player);
            }
            else
                Debug.LogError("Invalid object in Players Parent game object");
        }
        
        NumberOfPlayers = Players.Count;
        CurrentPlayerNumber = Players.Min(p => p.playerNumber);

        GameController.GetInstance().Invoke(() =>
        {
            Units = UnitManager.GetInstance().units;
            foreach (var unit in Units)
            {
                unit.UnitClicked += OnUnitClicked;
                unit.UnitDestroyed += OnUnitDestroyed;
                //设置同盟列表。
            }
            StartCoroutine(GameStart());
        }, 0.1f);
    }

    private void OnUnitClicked(object sender, EventArgs e)
    {

        //PC端判断如下


        //移动端判断如下
        if (!(Input.touchCount > 0 && EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)) && !EventSystem.current.IsPointerOverGameObject())
        {
            RoundState.OnUnitClicked(sender as Unit);
        }
    }

    private void OnUnitDestroyed(object sender, EventArgs e)
    {
        CheckGameEnd();
    }
    
    private bool CheckGameEnd()
    {
        switch (vc.CheckVectory(Units))
        {
            case 0:
                break;
            case 1:
                Win();
                if (GameEnded != null)
                    GameEnded.Invoke(this, new EventArgs());
                return true;
            case 2:
                Lose();
                if (GameEnded != null)
                    GameEnded.Invoke(this, new EventArgs());
                return true;
        }
        return false;
    }

    private void Win()
    {
        DebugLogPanel.GetInstance().Log("胜利!");
        GameController.GetInstance().Invoke(() => {
            Restart();
        }, 2f);
        
    }
    
    private void Lose()
    {
        DebugLogPanel.GetInstance().Log("失败!");
        GameController.GetInstance().Invoke(() => {
            Restart();
        }, 2f);
    }
    
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Exit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void AddUnit(Unit unit)
    {
        unit.UnitClicked += OnUnitClicked;
        unit.UnitDestroyed += OnUnitDestroyed;
    }
}
