﻿using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections;
using UnityEngine.Events;

public class RoundManager : SingletonComponent<RoundManager>
{
    //状态机划分依据：
    //1.执行一次和每帧执行之间切换。
    //2.可以明确的阶段。

    public event UnityAction GameStarted;
    public event UnityAction GameEnded;
    public event UnityAction RoundStarted;
    public event UnityAction RoundEnded;
    public event UnityAction TurnStarted;
    public event UnityAction TurnEnded;
    public event UnityAction UnitEnded;

#if UNITY_EDITOR
    public float GameStartTime { get { return 0.1f; } private set { } }
    public float RoundStartTime { get { return 0.1f; } private set { } }
    public float TurnStartTime { get { return 0.1f; } private set { } }
    public float FocusTime { get { return 0.1f; } private set { } }
#else
    public float GameStartTime { get { return 1f; } private set { } }
    public float RoundStartTime { get { return 1f; } private set { } }
    public float TurnStartTime { get { return 1f; } private set { } }
    public float FocusTime { get { return 1f; } private set { } }
#endif

    public int PlayerCount { get; private set; }

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
    public int roundNumber = 0;

    public Unit CurrentUnit { get; set; }

    public int CurrentPlayerNumber { get; private set; }

    public List<Player> Players { get; private set; }

    public bool BattleBegin { get; set; }

    public List<Unit> Units;

    private RoundState _roundState;
    private Transform playersParent;
    private Transform level;
    private LevelInfo levelInfo;
    private VectoryCondition vc;

    IEnumerator GameStart()
    {
        yield return StartCoroutine(LoadLevel());
        yield return StartCoroutine(BattlePrepare());
        yield return StartCoroutine(FocusTeamMember());
        BattleView.GetInstance().Open();
        gameEnded = false;
        if (GameStarted != null)
            GameStarted.Invoke();
        //角色加入忽略层
        Units.ForEach(u => u.gameObject.layer = 2);
        yield return new WaitForSeconds(GameStartTime);

        foreach (var unit in Units)
        {
            unit.UnitClicked += OnUnitClicked;
            unit.UnitDestroyed += OnUnitDestroyed;
            unit.UnitSelected += OnUnitSelected;
            //设置同盟列表。
        }


        //yield return new WaitForSeconds(0.5f);
        StartCoroutine(RoundStart());
    }

    IEnumerator LoadLevel()
    {
        //LoadPrefab
        var r = Resources.LoadAsync("Prefabs/Level/Level_" + GameManager.IndexToString(Global.LevelID));
        yield return r;

        //LevelInit
        var go = Instantiate(r.asset) as GameObject;
        level = go.transform;
        levelInfo = LevelInfoDictionary.GetParam(Global.LevelID);
        level.name = r.asset.name;
        var rtsCamera = Camera.main.GetComponent<RTSCamera>();
        rtsCamera.cameraRange = level.Find("CameraRange").gameObject;
        rtsCamera.enabled = true;

        //VectoryCondition
        vc = level.GetComponent<VectoryCondition>();

        //SkillManager
        SkillManager.GetInstance().Init();

        //UIManamger
        UIManager.GetInstance().Init();

        //LoadUnits
        //Units Init
        Units = new List<Unit>();

        var characterParent = level.Find("Characters");
        var spawnPointParent = level.Find("SpawnPoints");
        for (int i = 0; i < spawnPointParent.childCount; i++)
        {
            var slot = PrefabManager.GetInstance().GetPrefabIns(Global.characterSlotPath, spawnPointParent.GetChild(i).position, Vector3.one, characterParent);

            var characterName = spawnPointParent.GetChild(i).name.Substring(0, spawnPointParent.GetChild(i).name.IndexOf('_'));

            var c = PrefabManager.GetInstance().GetPrefab(Global.characterRenderPath + characterName + "Render");
            var cInstance = Instantiate(c, slot.transform);
            cInstance.name = "Render";
            slot.transform.position = spawnPointParent.GetChild(i).position;
            slot.transform.rotation = spawnPointParent.GetChild(i).rotation;
            var cs = slot.GetComponent<Unit>();
            cs.playerNumber = int.Parse(spawnPointParent.GetChild(i).name.Substring(spawnPointParent.GetChild(i).name.IndexOf('_') + 1));
            slot.name = spawnPointParent.GetChild(i).name;

            characterName = Utils_String.ToUpperFirstLetter(characterName);
            var characterInfoID = CharacterInfoDictionary.GetParamList().Find(character => character.roleEName == characterName).ID;
            Units.Add(cs);
            cs.Init(Global.characterDataList.Find(d => d.characterInfoID == characterInfoID));
            var tempAnimator = cInstance.GetComponent<Animator>();
            var animator = slot.AddComponent<Animator>();
            animator.avatar = tempAnimator.avatar;
            animator.runtimeAnimatorController = tempAnimator.runtimeAnimatorController;
            animator.applyRootMotion = false;
            Destroy(tempAnimator);
        }
        Destroy(spawnPointParent.gameObject);

        //Units 我方 敌方(需补全)


        //FXManager
        FXManager.GetInstance();
        FXPool.GetInstance().Init();

        //DialogManager
        DialogManager.GetInstance().enabled = true;

        //Task
        BattlePrepareView.GetInstance().Open(levelInfo);

        //Floor
        BattleFieldManager.GetInstance().BuildFloors(levelInfo.grid[0], levelInfo.grid[1]);

        //Camera
        rtsCamera.transform.position = levelInfo.cameraStartPosition;
        rtsCamera.transform.rotation = Quaternion.Euler(levelInfo.cameraStartRotation);

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
        PlayerCount = Players.Count;
        CurrentPlayerNumber = Players.Min(p => p.playerNumber);

        yield return new WaitForSeconds(0.1f);

        MaskView.GetInstance().FadeIn();
    }

    IEnumerator RoundStart()
    {
        roundNumber++;
        if (RoundStarted != null)
            RoundStarted.Invoke();
        //角色加入忽略层
        Units.ForEach(u => u.gameObject.layer = 2);

        yield return new WaitForSeconds(RoundStartTime);

        Units.ForEach(u => { u.OnRoundStart(); });
        //yield return new WaitForSeconds(0.5f);
        StartCoroutine(TurnStart());
    }

    IEnumerator TurnStart()
    {
        Units.ForEach(u => { u.OnTurnStart(); });
        if (TurnStarted != null)
            TurnStarted.Invoke();
        yield return new WaitForSeconds(TurnStartTime);

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
            UnitEnded.Invoke();
        if (Units.FindAll(u => u.playerNumber == CurrentPlayerNumber && u.UnitEnd == false).Count > 0)    //当前玩家仍有角色未操作。
        {
            Players.Find(p => p.playerNumber.Equals(CurrentPlayerNumber)).Play();

        }
        if (Units.FindAll(u => u.playerNumber == CurrentPlayerNumber && u.UnitEnd == false).Count == 0)   //当前Player的所有Unit执行完毕
        {

            CurrentPlayerNumber = (CurrentPlayerNumber + 1) % PlayerCount;
            while (Units.FindAll(u => u.playerNumber.Equals(CurrentPlayerNumber)).Count == 0)
            {
                CurrentPlayerNumber = (CurrentPlayerNumber + 1) % PlayerCount;
            }//Skipping players that are defeated.

            Units.ForEach(u => { u.OnTurnEnd(); });
            if (TurnEnded != null)
                TurnEnded.Invoke();

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
            RoundEnded.Invoke();
        StartCoroutine(RoundStart());
    }

    public void ForceEndTurn()
    {
        var list = Units.FindAll(u => u.playerNumber == CurrentPlayerNumber);
        foreach (var u in list)
        {
            u.GetComponent<Unit>().OnUnitEnd();
        }
        EndTurn();
    }


    void UnloadLevel()
    {

    }

    IEnumerator BattlePrepare()
    {
        yield return new WaitUntil(() => { return BattleBegin; });

        //把BattlePrepare阶段的改动传递至角色
        Units.ForEach(u => { u.Init(Global.characterDataList.Find(d => d.characterInfoID == u.CharacterData.characterInfoID)); });
    }

    IEnumerator FocusTeamMember()
    {
        if (Camera.main.GetComponent<RenderBlurOutline>())
        {
            //Camera.main.GetComponent<RTSCamera>().enabled = false;
            foreach (var player in Players)
            {
                List<Transform> temp = new List<Transform>();
                if (Units.FindAll(u => u.playerNumber == player.playerNumber).Count > 0)
                {
                    foreach (var u in Units.FindAll(u => u.playerNumber == player.playerNumber))
                    {
                        temp.Add(u.transform);
                    }
                    Camera.main.GetComponent<RenderBlurOutline>().RenderOutLine(temp);
                    yield return new WaitForSeconds(FocusTime);
                    Camera.main.GetComponent<RenderBlurOutline>().CancelRender();
                    yield return new WaitForSeconds(0.5f);
                }
            }
            Camera.main.GetComponent<RenderBlurOutline>().CancelRender();
        }
        //Camera.main.GetComponent<RTSCamera>().enabled = true;
    }

    void Start()
    {
        MaskView.GetInstance().Open();
        Utils_Coroutine.GetInstance().Invoke(() =>
        {
            StartCoroutine(GameStart());
        }, 0.1f);
#if (!UNITY_EDITOR && (UNITY_IOS || UNITY_ANDROID))
        GameController.GetInstance().TwoTouches += BackSpace;
#endif
    }

    private void OnUnitClicked(Unit sender)
    {
        RoundState.OnUnitClicked(sender);
    }

    public void OnUnitSelected(Unit unit)
    {
        CurrentUnit = unit;
        CurrentUnit.GetComponent<CharacterAction>().SetSkill("FirstAction");
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
            GameEnded.Invoke();
        gameEnded = true;
        SkillManager.GetInstance().skillQueue.Clear();
        Units.ForEach(u => u.gameObject.layer = 2);
        yield return StartCoroutine(DialogManager.GetInstance().PlayFinalDialog(win));

        if (win)
        {
            //奖励
            //yield return StartCoroutine(Reward());
            UnloadLevel();
            yield return new WaitForSeconds(2f);
            Global.LevelID++;
            GameManager.GetInstance().ChangeProcedure<Procedure_Gal>();
            //Restart();
        }
        else
        {
            GameOver();
        }
    }

    //private IEnumerator Reward()
    //{
    //    var levelInfo = level.GetComponent<LevelInfo>();
    //    foreach (var unit in Units)
    //    {
    //        if (unit.playerNumber == 0)
    //        {
    //            var CS = unit.GetComponent<Unit>();
    //            var levelBonus = levelInfo.levelBonus - roundNumber * 25 > 0 ? levelInfo.levelBonus - roundNumber * 25 : 0;
    //            int finalExp = levelInfo.levelExp + levelBonus + CS.bonusExp;
    //            var expData = GameController.GetInstance().characterDB.characterDataList.Find(d => d.roleEName == CS.roleEName && d.playerNumber == CS.playerNumber).attributes.Find(d => d.eName == "exp");
    //            if (expData.value + finalExp < expData.valueMax)
    //            {
    //                expData.value += finalExp;
    //            }
    //            else
    //            {
    //                finalExp -= expData.valueMax - expData.value;
    //                CS.LevelUp();
    //                expData.value += finalExp;
    //            }
    //        }
    //    }

    //    //奖励物品
    //    GameController.GetInstance().ItemGenerator("Shuriken");
    //    yield return new WaitForSeconds(1);
    //}

    public void Restart()
    {
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GameOver()
    {
        //SceneManager.LoadScene("Start");
    }

    public void AddUnit(Unit unit)
    {
        Units.Add(unit);
        unit.UnitSelected += OnUnitSelected;
        unit.UnitClicked += OnUnitClicked;
        unit.UnitDestroyed += OnUnitDestroyed;
    }

    void Update()
    {
        //GetMousePosition();
#if (UNITY_STANDALONE || UNITY_EDITOR)
        if (Input.GetMouseButtonDown(1))
        {
            BackSpace();
        }
#endif
    }
    public void BackSpace()
    {
        if (SkillManager.GetInstance().skillQueue.Count > 0)
        {
            if (CurrentUnit)
            {
                //Debug.Log("UIManager : " + SkillManager.GetInstance().skillQueue.Peek().Key.CName + " 队列剩余 " + SkillManager.GetInstance().skillQueue.Count);
                if (!SkillManager.GetInstance().skillQueue.Peek().Key.done)
                {
                    SkillManager.GetInstance().skillQueue.Peek().Key.Reset();
                }
            }
        }
        else
        {
            var outline = Camera.main.GetComponent<RenderBlurOutline>();
            if (outline)
                outline.CancelRender();
            foreach (var f in BattleFieldManager.GetInstance().floors)
            {
                f.Value.SetActive(false);
            }

            //if (RoundManager.GetInstance().RoundState != null)
            //    ((RoundStateWaitingForInput)RoundManager.GetInstance().RoundState).DestroyPanel();
            RoleInfoView.TryClose();
        }
    }
}
