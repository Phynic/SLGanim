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
    public int SceneIndex { get; private set; }

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
        
        StartCoroutine(XMLManager.LoadSync<GameDataBase>(Application.streamingAssetsPath + "/XML/gameData.xml", result => gameDB = result));
        StartCoroutine(XMLManager.LoadSync<CharacterDataBase>(Application.streamingAssetsPath + "/XML/Preset/characterData.xml", result => characterDB = result));
        StartCoroutine(XMLManager.LoadSync<PlayerDataBase>(Application.streamingAssetsPath + "/XML/Preset/playerData.xml", result => playerDB = result));
        StartCoroutine(XMLManager.LoadSync<List<string>>(Application.streamingAssetsPath + "/XML/scenes.xml", result => scenes = result));
    }

    public void NextScene()
    {
        SceneIndex++;
        //前缀‘_’的为预加载场景
        if(scenes[SceneIndex][0] == '_')
        {
            SceneManager.LoadScene("Loading");
        }
        else
        {
            SceneManager.LoadScene(scenes[SceneIndex]);
        }
    }

    public void NextScene(string sceneName)
    {
        SceneIndex++;
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
