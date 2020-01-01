using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ConfirmView : ViewBase<ConfirmView>
{
    private Button Left;
    private Text Left_Text;
    private Button Right;
    private Text Right_Text;
    private Text Text;

    public void Open(UnityAction LeftAction, UnityAction RightAction, UnityAction OnInit = null)
    {
        Open("确定吗？", "返回", "确定", LeftAction, RightAction, OnInit);
    }

    public void Open(string mainText, UnityAction LeftAction, UnityAction RightAction, UnityAction OnInit = null)
    {
        Open(mainText, "返回", "确定", LeftAction, RightAction, OnInit);
    }

    public void Open(string mainText, string leftText, string rightText, UnityAction LeftAction, UnityAction RightAction, UnityAction OnInit = null)
    {
        if (!isInit)
        {
            FindReferences();

            Text.text = mainText;

            if (leftText == "")
            {
                Destroy(Left.gameObject);
                Right.transform.localPosition = new Vector3(0, Right.transform.localPosition.y, Right.transform.localPosition.z);
            }
            else
            {
                LeftAction += Close;
                Left.onClick.AddListener(LeftAction);
                Left_Text.text = leftText;
            }
            
            RightAction += Close;
            Right.onClick.AddListener(RightAction);
            Right_Text.text = rightText;
        }
        base.Open(OnInit);
    }

    private void FindReferences()
    {
        Left = transform.Find("Left").GetComponent<Button>();
        Left_Text = transform.Find("Left/Text").GetComponent<Text>();
        Right = transform.Find("Right").GetComponent<Button>();
        Right_Text = transform.Find("Right/Text").GetComponent<Text>();
        Text = transform.Find("Text").GetComponent<Text>();
    }
}
