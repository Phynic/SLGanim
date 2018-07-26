using UnityEngine;

public class Global : MonoBehaviour {

    private static Global instance;

    public GameDataBase gameDB;
    public CharacterDataBase characterDB;

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
        StartCoroutine(XMLManager.GetInstance().LoadGameData(Application.streamingAssetsPath + "/XML/gameData.xml"));
        StartCoroutine(XMLManager.GetInstance().LoadCharacters(Application.streamingAssetsPath + "/XML/characterData.xml"));
    }

    public void OnLoadGameDataComplete()
    {
        instance.gameDB = XMLManager.GetInstance().gameDB;
    }

    public void OnLoadCharactersComplete()
    {
        instance.characterDB = XMLManager.GetInstance().characterDB;
    }
}
