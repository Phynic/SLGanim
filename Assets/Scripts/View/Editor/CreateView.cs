using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CreateView : MonoBehaviour
{
    private static string viewScriptPath = Application.dataPath + "/Script/View/";
    public static void GetComponentReferences(string goPath, Transform trans, Type[] types, ref Dictionary<string, Type> componentDic)
    {
        if (trans.childCount == 0)
        {
            return;
        }
        for (int i = 0; i < trans.childCount; i++)
        {
            var child = trans.GetChild(i);
            string path = goPath == "" ? child.name : goPath + "/" + child.name;
            foreach (var type in types)
            {
                if (child.GetComponent(type) != null)
                {
                    componentDic.Add(path, type);
                }
            }
            GetComponentReferences(path, child, types, ref componentDic);
        }
    }

    [MenuItem("Assets/程序工具/创建ViewScript")]
    public static void CreateViewScript()
    {
        UnityEngine.Object[] arr = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.DeepAssets);
        string path = AssetDatabase.GetAssetPath(arr[0]);

        Debug.Log("创建ViewScript" + path);
        string fileName = Path.GetFileName(path).Split('.')[0];

        GameObject go = (GameObject)arr[0];
        var buttons = go.GetComponentsInChildren<Button>();
        var texts = go.GetComponentsInChildren<Text>();



        Type[] types = new Type[] { typeof(Button), typeof(Text) /* 在这里添加新类型 */};



        Dictionary<string, Type> componentDic = new Dictionary<string, Type>();
        GetComponentReferences("", go.transform, types, ref componentDic);
        WriteViewScript(viewScriptPath, fileName, componentDic);
        AssetDatabase.Refresh();
    }
    public static void WriteViewScript(string path, string fileName, Dictionary<string, Type> componentDic)
    {
        Debug.Log(fileName);
        if (!Directory.Exists(@path))
        {
            Directory.CreateDirectory(@path);
        }

        FileStream fs = new FileStream(string.Format(path + "/{0}.cs", fileName), FileMode.Create);
        StreamWriter sw = new StreamWriter(fs);

        sw.WriteLine("using System;");
        sw.WriteLine("using System.Collections.Generic;");
        sw.WriteLine("using UnityEngine;");
        sw.WriteLine("using UnityEngine.UI;");
        sw.WriteLine("using UnityEngine.Events;");

        string viewContext = ViewTemplate.Replace("ClassName", fileName);

        foreach (var component in componentDic)
        {
            string cName = component.Key;
            while (cName.IndexOf('_') != -1)
            {
                cName = cName.Remove(cName.IndexOf('_'), 1);
            }
            string componentName = cName.Replace('/', '_');

            var full = component.Value.ToString().Split('.');
            string componentType = full[full.Length - 1];
            var index1 = viewContext.IndexOf("private ComponentType EXAMPLE;");
            viewContext = viewContext.Insert(index1, "private " + componentType + " " + componentName + ";\n    ");

            var index2 = viewContext.IndexOf(@"EXAMPLE = transform.Find(""POSITION"").GetComponent<ComponentType>();");
            viewContext = viewContext.Insert(index2, componentName + " = transform.Find(" + '"' + component.Key + '"' + ").GetComponent<" + componentType + ">()" + ";\n        ");
        }

        var example1 = "private ComponentType EXAMPLE;";
        var exampleIndex1 = viewContext.IndexOf(example1);
        viewContext = viewContext.Remove(exampleIndex1 - 5, example1.Length + 5);

        var example2 = @"EXAMPLE = transform.Find(""POSITION"").GetComponent<ComponentType>();";
        var exampleIndex2 = viewContext.IndexOf(example2);
        viewContext = viewContext.Remove(exampleIndex2 - 9, example2.Length + 9);

        viewContext = viewContext.Replace("ComponentType", "Button");

        sw.WriteLine(viewContext);
        //清空缓冲区
        sw.Flush();
        //关闭流
        sw.Close();
        fs.Close();
    }

    const string ViewTemplate = @"
public class ClassName : ViewBase<ClassName>
{
    private ComponentType EXAMPLE;

    public override void Open(UnityAction OnInit)
    {
        if (!isInit)
        {
            FindReferences();
        }
        base.Open(OnInit);
    }

    private void FindReferences()
    {
        EXAMPLE = transform.Find(""POSITION"").GetComponent<ComponentType>();
    }

    public override void Doback()
    {
        base.Doback();
    }
}";
}
