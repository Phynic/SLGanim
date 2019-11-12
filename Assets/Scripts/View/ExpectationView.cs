using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ExpectationView : ViewBase<ExpectationView>
{
    private Text Content_Expectation_EffectTitle;
    private Text Content_Expectation_EffectInfo;
    private Text Content_Expectation_RateTitle;
    private Text Content_Expectation_RateInfo;
    private Text Content_Expectation_AtkTitle;
    private Text Content_Expectation_AtkInfo;
    private Text Content_Expectation_DefTitle;
    private Text Content_Expectation_DefInfo;
    private Text Content_Expectation_DexTitle;
    private Text Content_Expectation_DexInfo;
    private Button Left;
    private Button Right;
    private RoleInfoElement roleInfoElement;

    private List<Transform> characters = new List<Transform>();
    private List<int> expectations = new List<int>();
    private List<string> finalRates = new List<string>();
    private int index;
    public void Open(List<Transform> characters, List<int> expectations, List<string> finalRates, UnityAction LeftButtonAction, UnityAction RightButtonActoin, UnityAction OnInit = null)
    {
        if (!isInit)
        {
            this.characters = characters;
            this.expectations = expectations;
            this.finalRates = finalRates;
            FindReferences();
            
            index = 0;

            if(characters.Count > 1)
            {
                Left.onClick.AddListener(LeftButtonAction);
                Right.onClick.AddListener(RightButtonActoin);
            }
            else
            {
                this.Left.gameObject.SetActive(false);
                this.Right.gameObject.SetActive(false);
            }
            Refresh(index);
        }
        base.Open(OnInit);
    }

    public void Refresh(int index)
    {
        var character = characters[index];
        var expectation = expectations[index];
        var finalRate = finalRates[index];

        roleInfoElement.Init(character);

        var attributes = character.GetComponent<CharacterStatus>().attributes;
        var atkInfo = attributes.Find(d => d.eName == "atk").Value.ToString();
        var defInfo = attributes.Find(d => d.eName == "def").Value.ToString();
        var dexInfo = attributes.Find(d => d.eName == "dex").Value.ToString();

        string effectTitle = expectation > 0 ? "ËðÉË" : "»Ö¸´";
        string effectInfo = Mathf.Abs(expectation).ToString();
        string rateInfo = finalRate + "%";

        Content_Expectation_EffectTitle.text = effectTitle;
        Content_Expectation_EffectInfo.text = effectInfo;
        Content_Expectation_RateInfo.text = rateInfo;
        Content_Expectation_AtkInfo.text = atkInfo;
        Content_Expectation_DefInfo.text = defInfo;
        Content_Expectation_DexInfo.text = dexInfo;
    }

    private void FindReferences()
    {
        Content_Expectation_EffectTitle = transform.Find("BodyPanel/Expectation/EffectTitle").GetComponent<Text>();
        Content_Expectation_EffectInfo = transform.Find("BodyPanel/Expectation/EffectInfo").GetComponent<Text>();
        Content_Expectation_RateTitle = transform.Find("BodyPanel/Expectation/RateTitle").GetComponent<Text>();
        Content_Expectation_RateInfo = transform.Find("BodyPanel/Expectation/RateInfo").GetComponent<Text>();
        Content_Expectation_AtkTitle = transform.Find("BodyPanel/Expectation/AtkTitle").GetComponent<Text>();
        Content_Expectation_AtkInfo = transform.Find("BodyPanel/Expectation/AtkInfo").GetComponent<Text>();
        Content_Expectation_DefTitle = transform.Find("BodyPanel/Expectation/DefTitle").GetComponent<Text>();
        Content_Expectation_DefInfo = transform.Find("BodyPanel/Expectation/DefInfo").GetComponent<Text>();
        Content_Expectation_DexTitle = transform.Find("BodyPanel/Expectation/DexTitle").GetComponent<Text>();
        Content_Expectation_DexInfo = transform.Find("BodyPanel/Expectation/DexInfo").GetComponent<Text>();
        roleInfoElement = transform.Find("BodyPanel/RoleInfoElement").GetComponent<RoleInfoElement>();

        Left = transform.Find("Left").GetComponent<Button>();
        Right = transform.Find("Right").GetComponent<Button>();
    }
}
