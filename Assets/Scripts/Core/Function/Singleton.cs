using UnityEngine;
using System.Collections;

///// <summary>
///// 用于非继承MonBehaviour类的单例
///// </summary>
public class Singleton<T> where T : new()
{
    class SingletonCreator
    {
        static SingletonCreator() { }
        internal static readonly T instance = new T();
    }
    /// <summary>
    /// 非继承MonBehaviour类的单例
    /// </summary>
    /// <returns></returns>
    public static T Instance
    {
        get { return SingletonCreator.instance; }
    }
}