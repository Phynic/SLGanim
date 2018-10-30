using UnityEngine;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using UnityEngine.SceneManagement;

public class Global : MonoBehaviour {

    private static Global instance;

    public GameDataBase gameDB;
    public CharacterDataBase characterDB;
    public PlayerDataBase playerDB;
    public int GalIndex { get; set; }
    public int BattleIndex { get; private set; }
    public string PrepareScene { get; private set; }
    public CharacterDataBase levelCharacterDB;
    public Dictionary<string, string> nameDic = new Dictionary<string, string>();
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

    private void Awake()
    {
        GalIndex = 0;
    }

    private void Start()
    {
        StartCoroutine(XMLManager.LoadSync<GameDataBase>(Application.streamingAssetsPath + "/XML/Core/gameData.xml", result => gameDB = result));
        StartCoroutine(XMLManager.LoadSync<CharacterDataBase>(Application.streamingAssetsPath + "/XML/Preset/characterData.xml", result => characterDB = result));
        StartCoroutine(XMLManager.LoadSync<PlayerDataBase>(Application.streamingAssetsPath + "/XML/Preset/playerData.xml", result => playerDB = result));

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

        GameController.GetInstance().Invoke(() =>
        {
            XMLManager.Save<GameDataBase>(gameDB, Application.streamingAssetsPath + "/XML/Core/gameData.xml");
        }, 0.2f);
    }

    public void NextScene(string sceneName)
    {
        if (sceneName[0] == '_')
        {
            PrepareScene = sceneName.Substring(1);
            SceneManager.LoadScene("Loading");
        }
        else
        {
            SceneManager.LoadScene(sceneName);
        }
    }

    public ItemData ItemGenerator(string itemName)
    {
        playerDB.items.Sort((x, y) => { return x.ID.CompareTo(y.ID); });
        int newID = playerDB.items[playerDB.items.Count - 1].ID + 1;
        ItemData newItem = new ItemData(newID, itemName);
        playerDB.items.Add(newItem);
        return newItem;
    }
}
