using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using UnityEngine.SceneManagement;

public class GameManager : SceneSingleton<GameManager> {
    [Header("Setting")]
    public bool battleTest = false;
    [Range(0f, 1f)]
    public float fadeTime = 0.5f;
    
    public int battleIndex = 0;
    [HideInInspector]
    public int maxSaveCount = 5;
    [Header("Build")]
    public bool useDecrypt = false;
    public bool playLogo = true;
    
    public static DataRegister playerData = new DataRegister();
    private Procedure gameProcedure;

    public int GalIndex { get; set; }
    public int BattleIndex { get { return battleIndex; } set { battleIndex = value; } }

#if (!UNITY_EDITOR && (UNITY_IOS || UNITY_ANDROID))
    private Config config;
#endif
    
    private void Start()
    {
#if !UNITY_EDITOR
        Destroy(GetComponent<Test>());
#endif
        LoadPrepare();
    }

    private void LoadPrepare()
    {
        DG.Tweening.DOTween.Init();

        SkillManager.GetInstance().InitSkillList();
        StartGame();
    }

#if (!UNITY_EDITOR && (UNITY_IOS || UNITY_ANDROID))
    public void ApplyConfig()
    {
        if(config.showFPS)
            gameObject.AddComponent<ShowFPS_OnGUI>();
        QualitySettings.SetQualityLevel(config.qualityLevel);
    }
#endif

    public void StartGame()
    {
        if (!battleTest)
        {
            ChangeProcedure<Procedure_Start>();
        }
        else
        {
            ChangeProcedure<Procedure_Battle>();
        }
    }

    public void Next(string sceneName)
    {
        MaskView.GetInstance().FadeOut(true, () =>
        {
            if (sceneName == "Gal")
            {
                GalView.GetInstance().Open();
            }
            else
            {
                SceneManager.LoadScene(sceneName);
            }
        });
    }

    public void ChangeProcedure<T>() where T : Procedure
    {
        ChangeProcedure(typeof(T));
    }

    public void ChangeProcedure(Type procedure)
    {
        MaskView.GetInstance().FadeOut(true, () => {
            if(gameProcedure == null)
            {
                gameProcedure = (Procedure)gameObject.AddComponent(procedure);
                gameProcedure.Enter();
            }
            else
            {
                if(gameProcedure.GetType() != procedure)
                {
                    gameProcedure.Exit();
                    gameProcedure = (Procedure)gameObject.AddComponent(procedure);
                }
                gameProcedure.Enter();
            }
        });
    }

    public string IndexToString(int index)
    {
        string indexString;
        indexString = index.ToString();
        int iter = 4 - indexString.Length;
        if (iter > 0)
        {
            for(int i = 0; i < iter; i++)
            {
                indexString = "0" + indexString;
            }
        }
        return indexString;
    }

    public void Save(string id)
    {
        Save save = new Save();
        save.ID = int.Parse(id);
        save.saveName = "存档" + id;
        save.playerData = playerData.ToArray();
        //        save.sceneName = SceneManager.GetActiveScene().name;
        //        save.battleIndex = BattleIndex;
        //        save.galIndex = GalIndex;
        //        save.characterDB = characterDB;
        //        save.timeStamp = Utils_Time.GenerateTimeStamp();
        //        saves.Remove(saves.Find(s => s.ID == save.ID));
        //        saves.Add(save);
        //        saves.Sort((x, y) => { return x.ID.CompareTo(y.ID); });
        //#if (UNITY_STANDALONE || UNITY_EDITOR)
        //        var path = Application.streamingAssetsPath + "/XML/Saves/" + id;
        //#elif (!UNITY_EDITOR && (UNITY_IOS || UNITY_ANDROID))
        //        var path = Application.persistentDataPath + "/XML/Saves/" + id;
        //#endif
        //        if (!Directory.Exists(path))
        //            Directory.CreateDirectory(path);

        //        XMLManager.Save(save, path + "/save.xml");
    }

    public void Load(string id)
    {
        //var save = saves.Find(s => s.ID == int.Parse(id));
        //characterDB = save.characterDB;
        //BattleIndex = save.battleIndex;
        //GalIndex = save.galIndex;
        //Next(save.sceneName);
    }

    public void Exit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}