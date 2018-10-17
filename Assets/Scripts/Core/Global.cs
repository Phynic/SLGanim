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
    public List<string> scenes = new List<string>();
    public int CurrentSceneIndex { get; private set; }

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
        CurrentSceneIndex = 0;
        StartCoroutine(XMLManager.GetInstance().LoadSync<GameDataBase>(Application.streamingAssetsPath + "/XML/gameData.xml", result => gameDB = result));
        StartCoroutine(XMLManager.GetInstance().LoadSync<CharacterDataBase>(Application.streamingAssetsPath + "/XML/characterData.xml", result => characterDB = result));
        StartCoroutine(XMLManager.GetInstance().LoadSync<PlayerDataBase>(Application.streamingAssetsPath + "/XML/playerData.xml", result => playerDB = result));
        StartCoroutine(XMLManager.GetInstance().LoadSync<List<string>>(Application.streamingAssetsPath + "/XML/scenes.xml", result => scenes = result));
    }

    public void NextScene()
    {
        CurrentSceneIndex++;
        //前缀‘_’的为预加载场景
        if(scenes[CurrentSceneIndex][0] == '_')
        {
            SceneManager.LoadScene("Loading");
        }
        else
        {
            SceneManager.LoadScene(scenes[CurrentSceneIndex]);
        }
    }

    public void NextScene(string sceneName)
    {
        CurrentSceneIndex++;
        if (sceneName[0] == '_')
        {
            SceneManager.LoadScene("Loading");
        }
        else
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}
