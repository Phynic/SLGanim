using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Touchable : MonoBehaviour {
    public void OnTouchDown()
    {
        gameObject.BroadcastMessage("OnTouchDown");
    }

    public void OnTouchUp()
    {
        gameObject.BroadcastMessage("OnTouchUp");
    }
}
