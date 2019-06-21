using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Component
{
    private static object _lock = new object();
    public static bool isInit = false;
    private static T _instance;

    public static T GetInstance()
    {
        if (applicationIsQuitting)
        {
            return null;
        }
        lock (_lock)
        {
            if (_instance == null)
            {
                _instance = (T)FindObjectOfType(typeof(T));

                if (FindObjectsOfType(typeof(T)).Length > 1)
                {
                    return _instance;
                }

                if (_instance == null)
                {
                    GameObject singleton = new GameObject();
                    _instance = singleton.AddComponent<T>();
                    singleton.name = "(singleton) " + typeof(T).ToString();

                    DontDestroyOnLoad(singleton);
                    isInit = true;
                }
            }
            return _instance;
        }
    }

    private static bool applicationIsQuitting = false;

    public void OnDestroy()
    {
        isInit = false;
        _instance = null;
        applicationIsQuitting = true;
    }
}