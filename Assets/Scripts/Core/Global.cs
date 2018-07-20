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
        
        instance.gameDB = XMLManager.GetInstance().LoadGameData();
        instance.characterDB = XMLManager.GetInstance().LoadCharacters();
    }

    
}
