using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameController : MonoBehaviour {
    private static GameController instance;

    RaycastHit lastHit;

    
    public EventHandler ThreeTouches;
    public static GameController GetInstance()
    {
        return instance;
    }

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
#if (UNITY_IOS || UNITY_ANDROID)
        RaycastHit hit = new RaycastHit();
        

        for (int i = 0; i < Input.touchCount; ++i)
        {
            
            if (Input.GetTouch(i).phase.Equals(TouchPhase.Began))
            {
                // Construct a ray from the current touch coordinates
                Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(i).position);
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.transform.GetComponent<Touchable>())
                    {
                        if (lastHit.transform && hit.transform != lastHit.transform)
                        {
                            if(lastHit.transform.GetComponent<Touchable>())
                                lastHit.transform.gameObject.SendMessage("OnTouchExited", SendMessageOptions.DontRequireReceiver);
                        }
                        hit.transform.gameObject.SendMessage("OnTouchDown");
                        lastHit = hit;
                    }
                }
            }

            //if (Input.GetTouch(i).phase.Equals(TouchPhase.Moved))
            //{

            //}

            if (Input.GetTouch(i).phase.Equals(TouchPhase.Ended))
            {
                // Construct a ray from the current touch coordinates
                Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(i).position);
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.transform.GetComponent<Touchable>())
                        hit.transform.gameObject.SendMessage("OnTouchUp");
                }
            }
            
            if(Input.touchCount == 3)
            {
                if (ThreeTouches != null)
                    ThreeTouches.Invoke(this, null);
            }
        }
        
#endif
    }


    public void Invoke(System.Object obj, string methodName, float delay)
    {
        StartCoroutine(InvokeCoroutine(obj, methodName, delay));
    }

    public IEnumerator InvokeCoroutine(System.Object obj, string methodName, float delay)
    {
        Type type = obj.GetType();
        var methodInfo = type.GetMethod(methodName);
        yield return new WaitForSeconds(delay);
        methodInfo.Invoke(obj, null);

    }

    public void Invoke(Action a, float delay)
    {
        StartCoroutine(InvokeCoroutine(a, delay));
    }

    public void Invoke(Action<int> a, float delay, int factor)
    {
        StartCoroutine(InvokeCoroutine(a, delay, factor));
    }

    public IEnumerator InvokeCoroutine(Action a, float delay)
    {
        yield return new WaitForSeconds(delay);
        a.Invoke();
    }

    public IEnumerator InvokeCoroutine(Action<int> a, float delay, int factor)
    {
        yield return new WaitForSeconds(delay);
        a.Invoke(factor);
    }
}
