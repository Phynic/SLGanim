using UnityEngine;

public class Global : MonoBehaviour {

    private static Global instance;

    public GameDataBase gameDB;
    public CharacterDataBase characterDB;
    public PlayerDataBase playerDB;

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
        StartCoroutine(XMLManager.GetInstance().LoadCharacterData(Application.streamingAssetsPath + "/XML/characterData.xml"));
        StartCoroutine(XMLManager.GetInstance().LoadPlayerData(Application.streamingAssetsPath + "/XML/playerData.xml"));
    }

    public void OnLoadGameDataComplete()
    {
        instance.gameDB = XMLManager.GetInstance().gameDB;
    }

    public void OnLoadCharacterDataComplete()
    {
        instance.characterDB = XMLManager.GetInstance().characterDB;
    }

    public void OnLoadPlayerDataComplete()
    {
        instance.playerDB = XMLManager.GetInstance().playerDB;
    }
}
