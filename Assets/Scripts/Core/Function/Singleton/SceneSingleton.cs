using UnityEngine;

public class SceneSingleton<T> : MonoBehaviour where T : SceneSingleton<T>
{
    protected static T instance;
    private void Awake()
    {
        //Debug.Log(typeof(T));
        instance = this as T;
    }

    public static T GetInstance()
    {
        return instance;
    }
}
