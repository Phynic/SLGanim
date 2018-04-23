using UnityEngine;
using UnityEditor;
using System.Collections;

public class AssetBundleMaker : Editor
{
    [MenuItem("MyTools/AB/分别打AB包")]
    static void CreateAssetBunldesMain()
    {   //将选中的Asset分别进行打包，包名就是游戏对象的名字
        //获取在Project视图中选择的所有游戏对象
        Object[] SelectedAsset = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);

        //遍历所有的游戏对象
        foreach (Object obj in SelectedAsset)
        {
            string sourcePath = AssetDatabase.GetAssetPath(obj);
            //本地测试：建议最后将Assetbundle放在StreamingAssets文件夹下，如果没有就创建一个，因为移动平台下只能读取这个路径
            //StreamingAssets是只读路径，不能写入
            //服务器下载：就不需要放在这里，服务器上客户端用www类进行下载。
            string targetPath = Application.dataPath + "/StreamingAssets/" + obj.name + ".assetbundle";
            //BuildPipeline.BuildAssetBundles()
            if (BuildPipeline.BuildAssetBundle(obj, null, targetPath, BuildAssetBundleOptions.CollectDependencies, BuildTarget.Android))
            {
                Debug.Log(obj.name + "资源打包成功");
            }
            else
            {
                Debug.Log(obj.name + "资源打包失败");
            }
        }
        //刷新编辑器
        AssetDatabase.Refresh();
    }

    [MenuItem("MyTools/AB/多合一AB包")]
    static void CreateAssetBunldesALL()
    {   //将选中的Asset打包成一个包
        Caching.ClearCache();

        string fileName = "t17"; //自定义AB文件名，不用加.assetbundle

        Object[] SelectedAsset = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);

        //foreach (Object obj in SelectedAsset)
        //{
        //    Debug.Log("Create AssetBunldes name :" + obj);
        //    Texture2D sp = obj as Texture2D;
        //    string spName = sp.name;
        //    Debug.Log(spName);  
        //}

        string path = Application.dataPath + "/StreamingAssets/" + fileName + ".assetbundle";
        if (BuildPipeline.BuildAssetBundle(null, SelectedAsset, path, BuildAssetBundleOptions.CollectDependencies, BuildTarget.Android))
        {
            Debug.Log(fileName + "===>全体资源打包成功");
        }
        else
        {
            Debug.Log(fileName + "===>全体资源打包失败");
        }
        AssetDatabase.Refresh();
    }
}
