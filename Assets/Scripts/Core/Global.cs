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
    public int SceneIndex { get; private set; }
    public int GalIndex { get; private set; }
    public int BattleIndex { get; private set; }
    public string PrepareScene { get; private set; }
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
        SceneIndex = 0;
        GalIndex = 0;
        StartCoroutine(XMLManager.LoadSync<GameDataBase>(Application.streamingAssetsPath + "/XML/Core/gameData.xml", result => gameDB = result));
        StartCoroutine(XMLManager.LoadSync<CharacterDataBase>(Application.streamingAssetsPath + "/XML/Preset/characterData.xml", result => characterDB = result));
        StartCoroutine(XMLManager.LoadSync<PlayerDataBase>(Application.streamingAssetsPath + "/XML/Preset/playerData.xml", result => playerDB = result));
    }

    public void NextScene(string sceneName)
    {
        SceneIndex++;
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
}
