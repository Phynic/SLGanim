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
    
    [Header("Build")]
    public bool useDecrypt = false;
    public bool playLogo = true;
    
    
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

        //SkillManager.GetInstance().InitSkillList();
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

    //public void Next(string sceneName)
    //{
    //    MaskView.GetInstance().FadeOut(true, () =>
    //    {
    //        if (sceneName == "Gal")
    //        {
    //            GalView.GetInstance().Open();
    //        }
    //        else
    //        {
    //            SceneManager.LoadScene(sceneName);
    //        }
    //    });
    //}

    public void ChangeProcedure<T>() where T : Procedure
    {
        ChangeProcedure(typeof(T));
    }

    public void ChangeProcedure(string procedureName)
    {
        ChangeProcedure(Type.GetType(procedureName));
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

    public string GetProcedureName()
    {
        return gameProcedure.GetType().ToString();
    }

    public static string IndexToString(int index)
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

    public void Exit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}