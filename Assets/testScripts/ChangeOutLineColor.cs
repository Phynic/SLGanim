using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeOutLineColor : MonoBehaviour {

    Renderer[] rend;

    private void Start()
    {
        rend = GetComponentsInChildren<Renderer>();
    }

    private void OnMouseOver()
    {
        for(int i = 0; i < rend.Length; i++)
        {
            if (rend[i].material.shader.name == "ToonOutLine")
                rend[i].material.SetColor("_OutLineColor", new Color(0, 255, 255, 1));
        }
    }

    private void OnMouseExit()
    {
        for (int i = 0; i < rend.Length; i++)
        {
            if (rend[i].material.shader.name == "ToonOutLine")
                rend[i].material.SetColor("_OutLineColor", new Color(0, 0, 0, 1));
        }
    }
}
