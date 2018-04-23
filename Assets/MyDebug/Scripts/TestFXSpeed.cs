using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class TestFXSpeed : MonoBehaviour {
    public string abName= "Chakra";

    public void OnClickTest()
    {
        StartCoroutine(downloadFX());
    }

    private IEnumerator downloadFX()
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();
        string url = Application.streamingAssetsPath + "/"+abName + ".assetbundle";
        WWW www = new WWW(url);
        // Wait for FX AB to be loaded completely
        yield return www;
        sw.Stop();
        UnityEngine.Debug.Log(string.Format("HardDisk=>{0}ms", sw.ElapsedMilliseconds));

        sw.Start();
        // Get the byte data
        byte[] myData = www.bytes;

        // Create an AssetBundle from the bytes array
        AssetBundle bundle = AssetBundle.LoadFromMemory(myData);

        // You can now use your AssetBundle
        object fxPerfab = bundle.LoadAsset(abName);

        GameObject fx = Instantiate(fxPerfab as GameObject);

        fx.transform.localPosition = new Vector3(0, -1.5f, 0);
        fx.transform.SetParent(transform);

        bundle.Unload(true);
        sw.Stop();
        UnityEngine.Debug.Log(string.Format("Memory=>{0}ms", sw.ElapsedMilliseconds));
        yield return 1;
    }
}
