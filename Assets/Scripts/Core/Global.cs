using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using UnityEngine.SceneManagement;

public class Global : MonoBehaviour {

    private static Global instance;

    public static string viewPath = "Prefabs/View/";

    public ScreenFader screenFader;
    public GameDataBase gameDB;
    public CharacterDataBase characterDB;
    public PlayerDataBase playerDB;
    public int GalIndex { get; set; }
    public int BattleIndex { get; set; }
    public string PrepareScene { get; private set; }
    public CharacterDataBase levelCharacterDB;
    public Dictionary<string, string> nameDic = new Dictionary<string, string>();
    public List<Save> saves = new List<Save>();
    public List<Growth> growthData = new List<Growth>();
    public int maxSaveCount = 5;
#if (!UNITY_EDITOR && (UNITY_IOS || UNITY_ANDROID))
    private Config config;
#endif
    public static Global GetInstance()
    {
        return instance;
    }

    static Global()
    {
        GameObject go = new GameObject("Global");
        DontDestroyOnLoad(go);
        instance = go.AddComponent<Global>();
    }

    private void Start()
    {
        StartCoroutine(XMLManager.LoadAsync<GameDataBase>(Application.streamingAssetsPath + "/XML/Core/gameData.xml", result => gameDB = result));
        
        nameDic.Add("Naruto", "旋涡 鸣人");
        nameDic.Add("Sasuke", "宇智波 佐助");
        nameDic.Add("Shikamaru", "奈良 鹿丸");
        nameDic.Add("Choji", "秋道 丁次");
        nameDic.Add("Neji", "日向 宁次");
        nameDic.Add("Lee", "洛克 李");
        nameDic.Add("Kiba", "犬冢 牙");
        nameDic.Add("Akamaru", "赤丸");
        nameDic.Add("Gaara", "我爱罗");
        nameDic.Add("Kankuro", "勘九郎");
        nameDic.Add("Temari", "手鞠");
        nameDic.Add("Kimimaro", "君麻吕");
        nameDic.Add("Kidoumaru", "鬼童丸");
        nameDic.Add("Tayuya", "多由也");
        nameDic.Add("Sakon", "左近");
        nameDic.Add("Ukon", "右近");
        nameDic.Add("Jiroubou", "次郎坊");
        
        //Util_Coroutine.GetInstance().Invoke(() =>
        //{
        //    //XMLManager.Save(gameDB, Application.streamingAssetsPath + "/XML/Core/gameData.xml");
        //    Save save = new Save();
        //    save.ID = 0;
        //    save.saveName = "存档1";
        //    save.sceneName = "Main";
        //    save.battleIndex = BattleIndex;
        //    save.galIndex = GalIndex;
        //    save.characterDB = characterDB;
        //    save.playerDB = playerDB;
        //    save.timeStamp = GenerateTimeStamp();
        //    XMLManager.Save(save, Application.streamingAssetsPath + "/XML/Saves/0001/save.xml");
        //}, 0.2f);

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
    }

#if (!UNITY_EDITOR && (UNITY_IOS || UNITY_ANDROID))
    public void ApplyConfig()
    {
        if(config.showFPS)
            gameObject.AddComponent<ShowFPS_OnGUI>();
        QualitySettings.SetQualityLevel(config.qualityLevel);
    }
#endif
    public static string GenerateTimeStamp()
    {
        TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
        return Convert.ToInt64(ts.TotalSeconds).ToString();
    }

    public string StampToDateTime(string timeStamp)
    {
        DateTime startTime = TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1, 0, 0, 0, 0), TimeZoneInfo.Local);
        long mTime = long.Parse(timeStamp + "0000000");
        TimeSpan toNow = new TimeSpan(mTime);
        //Debug.Log("\n 当前时间为：" + startTime.Add(toNow).ToString("yyyy年MM月dd日 HH:mm:ss"));
        return startTime.Add(toNow).ToString("yyyy年MM月dd日 HH:mm:ss");
    }

    public void NextScene(string sceneName)
    {
        screenFader.FadeOut(true, () => {
            if (sceneName[0] == '_')
            {
                PrepareScene = sceneName.Substring(1);
                SceneManager.LoadScene("Loading");
            }
            else
            {
                SceneManager.LoadScene(sceneName);
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
        save.timeStamp = GenerateTimeStamp();
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
        NextScene(save.sceneName);
    }
}
