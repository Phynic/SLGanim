using UnityEngine;
using System.Collections;
using System;

public class Floor : MonoBehaviour {

    //public Material blueFloor;
    //public Material redFloor;
    //public Material yellowFloor;

    public Texture blueFloor;
    public Texture redFloor;
    public Texture yellowFloor;
    
    public EventHandler FloorClicked;

    public EventHandler FloorHovered;

    public EventHandler FloorExited;
    

    protected virtual void OnMouseDown()
    {
        if (FloorClicked != null)
            FloorClicked.Invoke(gameObject, new EventArgs());
    }

    protected virtual void OnMouseOver()
    {
        if (FloorHovered != null)
            FloorHovered.Invoke(gameObject, new EventArgs());
    }

    protected virtual void OnMouseExit()
    {
        if (FloorExited != null)
            FloorExited.Invoke(this, new EventArgs());
    }

    //Floor变红色
    public void ChangeRangeColorToRed()
    {
        gameObject.GetComponent<Renderer>().material.mainTexture = redFloor;
    }
    //Floor变黄色
    public void ChangeRangeColorToYellow()
    {
        gameObject.GetComponent<Renderer>().material.mainTexture = yellowFloor;
    }

    //Floor变蓝色
    public void ChangeRangeColorToBlue()
    {
        gameObject.GetComponent<Renderer>().material.mainTexture = blueFloor;
    }
}
