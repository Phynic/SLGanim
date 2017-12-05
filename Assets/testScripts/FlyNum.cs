using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class FlyNum : MonoBehaviour {
    Transform thousand;
    Transform hundred;
    Transform ten;
    Transform one;
    public int factor = 100;
    void Start () {
        thousand = GameObject.Find("Thousand").transform;
        hundred = GameObject.Find("Hundred").transform;
        ten = GameObject.Find("Ten").transform;
        one = GameObject.Find("One").transform;

        thousand.GetComponent<Text>().text = "2";
        hundred.GetComponent<Text>().text = "5";
        ten.GetComponent<Text>().text = "3";
        one.GetComponent<Text>().text = "8";

        
    }
	
    public void Fly()
    {
        
        thousand.transform.DOPunchPosition(Vector3.up * factor, 0.6f, 1, 0, true);
        Invoke(() => { hundred.transform.DOPunchPosition(Vector3.up * factor, 0.6f, 1, 0, true); }, 0.09f);
        Invoke(() => { ten.transform.DOPunchPosition(Vector3.up * factor, 0.6f, 1, 0, true); }, 0.18f);
        Invoke(() => { one.transform.DOPunchPosition(Vector3.up * factor, 0.6f, 1, 0, true); }, 0.27f);
    }


    public void Invoke(Action a, float delay)
    {
        StartCoroutine(InvokeCoroutine(a, delay));
    }

    public IEnumerator InvokeCoroutine(Action a, float delay)
    {

        yield return new WaitForSeconds(delay);
        a.Invoke();
    }
}
