using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Events;

public class Floor : Touchable
{

    //public Material blueFloor;
    //public Material redFloor;
    //public Material yellowFloor;

    public Texture blueFloor;
    public Texture redFloor;
    public Texture yellowFloor;

    public UnityAction<GameObject> FloorClicked;

    public UnityAction<GameObject> FloorHovered;

    public UnityAction<GameObject> FloorExited;

#if (UNITY_STANDALONE || UNITY_EDITOR)
    protected virtual void OnMouseDown()
    {
        if (FloorClicked != null)
            FloorClicked.Invoke(gameObject);
    }

    protected virtual void OnMouseOver()
    {
        if (FloorHovered != null)
            FloorHovered.Invoke(gameObject);
    }

    protected virtual void OnMouseExit()
    {
        if (FloorExited != null)
            FloorExited.Invoke(gameObject);
    }
#endif
#if (!UNITY_EDITOR && (UNITY_IOS || UNITY_ANDROID))
    public override void OnTouchDown()
    {
        if (FloorHovered != null)
                FloorHovered.Invoke(gameObject);
    }

    public override void OnTouchUp()
    {
        if (FloorClicked != null)
                FloorClicked.Invoke(gameObject);
    }

    public override void OnTouchExited()
    {
        if (FloorExited != null)
            FloorExited.Invoke(gameObject);
    }
#endif




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
