using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenSetting : MonoBehaviour {
    Camera mainCamera;
    // Use this for initialization
    void Start () {
        //Screen.SetResolution(1600, 900, true, 60);
        mainCamera = Camera.main;
        //  float screenAspect = 1280 / 720;  现在android手机的主流分辨。
        //  mainCamera.aspect --->  摄像机的长宽比（宽度除以高度）
        mainCamera.aspect = 1.78f;
    }
}
