using UnityEngine;
using System.Collections;

public class ShowFPS_OnGUI : MonoBehaviour
{

    private float fpsMeasuringDelta = 0.5f;

    private float timePassed;
    private int m_FrameCount = 0;
    private float m_FPS = 0.0f;

    private void Start()
    {
        timePassed = 0.0f;
    }

    private void Update()
    {
        m_FrameCount = m_FrameCount + 1;
        timePassed = timePassed + Time.deltaTime;

        if (timePassed > fpsMeasuringDelta)
        {
            m_FPS = m_FrameCount / timePassed;
            timePassed = 0.0f;
            m_FrameCount = 0;
        }
    }

    private void OnGUI()
    {
        GUIStyle bb = new GUIStyle();
        bb.normal.background = null;    //这是设置背景填充的
        bb.normal.textColor = new Color(1.0f, 0.5f, 0.0f);   //设置字体颜色的
#if (UNITY_STANDALONE || UNITY_EDITOR)
        bb.fontSize = 10;       //当然，这是字体大小
#elif (!UNITY_EDITOR && (UNITY_IOS || UNITY_ANDROID))
        bb.fontSize = 25;
#endif
        //居中显示FPS
        GUI.Label(new Rect(1, 0, 80, 80), "FPS: " + (int)m_FPS, bb);
    }
}