using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Touchable : MonoBehaviour {
    protected bool marked = false;

    public virtual void OnTouchDown()
    {
        marked = true;
    }

    public virtual void OnTouchUp()
    {
        marked = false;
    }

    public virtual void OnTouchExited()
    {
        marked = false;
    }
}
