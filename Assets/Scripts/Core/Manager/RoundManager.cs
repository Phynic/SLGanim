using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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

    [HideInInspector]
    public bool gameEnded;

    public Unit CurrentUnit { get; set; }

    public int CurrentPlayerNumber { get; private set; }

    private Transform playersParent;

    public List<Player> Players { get; private set; }

    public bool BattleBegin { get; set; }

    public int roundNumber = 0;

    public float gameStartTime = 2f;                 //状态持续时间。
    public float roundStartTime = 2f;                //状态持续时间。
    public float turnStartTime = 2f;                 //状态持续时间。
    public float focusTime = 2f;                 //状态持续时间。
    private List<Unit> Units { get; set; }
    private RoundState _roundState;
    private Transform level;
    private VectoryCondition vc;
    public GameObject battlePrepare;

    IEnumerator GameStart()
    {
        yield return StartCoroutine(LoadLevel());
        yield return StartCoroutine(BattlePrepare());
        yield return StartCoroutine(FocusTeamMember());
        gameEnded = false;
        if (GameStarted != null)
            GameStarted.Invoke(this, new EventArgs());
        //角色加入忽略层
        Units.ForEach(u => u.gameObject.layer = 2);
        yield return new WaitForSeconds(gameStartTime);
        
        foreach (var unit in Units)
        {
            unit.UnitClicked += OnUnitClicked;
            unit.UnitDestroyed += OnUnitDestroyed;
            //设置同盟列表。
        }

        Units.ForEach(u => { u.Initialize(); }); //战斗场景角色初始化。

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
        yield return StartCoroutine(DialogManager.GetInstance().PlayDialog(roundNumber, CurrentPlayerNumber));
        
        //角色取出忽略层
        Units.ForEach(u => u.gameObject.layer = 0);

        //这里接一个EndTurn，目的应该是调用里面的Play，来让当前Player行动。
        EndTurn();
    }
    
    public void EndTurn()
    {
        Resources.UnloadUnusedAssets();
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

    IEnumerator LoadLevel()
    {
        //LoadPrefab
        var r = Resources.LoadAsync("Prefabs/Level/Level_" + Global.GetInstance().IndexToString(Global.GetInstance().BattleIndex));
        yield return r;

        //LevelInit
        var go = Instantiate(r.asset) as GameObject;
        level = go.transform;
        level.name = r.asset.name;
        var rtsCamera = Camera.main.GetComponent<RTSCamera>();
        rtsCamera.cameraRange = level.Find("CameraRange").gameObject;
        rtsCamera.enabled = true;

        //VectoryCondition
        vc = level.GetComponent<VectoryCondition>();

        //LoadUnits
        var characterParent = level.Find("Characters");
        var spawnPointParent = level.Find("SpawnPoints");
        for (int i = 0; i < spawnPointParent.childCount; i++)
        {
            var c = Resources.Load("Prefabs/Character/" + spawnPointParent.GetChild(i).name.Substring(0, spawnPointParent.GetChild(i).name.IndexOf('_'))) as GameObject;
            var cInstance = Instantiate(c, characterParent);
            cInstance.transform.position = spawnPointParent.GetChild(i).position;
            cInstance.transform.rotation = spawnPointParent.GetChild(i).rotation;
            cInstance.GetComponent<CharacterStatus>().playerNumber = int.Parse(spawnPointParent.GetChild(i).name.Substring(spawnPointParent.GetChild(i).name.IndexOf('_') + 1));
            cInstance.name = spawnPointParent.GetChild(i).name;
        }
        Destroy(spawnPointParent.gameObject);

        //Units
        var unitManager = UnitManager.GetInstance();
        unitManager.InitUnits();
        unitManager.units.ForEach(u => u.GetComponent<Unit>().UnitSelected += UIManager.GetInstance().OnUnitSelected);
        Units = unitManager.units;
        
        //DialogManager
        DialogManager.GetInstance().enabled = true;

        //Task
        var task = GameObject.Find("Canvas").transform.Find("BattlePrepare").Find("Task");
        task.Find("TaskTitle").GetComponent<Text>().text = level.Find("TaskTitle").GetComponent<Text>().text;
        task.Find("TaskContent").GetComponent<Text>().text = level.Find("TaskContent").GetComponent<Text>().text;
        Destroy(level.Find("TaskTitle").gameObject);
        Destroy(level.Find("TaskContent").gameObject);

        //Floor
        BattleFieldManager.GetInstance().BuildFloors(level.GetComponent<LevelInfo>().GridX, level.GetComponent<LevelInfo>().GridY);

        //Camera
        rtsCamera.transform.position = level.GetComponent<LevelInfo>().cameraStartPosition;
        rtsCamera.transform.rotation = Quaternion.Euler(level.GetComponent<LevelInfo>().cameraStartRotation);

        yield return StartCoroutine(XMLManager.LoadAsync<CharacterDataBase>(Application.streamingAssetsPath + "/XML/Core/Level/Level_Battle_" + Global.GetInstance().IndexToString(Global.GetInstance().BattleIndex) + ".xml", result => Global.GetInstance().levelCharacterDB = result));

        BattleBegin = false;

        //Players
        Players = new List<Player>();
        playersParent = level.Find("Players");
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
        
        yield return new WaitForSeconds(0.1f);

        if(Global.GetInstance().levelCharacterDB != null && Global.GetInstance().levelCharacterDB.characterDataList.Count > 0)
        {
            foreach (var characterData in Global.GetInstance().levelCharacterDB.characterDataList)
            {
                Global.GetInstance().characterDB.characterDataList.Add(characterData);
            }
        }

        GameObject.Find("Canvas").transform.Find("ScreenFader").GetComponent<ScreenFader>().FadeIn();
    }

    void UnloadLevel()
    {
        foreach (var characterData in Global.GetInstance().levelCharacterDB.characterDataList)
        {
            Global.GetInstance().characterDB.characterDataList.Remove(characterData);
        }
    }
    
    IEnumerator BattlePrepare()
    {
        //Units Initialize的时间
        yield return new WaitForSeconds(1);
        yield return new WaitUntil(() => { return BattleBegin; });

        Controller_Main.GetInstance().EndBattlePrepare();
        Destroy(Controller_Main.GetInstance());
        Destroy(battlePrepare);
    }

    IEnumerator FocusTeamMember()
    {
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
                yield return new WaitForSeconds(focusTime);
                Camera.main.GetComponent<RenderBlurOutline>().CancelRender();
                yield return new WaitForSeconds(0.5f);
            }
            Camera.main.GetComponent<RenderBlurOutline>().CancelRender();
        }
        //Camera.main.GetComponent<RTSCamera>().enabled = true;
    }

    void Start ()
    {
        GameController.GetInstance().Invoke(() =>
        {
            StartCoroutine(GameStart());
        }, 0.1f);
    }

    private void OnUnitClicked(object sender, EventArgs e)
    {
        RoundState.OnUnitClicked(sender as Unit);
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
                StartCoroutine(OnGameEnded(true));
                return true;
            case 2:
                StartCoroutine(OnGameEnded(false));
                return true;
        }
        return false;
    }
    
    private IEnumerator OnGameEnded(bool win)
    {
        if (GameEnded != null)
            GameEnded.Invoke(this, new EventArgs());
        gameEnded = true;
        SkillManager.GetInstance().skillQueue.Clear();
        Units.ForEach(u => u.gameObject.layer = 2);
        yield return StartCoroutine(DialogManager.GetInstance().PlayFinalDialog(win));
        
        if (win)
        {
            yield return StartCoroutine(Reward());
            UnloadLevel();
            Restart();
        }
        else
        {
            GameOver();
        }
    }
    
    private IEnumerator Reward()
    {
        var levelInfo = level.GetComponent<LevelInfo>();
        foreach (var unit in Units)
        {
            if(unit.playerNumber == 0)
            {
                var CS = unit.GetComponent<CharacterStatus>();
                var levelBonus = levelInfo.levelBonus - roundNumber * 25 > 0 ? levelInfo.levelBonus - roundNumber * 25 : 0;
                int finalExp = levelInfo.levelExp + levelBonus + CS.bonusExp;
                var expData = Global.GetInstance().characterDB.characterDataList.Find(d => d.roleEName == CS.roleEName && d.playerNumber == CS.playerNumber).attributes.Find(d => d.eName == "exp");
                if (expData.value + finalExp < expData.valueMax)
                {
                    expData.value += finalExp;
                }
                else
                {
                    finalExp -= expData.valueMax - expData.value;
                    CS.LevelUp();
                    expData.value += finalExp;
                }
            }
        }

            
        Global.GetInstance().ItemGenerator("Shuriken");
        yield return new WaitForSeconds(1);
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GameOver()
    {
        SceneManager.LoadScene("Start");
    }
    
    public void AddUnit(Unit unit)
    {
        unit.UnitClicked += OnUnitClicked;
        unit.UnitDestroyed += OnUnitDestroyed;
    }
}
