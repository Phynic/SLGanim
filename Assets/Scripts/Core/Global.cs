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
        StartCoroutine(XMLManager.GetInstance().LoadGameData());
        StartCoroutine(XMLManager.GetInstance().LoadCharacters());
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
