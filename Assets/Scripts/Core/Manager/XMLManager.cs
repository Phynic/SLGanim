using System;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;
using System.IO;
using System.Collections;
using UnityEngine.Networking;

public static class XMLManager
{
    //Create on disk
    public static void CreateXML<T>(string path) where T : new()
    {
        T t = new T();
        Save(t, path);
    }

    //SAVE
    public static void Save<T>(T t, string path)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(T));
        var encoding = System.Text.Encoding.GetEncoding("UTF-8");
        
        StreamWriter stream = new StreamWriter(path, false, encoding);
        serializer.Serialize(stream, t);
        stream.Close();
    }
    
    //LOAD
    public static T Load<T>(string path)
    {
        T t;
        XmlSerializer serializer = new XmlSerializer(typeof(T));
        StreamReader stream = new StreamReader(path);
        t = (T)serializer.Deserialize(stream);
        stream.Close();
        return t;
    }
    
    //深坑：这里的path只能外部传进来，写在内部在打包后无法读取。
    public static IEnumerator LoadAsync<T>(string path, Action<T> action)
    {
        T t;
        UnityWebRequest uwr = UnityWebRequest.Get(path);
        yield return uwr.SendWebRequest();

        XmlSerializer serializer = new XmlSerializer(typeof(T));
        StringReader sr = new StringReader(uwr.downloadHandler.text);
        try
        {
            sr.Read();      //跳过BOM头
            t = (T)serializer.Deserialize(sr);
            action(t);
            sr.Close();
        }
        catch
        {
            Debug.Log("无内容，或内容格式错误。");
        }
    }
}