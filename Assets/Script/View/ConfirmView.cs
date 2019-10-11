using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ConfirmView : ViewBase<ConfirmView>
{
    private Button Return;
    private Text Return_Text;
    private Button Confirm;
    private Text Confirm_Text;
    private Text Text;

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
        Return = transform.Find("Return").GetComponent<Button>();
        Return_Text = transform.Find("Return/Text").GetComponent<Text>();
        Confirm = transform.Find("Confirm").GetComponent<Button>();
        Confirm_Text = transform.Find("Confirm/Text").GetComponent<Text>();
        Text = transform.Find("Text").GetComponent<Text>();
    }
}
