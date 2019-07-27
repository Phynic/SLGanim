using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using UnityEngine.SceneManagement;

public class GameController : SceneSingleton<GameController> {
    [Header("Setting")]
    [Range(0f, 1f)]
    public float fadeTime = 0.5f;
    [HideInInspector]
    public int maxSaveCount = 5;
    [Header("Build")]
    public bool useDecrypt = false;
    public bool playLogo = true;
    [Header("Data")]
    public CharacterDataBase characterDB;
    public PlayerDataBase playerDB;
    public CharacterDataBase levelCharacterDB;
    public List<Save> saves = new List<Save>();
    public List<Growth> growthData = new List<Growth>();
    public Dictionary<string, string> nameDic = new Dictionary<string, string>();

    private Procedure gameProcedure;

    public int GalIndex { get; set; }
    public int BattleIndex { get; set; }

#if (!UNITY_EDITOR && (UNITY_IOS || UNITY_ANDROID))
    private Config config;
#endif
    
    private void Start()
    {
#if !UNITY_EDITOR
        Destroy(GetComponent<Test>());
#endif
        var characterList = CharacterDictionary.GetparamList();
        foreach (var character in characterList)
        {
            nameDic.Add(character.roleEName, character.roleCName);
        }

        StartCoroutine(LoadPrepare());
    }

    IEnumerator LoadPrepare()
    {
        yield return StartCoroutine(XMLManager.LoadAsync<List<Growth>>(Application.streamingAssetsPath + "/XML/Core/growth.xml", result => growthData = result));
#if (!UNITY_EDITOR && (UNITY_IOS || UNITY_ANDROID))
        yield return StartCoroutine(XMLManager.LoadAsync<Config>(Application.streamingAssetsPath + "/XML/Core/config.xml", result => config = result));
        if(config == null)
        {
            config = new Config();
            config.qualityLevel = 0;
            config.showFPS = true;
        }
        ApplyConfig();
#endif
        for (int i = 0; i < maxSaveCount; i++)
        {
#if (UNITY_STANDALONE || UNITY_EDITOR)
            yield return StartCoroutine(XMLManager.LoadAsync<Save>(Application.streamingAssetsPath + "/XML/Saves/" + IndexToString(i) + "/save.xml", result => { saves.Add(result); }));
#elif (!UNITY_EDITOR && (UNITY_IOS || UNITY_ANDROID))
            yield return StartCoroutine(XMLManager.LoadAsync<Save>("file:///" + Application.persistentDataPath + "/XML/Saves/" + IndexToString(i) + "/save.xml", result => { saves.Add(result); }));
#endif
        }

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
        ChangeProcedure<Procedure_Start>();
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

    public ItemData ItemGenerator(string itemName)
    {
        playerDB.items.Sort((x, y) => { return x.ID.CompareTo(y.ID); });
        int newID = playerDB.items[playerDB.items.Count - 1].ID + 1;
        ItemData newItem = new ItemData(newID, itemName);
        playerDB.items.Add(newItem);
        return newItem;
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
        save.sceneName = SceneManager.GetActiveScene().name;
        save.battleIndex = BattleIndex;
        save.galIndex = GalIndex;
        save.characterDB = characterDB;
        save.playerDB = playerDB;
        save.timeStamp = Utils_Time.GenerateTimeStamp();
        saves.Remove(saves.Find(s => s.ID == save.ID));
        saves.Add(save);
        saves.Sort((x, y) => { return x.ID.CompareTo(y.ID); });
#if (UNITY_STANDALONE || UNITY_EDITOR)
        var path = Application.streamingAssetsPath + "/XML/Saves/" + id;
#elif (!UNITY_EDITOR && (UNITY_IOS || UNITY_ANDROID))
        var path = Application.persistentDataPath + "/XML/Saves/" + id;
#endif
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        XMLManager.Save(save, path + "/save.xml");
    }

    public void Load(string id)
    {
        var save = saves.Find(s => s.ID == int.Parse(id));
        characterDB = save.characterDB;
        playerDB = save.playerDB;
        BattleIndex = save.battleIndex;
        GalIndex = save.galIndex;
        Next(save.sceneName);
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
