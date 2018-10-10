using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T> {
    protected static T instance;

    public static T GetInstance()
    {
        return instance;
    }

    private void Awake()
    {
        instance = this as T;
    }
}
