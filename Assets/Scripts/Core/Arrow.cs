using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour {

    public EventHandler ArrowClicked;
    public EventHandler ArrowHovered;
    public EventHandler ArrowExited;

    private void Start()
    {
        //GetComponent<SpriteRenderer>().color = Color.yellow;
    }

    private void OnMouseDown()
    {
        if (ArrowClicked != null)
            ArrowClicked.Invoke(gameObject, new EventArgs());
    }

    private void OnMouseOver()
    {
        if(ArrowHovered != null)
            ArrowHovered.Invoke(gameObject, new EventArgs());
    }

    private void OnMouseExit()
    {
        if (ArrowExited != null)
            ArrowExited.Invoke(gameObject, new EventArgs());
    }
}
